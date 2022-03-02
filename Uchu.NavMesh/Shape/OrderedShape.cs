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
}