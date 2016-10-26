﻿using UnityEngine;

public struct VehicleProperties
{
    private float deltaTime
    {
        get { return AStar.FIXED_STEP; }
    }

    //Valeurs à initialiser
    public float rotationSpeed;
    public float maxSpeed;
    public float accelerationTime;
    public float grassSlowFactor;
    public float grassMaxSpeed;
    public float brakePower;


    //Valeurs à updater
    public Vector3 position;
    public float orientation;
    public VehicleAction action;
    public int nextWaypointIndex;


    //Variables internes
    public float speedAcceleration;


    public CustomTransform UpdateVehicle(VehicleAction action, GroundType groundType)
    {
        float orientationIncrement = 0.0f;
        float speedIncrement = 0.0f;

        if (groundType == GroundType.Grass || groundType == GroundType.Wall)
        {
            if(speedAcceleration > grassMaxSpeed)
            speedAcceleration = Mathf.Clamp(speedAcceleration - deltaTime * grassSlowFactor, grassMaxSpeed, 1);
        } 

        if ((action & VehicleAction.ACCELERATE) == VehicleAction.ACCELERATE) {
            speedAcceleration += deltaTime / accelerationTime;
        }
        else
        {
            speedAcceleration -= deltaTime / accelerationTime;
        }

        if ((action & VehicleAction.BRAKE) == VehicleAction.BRAKE)
        {
            speedAcceleration -= deltaTime / (accelerationTime / brakePower);
        }
        speedAcceleration = Mathf.Clamp01(speedAcceleration);
        speedIncrement = maxSpeed * deltaTime * speedAcceleration;

        if ((action & VehicleAction.LEFT) == VehicleAction.LEFT)
        {
            orientationIncrement = rotationSpeed * deltaTime;
        }

        if ((action & VehicleAction.RIGHT) == VehicleAction.RIGHT)
        {
            orientationIncrement = -rotationSpeed * deltaTime;
        }

        orientation = orientation + orientationIncrement;

        CustomTransform output;
        output.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
        output.position = position + speedIncrement * (output.rotation * Vector3.forward);

        position += speedIncrement * (output.rotation * Vector3.forward);

        return output;
    }
}

public struct CustomTransform
{
    public Vector3 position;
    public Quaternion rotation;
}

