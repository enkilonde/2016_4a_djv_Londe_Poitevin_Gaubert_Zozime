using System;

[Flags]
public enum VehicleAction
{
    NO_INPUT = 0,
    ACCELERATE = 1,
    LEFT = 2,
    RIGHT = 4,
    BRAKE = 16,
    USE_ITEM = 32
}
