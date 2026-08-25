using Godot;

public static class Library
{
    public static Vector2 Drag(this Vector2 value, float Coeff, double delta)
    {
        value.X -= value.X * Coeff;
        return value;
    }

    //Slides against a normal concerving original length.
    public static Vector2 Ramp(this Vector2 value, Vector2 normal)
    {
        return value.Slide(normal).Normalized() * value.Length();
    }
}

