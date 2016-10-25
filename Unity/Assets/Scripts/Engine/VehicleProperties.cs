using UnityEngine;

public struct VehicleProperties
{
    private float deltaTime
    {
        get { return AStar.FIXED_STEP; }
    }

    public Transform transform;


    public Vector3 position;
    public float orientation;
    public float currentSpeed;
    public float maxSpeed;
    public float acceleration;
    public float rotationSpeed;
    public float dragFactor;
    public int nextWaypointIndex;
    public VehicleAction action;

    public void UpdateVehicle(VehicleAction action, float roadFactor)
    {
        float positionIncrement = 0.0f;
        float orientationIncrement = 0.0f;
        float speedIncrement = 0.0f;

        switch (action)
        {
            case VehicleAction.ACCELERATE:
                speedIncrement = acceleration * deltaTime;
                break;

            case VehicleAction.ACCELERATE_LEFT:
                speedIncrement = acceleration * deltaTime;
                orientationIncrement = rotationSpeed * deltaTime;
                break;

            case VehicleAction.ACCELERATE_RIGHT:
                speedIncrement = acceleration * deltaTime;
                orientationIncrement = -rotationSpeed * deltaTime;
                break;

            case VehicleAction.BRAKE:
                speedIncrement = -acceleration * deltaTime;
                break;

            case VehicleAction.BRAKE_LEFT:
                speedIncrement = -acceleration * deltaTime;
                orientationIncrement = -rotationSpeed * deltaTime;
                break;

            case VehicleAction.BRAKE_RIGHT:
                speedIncrement = -acceleration * deltaTime;
                orientationIncrement = rotationSpeed * deltaTime;
                break;

            case VehicleAction.LEFT:
                orientationIncrement = rotationSpeed * deltaTime;
                break;

            case VehicleAction.RIGHT:
                orientationIncrement = -rotationSpeed * deltaTime;
                break;

            case VehicleAction.USE_ITEM:
                speedIncrement = -dragFactor * deltaTime;
                break;

            case VehicleAction.NO_INPUT:
                speedIncrement = -dragFactor * deltaTime;
                break;
        }

        currentSpeed = Mathf.Clamp(currentSpeed + speedIncrement, -maxSpeed / 2.0f, maxSpeed);
        orientation = orientation + orientationIncrement;
        transform.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
        transform.position += transform.forward * currentSpeed;
    }
}

