using UnityEngine;
using System.Collections;

public enum GroundType { Road, Grass, Wall}

public class GameStateManager : MonoBehaviour
{
    //Public refs
    public GameState gameState;
    public Transform player;
    public Transform speedIndicator;

    //Private refs
    LevelManager levelManagerScript;
    PauseManager pauseManagerScript;
    Transform[] allWalls;

    //internal
    [HideInInspector] public float playerCurrentSpeed;

    [Header("Vehicle Properties")]
    public float rotationSpeed = 90;
    public float maxSpeed = 15;
    public float accelerationTime = 2;
    public float grassSlowFactor = 5;
    public float grassMaxSpeed = 0.2f;
    public float brakePower = 0.5f;




    void Awake()
    {
        levelManagerScript = FindObjectOfType<LevelManager>();
        pauseManagerScript = FindObjectOfType<PauseManager>();

        GameObject[] WallsTemp = GameObject.FindGameObjectsWithTag("Wall");
        allWalls = new Transform[WallsTemp.Length];
        for (int i = 0; i < WallsTemp.Length; i++)
        {
            allWalls[i] = WallsTemp[i].transform;
        }

        InitGameState();

    }

    void InitGameState()
    {
        gameState.player.rotationSpeed = rotationSpeed;
        gameState.player.maxSpeed = maxSpeed;
        gameState.player.accelerationTime = accelerationTime;
        gameState.player.grassSlowFactor = grassSlowFactor;
        gameState.player.grassMaxSpeed = grassMaxSpeed;
        gameState.player.brakePower = brakePower;

        UpdateGameState();
    }


    void Update()
    {
        if (pauseManagerScript.paused) return;
        ApplyNextState();
        ApplyPhysics();


    }

    void UpdateGameState()
    {
        gameState.player.position = player.position;
        gameState.player.orientation = player.rotation.eulerAngles.y;
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

        CustomTransform _t = gameState.player.UpdateVehicle(inputsSum, isEntityInGrass(gameState.player.position));

        player.transform.position = _t.position;
        player.transform.rotation = _t.rotation;

        playerCurrentSpeed = gameState.player.speedAcceleration; // get the speed of the player


        if (isEntityOnCheckpoint(gameState.player.position))
        {
            bool insideRoad;
            levelManagerScript.PassCheckpoint(GetEntityTile(gameState.player.position, out insideRoad));
        }

        UpdateSpeedIndicator();

    }

    public GameState ComputeGameState(GameState state, VehicleAction action)
    {
        state.player.UpdateVehicle(state.player.action, isEntityInGrass(state.player.position));
        state.player.position = CollisionScript.CollisionManage(allWalls, state.player.position, 0.75f,
            state.player.currentSpeed * (Quaternion.AngleAxis(state.player.orientation, Vector3.up) * Vector3.forward));

        state.AI.UpdateVehicle(action, isEntityInGrass(state.AI.position));
        state.AI.position = CollisionScript.CollisionManage(allWalls, state.AI.position, 0.75f,
            state.AI.currentSpeed * (Quaternion.AngleAxis(state.AI.orientation, Vector3.up) * Vector3.forward));

        return state;
    }

    void ApplyPhysics()
    {
        Vector3 collResult = CollisionScript.CollisionManage(allWalls, player.position, 0.75f, player.forward * playerCurrentSpeed);
        if (collResult != Vector3.zero) player.position = collResult;
        UpdateGameState();
    }



    void UpdateSpeedIndicator()
    {
        speedIndicator.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(42, -220, playerCurrentSpeed));
    }

    void ForSee()
    {



    }



    GroundType isEntityInGrass(Vector3 entityPos)
    {
        bool insideRoad;
        Transform tile = GetEntityTile(entityPos, out insideRoad);
        if (!insideRoad) return GroundType.Grass;
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
                return GroundType.Road;
        }
    }

    bool isEntityOnCheckpoint(Vector3 entityPos)
    {
        bool insideRoad;
        Transform tile = GetEntityTile(entityPos, out insideRoad);

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

    Transform GetEntityTile(Vector3 entityPos, out bool insideRoad)
    {
        try
        {
            insideRoad = true;
            return levelManagerScript.tiles[(int)entityPos.x / 10, (int)entityPos.z / 10].transform;
        }
        catch
        {
            insideRoad = false;
            for (int i = 0; i < levelManagerScript.tiles.GetLength(0); i++)
            {
                for (int j = 0; j < levelManagerScript.tiles.GetLength(1); j++)
                {
                    if (levelManagerScript.tiles[i, j]) return levelManagerScript.tiles[i, j].transform;
                }
            }
        }
        insideRoad = false;
        return null;
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

    GroundType GetCheckpointTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 4.5f || relativpos.y < -4.5f) return GroundType.Wall;
        }
        else
        {
            if (relativpos.x > 4.5f || relativpos.x < -4.5f) return GroundType.Wall;
        }

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 2.5f || relativpos.y < -2.5f) return GroundType.Grass;
        }
        else
        {
            if (relativpos.x > 2.5f || relativpos.x < -2.5f) return GroundType.Grass;
        }

        return GroundType.Road;
    }

    GroundType GetCurvedGrass(Transform tile, Vector3 playerpos)
    {
        Vector3 pivot = tile.GetChild(0).position;
        float dist = (playerpos.x - pivot.x) * (playerpos.x - pivot.x) + (playerpos.z - pivot.z) * (playerpos.z - pivot.z);

        if (dist < 0.5f * 0.5f || dist > 9.5f * 9.5f) return GroundType.Wall;


        if (dist < 2.5f * 2.5f || dist > 7.5f * 7.5f) return GroundType.Grass;
        return GroundType.Road;
    }

    GroundType GetStraightTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 4.5f || relativpos.y < -4.5f) return GroundType.Wall;
        }
        else
        {
            if (relativpos.x > 4.5f || relativpos.x < -4.5f) return GroundType.Wall;
        }

        if (Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1)
        {
            if (relativpos.y > 2.5f || relativpos.y < -2.5f) return GroundType.Grass;
        }
        else
        {
            if (relativpos.x > 2.5f || relativpos.x < -2.5f) return GroundType.Grass;
        }

        return GroundType.Road;
    }
}
