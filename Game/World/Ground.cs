using Godot;
using System;


[Tool]
[GlobalClass]
public partial class Ground : StaticBody2D
{
	[Export(PropertyHint.ResourceType, "RectangleShape2D,ConvexPolygonShape2D,ConcavePolygonShape2D")]
	private Shape2D shape;

	[Export]
	private Color color = new Color(0.5f, 0.5f, 0.5f);

	private CollisionShape2D collisionShape;
	private Polygon2D polygon;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetupNodes();
	}

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{	
			if (collisionShape == null || polygon == null)
			{
				SetupNodes();
			}

			collisionShape.Shape = shape;
			polygon.Polygon = shape.Points();
			polygon.Color = color;
		}
	}

	public void SetupNodes()
	{
		collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collisionShape == null)
		{
			collisionShape = new CollisionShape2D{Shape = shape};
			AddChild(collisionShape, false, InternalMode.Front);
		}

		polygon = GetNodeOrNull<Polygon2D>("Polygon2D");
		if (polygon == null)
		{
			polygon = shape.ToPolygon(color);
			AddChild(polygon, false, InternalMode.Front);
		}
	}
}
