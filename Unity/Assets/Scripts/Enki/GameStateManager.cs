using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{

    public GameState gameState;
    public Transform player;

    public float rotationSpeed = 90;
    public float maxSpeed = 15;

    LevelManager levelManagerScript;

    void Awake()
    {
        levelManagerScript = FindObjectOfType<LevelManager>();
        //gameState.player.transform = 
        InitGameState();

    }

    void InitGameState()
    {
        gameState.player.rotationSpeed = rotationSpeed;
        gameState.player.maxSpeed = maxSpeed;

        UpdateGameState();
    }


    void Update()
    {
        UpdateGameState();
        ApplyNextState();
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

        CustomTransform _t = gameState.player.UpdateVehicle(inputsSum, isEntityInGrass(gameState.player.position));
        player.transform.position = _t.position;
        player.transform.rotation = _t.rotation;

    }



    void ForSee()
    {



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
                return GetCurvedGrass(tile, relativePos);

            case "roadtile_straight":
                return GetStraightTileGrass(tile, relativePos);

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

    Vector2 GetPosInTile(Vector3 pos, Vector3 Tilepos)
    {
        return new Vector2(pos.x - Tilepos.x, pos.z - Tilepos.z);
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

    bool GetCurvedGrass(Transform tile, Vector2 relativpos)
    {
        Vector3 pivot = tile.GetChild(0).position;
        float dist = (transform.position.x - pivot.x) * (transform.position.x - pivot.x) + (transform.position.z - pivot.z) * (transform.position.z - pivot.z);
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
