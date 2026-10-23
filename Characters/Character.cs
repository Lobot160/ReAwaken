using Godot;

public abstract partial class Character : CharacterBody2D
{
    public float speed, acceleration, jumpPower;

    public bool Grounded { get => MotionMode == MotionModeEnum.Grounded; }

    public float TotalFriction { get => GetFloorFriction() * GetDrag(); }
    
    public virtual bool CanJump { get => IsOnFloor(); }

    private bool Moving;
    private bool Stick;

    private float GetDrag()
    {
        return Global.DRAG;
    }
    private float GetFloorFriction()
    {
        return 0.5f;
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyGravity(delta);
        ApplyDrag(delta);
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        if (MoveAndSlide())
        {
            var collision = GetLastSlideCollision();
            PhysicsMaterial material = ((StaticBody2D)collision.GetCollider())?.GetPhysicsMaterialOverride() ;

            Velocity += collision.GetColliderVelocity();
            Velocity = collision.GetRemainder().Reflect(collision.GetNormal()) * material.Bounce;
        }
    }

    public virtual void ApplyJump()
    {
        Velocity += new Vector2(0, Velocity.Y + jumpPower);
    }
    public void ApplyMovement(Vector2 direction, double delta)
    {
        Velocity += direction * acceleration * (float)delta * TotalFriction;
    }

    private void ApplyGravity(double delta)
    {
        if(IsOnFloor() && Grounded) Velocity += GetGravity() * (float)delta;
    }

    private void ApplyDrag(double delta)
    {
        Velocity = Velocity.Drag(TotalFriction, delta);
    }
}