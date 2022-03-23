using System.Numerics;
using Uchu.NavMesh.Graph;

namespace Uchu.NavMesh.Shape;

public class OrderedShape
{
    /// <summary>
    /// Result for a line intersection test.
    /// </summary>
    public enum LineIntersectionResult {
        LineIntersects,
        NoLineIntersects,
        PartOfShape,
    }
    
    /// <summary>
    /// Points of the ordered shape.
    /// </summary>
    public List<Vector2> Points { get; set; } = new List<Vector2>();

    /// <summary>
    /// Nodes of the shape.
    /// </summary>
    public List<Node> Nodes { get; set; } = new List<Node>();

    /// <summary>
    /// Shapes that are contained in the shape.
    /// </summary>
    public List<OrderedShape> Shapes { get; set; } = new List<OrderedShape>();

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
    /// Generates the connected nodes of the shape.
    /// </summary>
    public void GenerateGraph()
    {
        // Create the nodes.
        foreach (var point in this.Points)
        {
            this.Nodes.Add(new Node(point));
        }
        
        // Connect the nodes.
        foreach (var node in this.Nodes)
        {
            foreach (var otherNode in this.Nodes)
            {
                if (node == otherNode) continue;
                if (!this.LineValid(node.Point, otherNode.Point)) continue;
                node.Nodes.Add(otherNode);
            }
        }
    }

    /// <summary>
    /// Tries to add a child shape.
    /// </summary>
    /// <param name="shape">Shape to try to add.</param>
    /// <returns>Whether the shape was added.</returns>
    public bool TryAddShape(OrderedShape shape)
    {
        // Return false if there is a point not in the shape.
        foreach (var point in shape.Points)
        {
            if (!this.PointInShape(point)) return false;
        }
        
        // Return true if it can be added directly to a child shape.
        foreach (var otherShape in this.Shapes)
        {
            if (!otherShape.TryAddShape(shape)) continue;
            return true;
        }
        
        // Add the child shape directly and remove the child shapes that are contained in the new shape.
        foreach (var otherShape in this.Shapes.ToList())
        {
            if (!shape.TryAddShape(otherShape)) continue;
            this.Shapes.Remove(otherShape);
        }
        this.Shapes.Add(shape);
        return true;
    }

    /// <summary>
    /// Returns if a point is in the shape.
    /// </summary>
    /// <param name="point">Point to check.</param>
    /// <returns>Whether the point is in the shape.</returns>
    public bool PointInShape(Vector2 point)
    {
        // Get the sum of the angles of the point to every pair of points that form the lines.
        var totalAngle = 0d;
        for (var i = 0; i < this.Points.Count; i++)
        {
            // Get the current point and last point.
            var currentPoint = this.Points[i];
            if (point == currentPoint) return true;
            var lastPoint = this.Points[i == 0 ? this.Points.Count - 1 : (i - 1)];

            // Determine the angles to each point and determine the angle difference.
            var theta1 = Math.Atan2(currentPoint.Y - point.Y, currentPoint.X - point.X);
            var theta2 = Math.Atan2(lastPoint.Y - point.Y, lastPoint.X - point.X);
            var thetaDelta = theta2 - theta1;
            while (thetaDelta > Math.PI)
            {
                thetaDelta += -(2 * Math.PI);
            }
            while (thetaDelta < -Math.PI)
            {
                thetaDelta += (2 * Math.PI);
            }

            // Add the difference.
            totalAngle += thetaDelta;
        }

        // Return if the sum is 360 degrees.
        // If it is 0, the point is outside the polygon.
        return Math.Abs(totalAngle) > Math.PI;
    }

    /// <summary>
    /// Returns if the given line intersects the shape.
    /// Inner shapes are not checked.
    /// </summary>
    /// <param name="start">Start point of the line.</param>
    /// <param name="end">End point of the line.</param>
    /// <returns>Whether the line intersects the shape.</returns>
    public LineIntersectionResult LineIntersects(Vector2 start, Vector2 end)
    {
        // Return true if at least 1 line intersects.
        var lineDelta1 = end - start;
        for (var i = 0; i < this.Points.Count; i++)
        {
            // Get the start and end. Ignore if the start or end of the line match the start or end of the parameters.
            var currentPoint = this.Points[i];
            var lastPoint = this.Points[i == 0 ? this.Points.Count - 1 : (i - 1)];
            if ((currentPoint == start && lastPoint == end) || (lastPoint == start && currentPoint == end)) return LineIntersectionResult.PartOfShape;
            if (currentPoint == start || currentPoint == end) continue;
            if (lastPoint == start || lastPoint == end) continue;

            // Return false if the lines intersect.
            var lineDelta2 = lastPoint - currentPoint;
            var mainCross = Cross(lineDelta1, lineDelta2);
            var coefficient1 = Cross(currentPoint - start, lineDelta1) / mainCross;
            var coefficient2 = Cross(currentPoint - start, lineDelta2) / mainCross;
            if (coefficient1 >= 0 && coefficient1 <= 1 && coefficient2 >= 0 && coefficient2 <= 1)
                return LineIntersectionResult.LineIntersects;
        }

        // Return false (doesn't intersect).
        return LineIntersectionResult.NoLineIntersects;
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
        var lineIntersectResult = this.LineIntersects(start, end);
        if (lineIntersectResult == LineIntersectionResult.PartOfShape)
            return true;
        if (lineIntersectResult == LineIntersectionResult.LineIntersects)
            return false;
        
        // Return false if a contained shape intersects or the center of the line is inside the contained shape.
        foreach (var shape in this.Shapes)
        {
            var containedLineIntersectResult = shape.LineIntersects(start, end);
            if (containedLineIntersectResult == LineIntersectionResult.PartOfShape)
                return true;
            if (containedLineIntersectResult == LineIntersectionResult.LineIntersects)
                return false;
            if (shape.PointInShape(new Vector2(start.X + ((end.X - start.X) * 0.5f), start.Y + ((end.Y - start.Y) * 0.5f))))
                return false;
        }

        // Return false if the middle of the line is not in the shape.
        if (!this.PointInShape(new Vector2(start.X + ((end.X - start.X) / 2), start.Y + ((end.Y - start.Y) / 2))))
            return false;

        // Return true (valid).
        return true;
    }
}