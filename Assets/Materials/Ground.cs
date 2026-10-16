using Godot;
using System;


[Tool]
[GlobalClass]
public partial class Ground : StaticBody2D
{
	


	[Export(PropertyHint.ResourceType, "RectangleShape2D,ConvexPolygonShape2D,ConcavePolygonShape2D")]
	private Shape2D Shape = new RectangleShape2D();
	
	private Vector2[] Points { get => Shape.Points();}

	[Export]
	private Color color = new Color(0.5f, 0.5f, 0.5f);

	[Export]
	private CollisionShape2D collisionShape = null;
	private Polygon2D polygon = null;

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

			collisionShape.Owner = Owner ?? this;
			collisionShape.Shape = Shape;
			polygon.Polygon = Points;
			polygon.Color = color;

			collisionShape.Position = Vector2.Zero;
		}
	}

	public void SetupNodes()
	{
		collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collisionShape == null)
		{
			collisionShape = new CollisionShape2D{ Name = "CollisionShape2D", Shape = Shape};
			AddChild(collisionShape);
			
			collisionShape.Owner = Owner ?? this;
		}

		polygon = GetNodeOrNull<Polygon2D>("Polygon2D");
		if (polygon == null)
		{
			polygon = Shape.ToPolygon(color);
			AddChild(polygon);
		}
	}
}
