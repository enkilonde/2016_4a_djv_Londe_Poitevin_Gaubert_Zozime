using UnityEngine;
using System.Collections;

public class VehicleStaticProperties
{

    //Valeurs à initialiser
    public static float rotationSpeed;
    public static float maxSpeed;
    public static float accelerationTime;
    public static float grassSlowFactor;
    public static float grassMaxSpeed;
    public static float brakePower;

    

    

    public static void Reset(float _rotationSpeed, float _maxSpeed, float _accelerationTime, float _grassSlowFactor, float _grassMaxSpeed, float _brakePower)
    {
        rotationSpeed = _rotationSpeed;
        maxSpeed = _maxSpeed;
        accelerationTime = _accelerationTime;
        grassSlowFactor = _grassSlowFactor;
        grassMaxSpeed = _grassMaxSpeed;
        brakePower = _brakePower;
}

    public static VehicleProperties UpdateVehicle(VehicleAction action, VehicleProperties vehicleProps)
    {
        vehicleProps.action = action;

        float deltaTime = Time.fixedDeltaTime;

        float orientationIncrement = 0.0f;
        float speedIncrement = 0.0f;

        if (vehicleProps.ground == GroundType.Grass || vehicleProps.ground == GroundType.Wall)
        {
            if (vehicleProps.speedAcceleration > grassMaxSpeed)
                vehicleProps.speedAcceleration = Mathf.Clamp(vehicleProps.speedAcceleration - deltaTime * grassSlowFactor, grassMaxSpeed, 1);
        }

        if ((action & VehicleAction.ACCELERATE) == VehicleAction.ACCELERATE)
        {
            vehicleProps.speedAcceleration += deltaTime / accelerationTime;
        }
        else
        {
            vehicleProps.speedAcceleration -= deltaTime / accelerationTime;
        }

        if ((action & VehicleAction.BRAKE) == VehicleAction.BRAKE)
        {
            vehicleProps.speedAcceleration -= deltaTime / (accelerationTime / brakePower);
        }
        vehicleProps.speedAcceleration = Mathf.Clamp01(vehicleProps.speedAcceleration);

        if ((action & VehicleAction.LEFT) == VehicleAction.LEFT)
        {
            orientationIncrement = -rotationSpeed * deltaTime;
            vehicleProps.speedAcceleration -= deltaTime / accelerationTime / 2;
        }

        if ((action & VehicleAction.RIGHT) == VehicleAction.RIGHT)
        {
            orientationIncrement = rotationSpeed * deltaTime;
            vehicleProps.speedAcceleration -= deltaTime / accelerationTime / 2;
        }

        speedIncrement = maxSpeed * deltaTime * vehicleProps.speedAcceleration;

        vehicleProps.orientation = vehicleProps.orientation + orientationIncrement;

        vehicleProps.position += speedIncrement * (Quaternion.AngleAxis(vehicleProps.orientation, Vector3.up) * Vector3.forward);

        vehicleProps.ground = GameStateManager.isEntityInGrass(vehicleProps.position);

        return vehicleProps;
    }
    public static VehicleProperties UpdateVehicle(VehicleAction action, VehicleProperties vehicleProps, int steps)
    {
        vehicleProps.action = action;

        float deltaTime = Time.fixedDeltaTime * steps;

        float orientationIncrement = 0.0f;
        float speedIncrement = 0.0f;

        if (vehicleProps.ground == GroundType.Grass || vehicleProps.ground == GroundType.Wall)
        {
            if (vehicleProps.speedAcceleration > grassMaxSpeed)
                vehicleProps.speedAcceleration = Mathf.Clamp(vehicleProps.speedAcceleration - deltaTime * grassSlowFactor, grassMaxSpeed, 1);
        }

        if ((action & VehicleAction.ACCELERATE) == VehicleAction.ACCELERATE)
        {
            vehicleProps.speedAcceleration += deltaTime / accelerationTime;
        }
        else
        {
            vehicleProps.speedAcceleration -= deltaTime / accelerationTime;
        }

        if ((action & VehicleAction.BRAKE) == VehicleAction.BRAKE)
        {
            vehicleProps.speedAcceleration -= deltaTime / (accelerationTime / brakePower);
        }
        vehicleProps.speedAcceleration = Mathf.Clamp01(vehicleProps.speedAcceleration);

        if ((action & VehicleAction.LEFT) == VehicleAction.LEFT)
        {
            orientationIncrement = -rotationSpeed * deltaTime;
            vehicleProps.speedAcceleration -= deltaTime / accelerationTime / 2;
        }

        if ((action & VehicleAction.RIGHT) == VehicleAction.RIGHT)
        {
            orientationIncrement = rotationSpeed * deltaTime;
            vehicleProps.speedAcceleration -= deltaTime / accelerationTime / 2;
        }

        speedIncrement = maxSpeed * deltaTime * vehicleProps.speedAcceleration;

        vehicleProps.orientation = vehicleProps.orientation + orientationIncrement;

        vehicleProps.position += speedIncrement * (Quaternion.AngleAxis(vehicleProps.orientation, Vector3.up) * Vector3.forward);

        vehicleProps.ground = GameStateManager.isEntityInGrass(vehicleProps.position);

        return vehicleProps;
    }


    public static float DiffBetweenVehicleProperties(VehicleProperties prop1, VehicleProperties prop2)
    {

        return (Vector3.Distance(prop1.position, prop2.position) * Mathf.Abs(prop2.orientation - prop1.orientation) * Mathf.Abs(prop1.speedAcceleration - prop2.speedAcceleration));
    }
}
