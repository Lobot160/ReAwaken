using Godot;
using System;
using System.Reflection;
using System.Text.Json;
using static Library;

using static IHealth;

public partial class Player : Character, IHealth
{

	public Inventory EquipedUpgrades { get; } = new ("playerUpgrade", energyLimit: 1000);
	public Inventory ReservedUpgrades { get; } = new ("playerUpgrade", capacity: 10);



	public double FloorFriction { get; set; } = 0.04;

    public float Health {get; private set;}
    public float MaxHealth {get; private set;}

    public bool IsDead {get; private set;} = false;


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

		

		Vector2 inputDir = new (Input.GetAxis("left", "right"), 0);

		ApplyMovement(inputDir, delta);

		if (IsOnFloor())
		{
			LastFloorAngle = GetFloorAngle();
			stick = true;
		}

		if (stick && LastFloorAngle < Mathf.DegToRad(FloorSnapAngle))
		{
			ApplyFloorSnap();
		}


		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && CanJump)
		{
			ApplyJump();
		}

		if (Input.IsActionJustPressed("debug"))
		{
			//Insert breakpoint here
			GD.Print("Debug action pressed");
		}
		

		base._PhysicsProcess(delta);
	}
	
	

    public void TakeDamage(Damage amount)
    {
        Health -= amount.Total;
    }

    public void Heal(float amount)
    {
		
        Health += amount;
    }

    public void OnDeath()
    {
        throw new NotImplementedException();
    }
}
