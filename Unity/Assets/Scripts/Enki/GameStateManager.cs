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


    }

    void InitGameState()
    {
        gameState.player.rotationSpeed = rotationSpeed;
        gameState.player.maxSpeed = maxSpeed;

        UpdateGameState();
    }

    void UpdateGameState()
    {
        gameState.player.position = player.position;
        gameState.player.orientation = player.rotation.eulerAngles.y;


    }

    void ApplyNextState()
    {

    }



    void Forsee()
    {



    }




}
