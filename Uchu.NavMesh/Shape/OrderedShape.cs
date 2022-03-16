using System.Numerics;

namespace Uchu.NavMesh.Shape;

public class OrderedShape
{
    /// <summary>
    /// Points of the ordered shape.
    /// </summary>
    public List<Vector2> Points { get; set; } = new List<Vector2>();

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
}