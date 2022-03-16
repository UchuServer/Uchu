using System.Numerics;

namespace Uchu.NavMesh.Shape;

public class OrderedShape
{
    /// <summary>
    /// Points of the ordered shape.
    /// </summary>
    public List<Vector2> Points { get; set; } = new List<Vector2>();

    /// <summary>
    /// Returns the cross product of 2 2D vectors.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <returns>The cross product of the 2 vectors.</returns>
    private static double Cross(Vector2 point1, Vector2 point2)
    {
        return (point1.X * point2.Y) - (point1.Y * point2.X);
    }
    
    /// <summary>
    /// Optimizes the shape by removing points to make longer lines.
    /// </summary>
    public void Optimize()
    {
        // Remove points that are in the middle of straight lines.
        var currentIndex = 0;
        while (currentIndex < Points.Count - 2)
        {
            var point = this.Points[currentIndex];
            var remainingPoints = this.Points.Count;
            for (var i = currentIndex + 1; i < remainingPoints - 2; i++)
            {
                var middlePoint = this.Points[currentIndex + 1];
                var endPoint = this.Points[currentIndex + 2];
                if (Math.Abs(Math.Atan2(endPoint.Y - middlePoint.Y, endPoint.X - middlePoint.X) - Math.Atan2(middlePoint.Y - point.Y, middlePoint.X - point.X)) > 0.01) break;
                this.Points.Remove(middlePoint);
            }
            currentIndex += 1;
        }
        
        // Remove the last point if the last and first line are collinear.
        if (Math.Abs(Math.Atan2(this.Points[^1].Y - this.Points[0].Y, this.Points[^1].X - this.Points[0].X) - Math.Atan2(this.Points[0].Y - this.Points[1].Y, this.Points[0].X - this.Points[1].X)) < 0.01)
        {
            this.Points.RemoveAt(0);
        }
    }

    /// <summary>
    /// Returns if a point is in the shape.
    /// </summary>
    /// <param name="point">Point to check.</param>
    /// <returns>Whether the point is in the shape.</returns>
    public bool PointInShape(Vector2 point)
    {
        // Get the lines that are left of the point.
        var linesLeftOfPoint = 0;
        for (var i = 0; i < this.Points.Count; i++)
        {
            var currentPoint = this.Points[i];
            if (point == currentPoint) return true;
            var lastPoint = this.Points[i == 0 ? this.Points.Count - 1 : (i - 1)];
            if (!((currentPoint.Y > point.Y && lastPoint.Y < point.Y) || (currentPoint.Y < point.Y && lastPoint.Y > point.Y))) continue;
            var lineRatio = (point.Y - currentPoint.Y) / (lastPoint.Y - currentPoint.Y);
            var lineX = currentPoint.X + ((lastPoint.X - currentPoint.X) * lineRatio);
            if (lineX > point.X) continue;
            linesLeftOfPoint += 1;
        }
        
        // Return if the points to the left is odd.
        return linesLeftOfPoint % 2 == 1;
    }

    
    
    /// <summary>
    /// Returns if a line is valid for the shape. A line is considered valid if 
    /// </summary>
    /// <param name="start">Start point of the line.</param>
    /// <param name="end">End point of the line.</param>
    /// <returns>Whether the line is valid.</returns>
    public bool LineValid(Vector2 start, Vector2 end)
    {
        // Return false if at least 1 line intersects.
        var lineDelta1 = end - start;
        for (var i = 0; i < this.Points.Count; i++)
        {
            // Get the start and end. Ignore if the start or end of the line match the start or end of the parameters.
            var currentPoint = this.Points[i];
            var lastPoint = this.Points[i == 0 ? this.Points.Count - 1 : (i - 1)];
            if ((currentPoint == start && lastPoint == end) || (lastPoint == start && currentPoint == end)) return true;
            if (currentPoint == start || currentPoint == end) continue;
            if (lastPoint == start || lastPoint == end) continue;

            // Return false if the lines intersect.
            var lineDelta2 = lastPoint - currentPoint;
            var mainCross = Cross(lineDelta1, lineDelta2);
            var coefficient1 = Cross(currentPoint - start, lineDelta1) / mainCross;
            var coefficient2 = Cross(currentPoint - start, lineDelta2) / mainCross;
            if (coefficient1 >= 0 && coefficient1 <= 1 && coefficient2 >= 0 && coefficient2 <= 1)
                return false;
        }
        
        // Return false if the middle of the line is not in the shape.
        if (!this.PointInShape(new Vector2(start.X + ((end.X - start.X) / 2), start.Y + ((end.Y - start.Y) / 2))))
            return false;

        // Return true (valid).
        return true;
    }
}