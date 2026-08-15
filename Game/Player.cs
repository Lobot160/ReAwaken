using Godot;
using System.Reflection;
using System.Text.Json;

public partial class Player : CharacterBody2D
{
	public float speed, jumpPower, acceleration;

	public Inventory EquipedUpgrades { get; } = new ("playerUpgrade", energyLimit: 1000);
	public Inventory ReservedUpgrades { get; } = new ("playerUpgrade", capacity: 10);


	public void updateStats()
	{
		foreach (var item in EquipedUpgrades)
		{
			var stats = Inventory.GetStats(EquipedUpgrades.ItemType, item.Key);
			foreach (var stat in stats.EnumerateObject())
			{
				FieldInfo field = GetType().GetField(stat.Name);
				field?.SetValue(this, stat.Value.GetSingle());
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
			GD.Print($"Cannot add {quantity} of {itemName}. Inventory is full.");
		};
		EquipedUpgrades.OverEnergyLimit += (itemName, quantity) =>
		{
			GD.Print($"Cannot add {quantity} of {itemName}. Energy limit exceeded.");
		};

		EquipedUpgrades.AddItem("basicHealth", 3);
		EquipedUpgrades.AddItem("basicSpeed", 3);
		EquipedUpgrades.AddItem("basicJump", 3);
		GD.Print($"{speed}, {jumpPower}, {acceleration}");
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y += jumpPower;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, acceleration * (float)delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, acceleration * (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}


}
