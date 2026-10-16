using Godot;
using System;
using System.Reflection;
using System.Text.Json;
using static Library;

public partial class Player : CharacterBody2D
{
	public float speed, jumpPower, acceleration;

	public Vector2 movementVelocity = Vector2.Zero;

	public Inventory EquipedUpgrades { get; } = new ("playerUpgrade", energyLimit: 1000);
	public Inventory ReservedUpgrades { get; } = new ("playerUpgrade", capacity: 10);



	public double FloorFriction { get; set; } = 0.04;


	//Helps stick to floor
	private bool stick = true;
	private float LastFloorAngle = 0f;
	private float FloorSnapAngle = 30f;

	public void updateStats()
	{
		foreach (var item in EquipedUpgrades)
		{
			var stats = Inventory.GetStats(EquipedUpgrades.ItemType, item.Key);
			foreach (var stat in stats.EnumerateObject())
			{
				FieldInfo field = GetType().GetField(stat.Name);
				field?.SetValue(this, stat.Value.GetSingle() * item.Value);
			}
		}
	}

	public override void _Ready()
	{
	}

	public Player()
	{
		EquipedUpgrades.OnItemAdded += (itemName, quantity) =>
		{
			updateStats();
		};
		EquipedUpgrades.OverCapacity += (itemName, quantity) =>
		{
			GD.PrintErr($"Cannot add {quantity} of {itemName}. Inventory is full.");
		};
		EquipedUpgrades.OverEnergyLimit += (itemName, quantity) =>
		{
			GD.PrintErr($"Cannot add {quantity} of {itemName}. Energy limit exceeded.");
		};

		EquipedUpgrades.AddItem("basicHealth", 3);
		EquipedUpgrades.AddItem("basicSpeed", 3);
		EquipedUpgrades.AddItem("basicJump", 3);
		GD.Print($"{speed}, {jumpPower}, {acceleration}");
		
	}

	public override void _PhysicsProcess(double delta)
	{
		

		//copy of main Velocity
		Vector2 velocity = Velocity;

		

		float inputDir = Input.GetAxis("left", "right");
		velocity.X +=  inputDir * acceleration * (float)delta;

		if (IsOnFloor())
		{
			LastFloorAngle = GetFloorAngle();
			stick = true;
		}

		if (stick && LastFloorAngle < Mathf.DegToRad(FloorSnapAngle))
		{
			ApplyFloorSnap();
		}
		
		if (!IsOnFloor()) 
		{
			velocity += GetGravity() * (float)delta;
			stick = false;
		}



		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && CanJump())
		{
			velocity.Y = -jumpPower;
			stick = false;
		}

		if (Input.IsActionJustPressed("debug"))
		{
			//Insert breakpoint here
			GD.Print("Debug action pressed");
		}

		Velocity = velocity.Drag(0.04f, delta);


		

		if (MoveAndSlide())
		{
			Velocity = Velocity.Slide(GetFloorNormal());
		}
		
		
	}
	

	public bool CanJump()
	{
		//Able to add multiple flags for jumping
		return IsOnFloor();
	}
}
