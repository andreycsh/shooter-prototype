using System.Collections.Generic;
using System.Linq;
using Godot;

[Tool]
public partial class RetickleDraw : Control
{
    [Export]
    public float Radius { get; set; } = 30.0f;

    [Export]
    public float Thickness { get; set; } = 1.0f;

    [Export]
    public Color _Color { get; set; } = Colors.White;

    [Export]
    public float GapAngle { get; set; } = 45.0f;

    [Export]
    public int segments { get; set; } = 32;

    public override void _Draw()
    {
        this.DrawCircleCrosshair();
    }

    public void DrawCircleCrosshair()
    {
        float gapRad = Mathf.DegToRad(GapAngle);

        float[][] arcSegment = [
            // Bottom-right quadrant
            [gapRad / 2, Mathf.Pi / 2 - gapRad / 2],
            // Bottom-left qiadrant
            [Mathf.Pi / 2 + gapRad / 2, Mathf.Pi - gapRad / 2],
            // Top-left quadrant
            [Mathf.Pi + gapRad / 2, 3 * Mathf.Pi / 2 - gapRad / 2],
            // Top-right quadrant
            [3 * Mathf.Pi / 2 + gapRad / 2, 2 * Mathf.Pi - gapRad / 2]
        ];

        foreach (float[] arc in arcSegment)
        {
            float startAngle = arc[0];
            float endAngle = arc[1];

            List<Vector2> points = [];
            float angleStep = (endAngle - startAngle) / segments;

            foreach (int i in Enumerable.Range(0, segments))
            {
                float angle = startAngle + i * angleStep;
                Vector2 point = new Vector2(Radius * Mathf.Cos(angle), Radius * Mathf.Sin(angle));
                points.Add(point);
            }

            if(points.Count > 1)
            {
                DrawPolyline([.. points], _Color, Thickness, true);
            }
        }
    }

    public void UpdateCrosshair()
    {
        QueueRedraw();
    }

}
