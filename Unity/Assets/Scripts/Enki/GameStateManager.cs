using UnityEngine;
using System.Collections;

public enum GroundType { Road, Grass, Wall }

public class GameStateManager : MonoBehaviour
{
    //Public refs
    public GameState gameState;
    public Transform player;
    public Transform AI;
    public Transform speedIndicator;

    Vector3 destination;

    //Private refs
    LevelManager levelManagerScript;
    Vector3[] myWaypoints;
    int currentPositionInLevel;

    PauseManager pauseManagerScript;
    ForecastEngine forecastEngine;
    Transform[] allWalls;

    //internal
    public float playerCurrentSpeed
    {
        get { return gameState.player.speedAcceleration; }
    }
    public float AICurrentSpeed
    {
        get { return gameState.AI.speedAcceleration; }
    }

    [Header("Vehicle Properties")]
    public float rotationSpeed = 90;
    public float maxSpeed = 15;
    public float accelerationTime = 2;
    public float grassSlowFactor = 5;
    public float grassMaxSpeed = 0.2f;
    public float brakePower = 0.5f;


    void Awake()
    {
        levelManagerScript = GetComponent<LevelManager>();
        pauseManagerScript = GetComponent<PauseManager>();
        forecastEngine = GetComponent<ForecastEngine>();
    }

    void Start()
    {
        GameObject[] WallsTemp = GameObject.FindGameObjectsWithTag("Wall");
        allWalls = new Transform[WallsTemp.Length];
        for (int i = 0; i < WallsTemp.Length; i++)
        {
            allWalls[i] = WallsTemp[i].transform;
        }

        InitGameState();

        myWaypoints = new Vector3[levelManagerScript.checkpoints.Length * levelManagerScript.trackLapsCount];
        for (int i = 0; i < levelManagerScript.trackLapsCount; i++)
        {
            for (int j = 0; j < levelManagerScript.checkpoints.Length; j++)
            {
                myWaypoints[i * levelManagerScript.checkpoints.Length + j] = levelManagerScript.checkpoints[j].position;
            }
        }

        currentPositionInLevel = 0;
        destination = myWaypoints[0];
        GameState goalState = new GameState();
        goalState.AI.position = destination;
        forecastEngine.SetGoalState(goalState);
    }

    void InitGameState()
    {
        VehicleStaticProperties.Reset(rotationSpeed, maxSpeed, accelerationTime, grassSlowFactor, grassMaxSpeed, brakePower);
        UpdateGameState();
    }


    void FixedUpdate()
    {
        if (pauseManagerScript.paused) return;
        ApplyPlayerNextState();
        ApplyIANextState();

        ApplyPhysics(player);
        //ApplyPhysics(AI);

        //Debug.Log(gameState.player.ground);
        //gameState.AI.position = Vector3.zero + new Vector3(1, 1, 1);


        // actualize destination
        UpdateDestination();
        GameState goalState = new GameState();
        goalState.AI.position = destination;
        forecastEngine.SetGoalState(goalState);
    }

    void UpdateDestination ()
    {
        if (currentPositionInLevel + 1 < myWaypoints.Length)
        {
            if (Vector3.SqrMagnitude(gameState.AI.position - myWaypoints[currentPositionInLevel]) < 100f)
            {
                currentPositionInLevel++;
                destination = myWaypoints[currentPositionInLevel];
            }
        }
    }

    void UpdateGameState()
    {
        gameState.player.position = player.position;
        gameState.player.orientation = player.rotation.eulerAngles.y;
        //gameState.player.ground = isEntityInGrass(player.position);

        gameState.AI.position = AI.position;
        gameState.AI.orientation = AI.rotation.eulerAngles.y;
        //gameState.player.ground = isEntityInGrass(player.position);
    }

    void ApplyPlayerNextState()
    {

        VehicleAction inputsSum = VehicleAction.NO_INPUT;

        if (Input.GetAxisRaw("Horizontal") > 0)
            inputsSum = inputsSum | VehicleAction.RIGHT;

        if (Input.GetAxisRaw("Horizontal") < 0)
            inputsSum = inputsSum | VehicleAction.LEFT;

        if (Input.GetAxisRaw("Vertical") > 0)
            inputsSum = inputsSum | VehicleAction.ACCELERATE;

        if (Input.GetAxisRaw("Vertical") < 0 || Input.GetKey(KeyCode.Space))
            inputsSum = inputsSum | VehicleAction.BRAKE;

        gameState.player.action = inputsSum;

        gameState.player = VehicleStaticProperties.UpdateVehicle(inputsSum, gameState.player);
        player.transform.position = gameState.player.position;
        player.transform.rotation = Quaternion.AngleAxis(gameState.player.orientation, Vector3.up);


        if (isEntityOnCheckpoint(gameState.player.position))
        {
            levelManagerScript.PassCheckpoint(GetEntityTile(gameState.player.position));
        }

        UpdateSpeedIndicator();

    }

    void ApplyIANextState()
    {
        VehicleAction inputsSum = forecastEngine.getBestAction(gameState);


        gameState.AI = VehicleStaticProperties.UpdateVehicle(inputsSum, gameState.AI);
        AI.transform.position = gameState.AI.position;
        AI.transform.rotation = Quaternion.AngleAxis(gameState.AI.orientation, Vector3.up);



        if (isEntityOnCheckpoint(gameState.AI.position))
        {
            levelManagerScript.PassCheckpoint(GetEntityTile(gameState.AI.position), true);
        }
    }

    public GameState ComputeGameState(GameState state, VehicleAction action)
    {
        state.AI = VehicleStaticProperties.UpdateVehicle(action, state.AI);

        return state;
    }
    public GameState ComputeGameState(GameState state, VehicleAction action, int steps)
    {
        state.AI = VehicleStaticProperties.UpdateVehicle(action, state.AI, steps);

        return state;
    }

    void ApplyPhysics(Transform entity)
    {
        Vector3 collResult = CollisionScript.CollisionManage(allWalls, entity.position, 0.75f, entity.forward * playerCurrentSpeed);
        if (entity.position != collResult)
        {
            // collision 
            entity.position = collResult;
        }
        UpdateGameState();
    }




    void UpdateSpeedIndicator()
    {
        speedIndicator.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(42, -220, playerCurrentSpeed));
    }

    static public GroundType isEntityInGrass(Vector3 entityPos)
    {
        Transform tile = GetEntityTile(entityPos);
        if (tile == null)
        {
            return GroundType.Wall;
        }

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
        Transform tile = GetEntityTile(entityPos);

        // Not a tile ? So we're not in a checkpoint
        if (tile == null)
            return false;

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

    static Transform GetEntityTile(Vector3 entityPos)
    {
        

        // We get our game object
        GameObject go = LevelManager.GetTileAt((int)entityPos.x / 10, (int)entityPos.z / 10);

        // Isn't null ? We return the transform
        if (go != null)
            return go.transform;

        // Otherwise we return null
        return null;
    }

    static Vector2 GetPosInTile(Vector3 playerpos, Vector3 Tilepos)
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

    static GroundType GetCheckpointTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;


        if ((Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.y > 4.5f || relativpos.y < -4.5f)) return GroundType.Wall;
        else if (!(Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.x > 4.5f || relativpos.x < -4.5f)) return GroundType.Wall;


        if ((Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.y > 2.5f || relativpos.y < -2.5f)) return GroundType.Grass;
        else if (!(Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.x > 2.5f || relativpos.x < -2.5f)) return GroundType.Grass;

        return GroundType.Road;
    }

    static GroundType GetCurvedGrass(Transform tile, Vector3 playerpos)
    {
        Vector3 pivot = tile.GetChild(0).position;
        float dist = (playerpos.x - pivot.x) * (playerpos.x - pivot.x) + (playerpos.z - pivot.z) * (playerpos.z - pivot.z);


        if (dist < 0.5f * 0.5f || dist > 9.5f * 9.5f) return GroundType.Wall;


        if (dist < 2.5f * 2.5f || dist > 7.5f * 7.5f) return GroundType.Grass;

        return GroundType.Road;
    }

    static GroundType GetStraightTileGrass(Transform tile, Vector2 relativpos)
    {
        float rotationY = tile.rotation.eulerAngles.y;


        if ((Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.y > 4.5f || relativpos.y < -4.5f)) return GroundType.Wall;
        else if (!(Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) &&(relativpos.x > 4.5f || relativpos.x < -4.5f)) return GroundType.Wall;


        if ((Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.y > 2.5f || relativpos.y < -2.5f)) return GroundType.Grass;
        else if(!(Mathf.Abs(rotationY - 90) < 1 || Mathf.Abs(rotationY - 270) < 1) && (relativpos.x > 2.5f || relativpos.x < -2.5f)) return GroundType.Grass;

        return GroundType.Road;
    }
}