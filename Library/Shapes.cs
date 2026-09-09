using Godot;
using System;


public static partial class Library
{
    public static Vector2[] Points(this Shape2D shape)
    {
        return shape switch
        {
            RectangleShape2D rectangleShape => rectangleShape.Points(),
            ConvexPolygonShape2D convexShape => convexShape.Points,
            ConcavePolygonShape2D concaveShape => concaveShape.Segments,
            _ => throw new ArgumentException("Unsupported shape type.")
        };
    }

    public static Vector2[] Points(this RectangleShape2D rectangleShape)
    {
        Vector2 extents = rectangleShape.Size / 2;
        return
        [
            new Vector2(-extents.X, -extents.Y),
            new Vector2(extents.X, -extents.Y),
            new Vector2(extents.X, extents.Y),
            new Vector2(-extents.X, extents.Y)
        ];
    }

    public static Polygon2D ToPolygon(this Shape2D shape, Color color)
	{
		if (shape is RectangleShape2D rectangleShape)
		{
			return new Polygon2D
			{
				Polygon = rectangleShape.Points(),
				Color = color
			};
		}
		else if (shape is ConvexPolygonShape2D convexShape)
		{
			return new Polygon2D
			{
				Polygon = convexShape.Points,
				Color = color
			};
		}
		else if (shape is ConcavePolygonShape2D concaveShape)
		{
			return new Polygon2D
			{
				Polygon = concaveShape.Segments,
				Color = color
			};
		}
		else
		{
			throw new ArgumentException("Unsupported shape type.");
		}
	}

}