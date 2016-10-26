using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{

    public GameState gameState;
    public Transform player;
    public Transform AI;

    public float rotationSpeed = 90;
    public float maxSpeed = 15;
    public float accelerationTime = 2;
    public float grassSlowFactor = 5;
    public float grassMaxSpeed = 0.2f;
    public Transform[] goalTransforms;

    LevelManager levelManagerScript;

    PauseManager pauseManagerScript;

    void Awake()
    {
        levelManagerScript = FindObjectOfType<LevelManager>();
        pauseManagerScript = FindObjectOfType<PauseManager>();

        InitGameState();

    }

    void InitGameState()
    {
        initVehicle(ref gameState.player);
        initVehicle(ref gameState.AI);

        UpdateGameState();
    }

    void initVehicle(ref VehicleProperties vehicle)
    {
        vehicle.rotationSpeed = rotationSpeed;
        vehicle.maxSpeed = maxSpeed;
        vehicle.accelerationTime = accelerationTime;
        vehicle.grassSlowFactor = grassSlowFactor;
        vehicle.grassMaxSpeed = grassMaxSpeed;
        vehicle.grassDecelerate = 1;
    }


    void Update()
    {
        if (pauseManagerScript.paused) return;

        UpdateGameState();
        ApplyNextState();
    }

    void UpdateGameState()
    {
        gameState.player.position = player.position;
        gameState.player.orientation = player.rotation.eulerAngles.y;

        gameState.AI.position = AI.position;
        gameState.AI.orientation = AI.rotation.eulerAngles.y;
    }

    void ApplyNextState()
    {

        VehicleAction inputsSum = VehicleAction.NO_INPUT;

        if(Input.GetAxisRaw("Horizontal") < 0)
        inputsSum = inputsSum | VehicleAction.RIGHT;

        if (Input.GetAxisRaw("Horizontal") > 0)
            inputsSum = inputsSum | VehicleAction.LEFT;

        if (Input.GetAxisRaw("Vertical") > 0)
            inputsSum = inputsSum | VehicleAction.ACCELERATE;

        if (Input.GetAxisRaw("Vertical") < 0)
            inputsSum = inputsSum | VehicleAction.BRAKE;

        CustomTransform playerTransform = gameState.player.UpdateVehicle(inputsSum, isEntityInGrass(gameState.player.position));
        player.transform.position = playerTransform.position;
        player.transform.rotation = playerTransform.rotation;

        VehicleAction AIInputs = VehicleAction.RIGHT | VehicleAction.ACCELERATE;
        CustomTransform AITransform = gameState.AI.UpdateVehicle(AIInputs, isEntityInGrass(gameState.AI.position));
        AI.transform.position = AITransform.position;
        AI.transform.rotation = AITransform.rotation;

        if (isEntityOnCheckpoint(gameState.player.position))
        {
            levelManagerScript.PassCheckpoint(GetEntityTile(gameState.player.position));
        }

        if (isEntityOnCheckpoint(gameState.AI.position))
        {
            levelManagerScript.PassCheckpoint(GetEntityTile(gameState.AI.position));
        }
    }



    void ForSee()
    {
//        ForecastEngine.FindBestActions(gameState, )


    }



    bool isEntityInGrass(Vector3 entityPos)
    {

        Transform tile = GetEntityTile(entityPos);

        Vector2 relativePos = GetPosInTile(entityPos, tile.position);

        switch (tile.name)
        {
            case "roadtile_checkpoint":
                return GetCheckpointTileGrass(tile, relativePos);

            case "roadtile_curve":
                return GetCurvedGrass(tile, entityPos);

            case "roadtile_straight":
                return GetStraightTileGrass(tile, relativePos);

            default:
                break;
        }


        return false;
    }

    bool isEntityOnCheckpoint(Vector3 entityPos)
    {
        Transform tile = GetEntityTile(entityPos);

        Vector2 relativePos = GetPosInTile(entityPos, tile.position);

        switch (tile.name)
        {
            case "roadtile_checkpoint":
                return GetCheckpointTileCheckpoint(tile, relativePos);

            default:
                break;
        }
        return false;
    }

    Transform GetEntityTile(Vector3 entityPos)
    {
        //Debug.Log("Player pos : (" + (int)entityPos.x / 10 + ", " + (int)entityPos.z / 10 + ")");
        return levelManagerScript.tiles[(int)entityPos.x / 10, (int)entityPos.z / 10].transform;
    }

    Vector2 GetPosInTile(Vector3 playerpos, Vector3 Tilepos)
    {
        return new Vector2(playerpos.x - Tilepos.x, playerpos.z - Tilepos.z);
    }

    bool GetCheckpointTileCheckpoint(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.x > -0.5f && relativpos.x < 0.5f) return true;
        }
        else
        {
            if (relativpos.y > -0.5f && relativpos.y < 0.5f) return true;
        }

        return false;
    }

    bool GetCheckpointTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 2.5f || relativpos.y < -2.5f) return true;
        }
        else
        {
            if (relativpos.x > 2.5f || relativpos.x < -2.5f) return true;
        }

        return false;
    }

    bool GetCurvedGrass(Transform tile, Vector3 playerpos)
    {
        Vector3 pivot = tile.GetChild(0).position;
        float dist = (playerpos.x - pivot.x) * (playerpos.x - pivot.x) + (playerpos.z - pivot.z) * (playerpos.z - pivot.z);
        if (dist < 2.5f * 2.5f || dist > 7.5f * 7.5f) return true;
        return false;
    }

    bool GetStraightTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 2.5f || relativpos.y < -2.5f) return true;
        }
        else
        {
            if (relativpos.x > 2.5f || relativpos.x < -2.5f) return true;
        }

        return false;
    }
}
