using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{

    public GameState gameState;
    public Transform player;

    public float rotationSpeed = 90;
    public float maxSpeed = 15;

    void Awake()
    {

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

        CustomTransform _t = gameState.player.UpdateVehicle(inputsSum, 1);
        player.transform.position = _t.position;
        player.transform.rotation = _t.rotation;

    }



    void Forsee()
    {



    }




}
