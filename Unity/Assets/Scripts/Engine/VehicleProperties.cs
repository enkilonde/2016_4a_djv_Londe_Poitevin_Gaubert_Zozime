using UnityEngine;

public struct VehicleProperties
{
    private float deltaTime
    {
        get { return AStar.FIXED_STEP; }
    }

    //Valeurs à initialiser
    public float rotationSpeed;
    public float maxSpeed;



    //Valeurs à updater
    public Vector3 position;
    public float orientation;
    public VehicleAction action;
    public int nextWaypointIndex;




    public CustomTransform UpdateVehicle(VehicleAction action, float roadFactor)
    {
        float orientationIncrement = 0.0f;
        float speedIncrement = 0.0f;

        if ((action & VehicleAction.ACCELERATE) == VehicleAction.ACCELERATE) {
            speedIncrement = maxSpeed * deltaTime;
        }

        if ((action & VehicleAction.LEFT) == VehicleAction.LEFT)
        {
            orientationIncrement = rotationSpeed * deltaTime;
        }

        if ((action & VehicleAction.RIGHT) == VehicleAction.RIGHT)
        {
            orientationIncrement = -rotationSpeed * deltaTime;
        }

        if ((action & VehicleAction.BRAKE) == VehicleAction.BRAKE)
        {
            // TODO : implementer freinage
        }

        orientation = orientation + orientationIncrement;

        CustomTransform output;
        output.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
        output.position = position + speedIncrement * (output.rotation * Vector3.forward); //experimental
        return output;
    }
}

public struct CustomTransform
{
    public Vector3 position;
    public Quaternion rotation;
}

