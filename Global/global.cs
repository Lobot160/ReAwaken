using Godot;
using System;


public static class Global
{
    public const float AIR_RESISTANCE = 0.5f;
    public const float DRAG = 2;
    public const float DRAG_LNR = 30;
    public const float FRICTION = 7;

    public static readonly float GRAVITY = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
}