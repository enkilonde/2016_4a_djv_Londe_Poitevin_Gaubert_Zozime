using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /**
        Structure de la piste
    **/
    public GameObject[,] tiles = new GameObject[500, 500];


    /**
        Nombre de tours effectués par les joueurs
        et nombre de tours total
    **/
    private int trackLapsCount;
    private int playerLapsCount;
    private int AILapsCount;

    private int maxBananasCount;

    void Awake()
    {
        GameObject[] tilesTemp = GameObject.FindGameObjectsWithTag("Tiles");

        for (int i = 0; i < tilesTemp.Length; i++)
        {
            tiles[(int)tilesTemp[i].transform.position.x / 10, (int)tilesTemp[i].transform.position.z / 10] = tilesTemp[i];
            Debug.Log("Tile (" + (int)tilesTemp[i].transform.position.x / 10 + ", " + (int)tilesTemp[i].transform.position.z / 10 + ") registered");
        }
    }

    public Transform[] checkpoints;
    private int playerCheckpointCount = 1;
    private int AICheckpointCount = 1;

    public void PassCheckpoint(Transform checkpointTransform, bool isIA = false)
    {
        if (isIA)
        {
            if (checkpoints[AICheckpointCount] == checkpointTransform)
            {
                AICheckpointCount++;
                if (AICheckpointCount >= checkpoints.Length)
                {
                    AICheckpointCount = 1;
                    AILapsCount++;
                    Debug.Log("AI lap " + AILapsCount);
                }
            }
        }
        else
        {
            if (checkpoints[playerCheckpointCount] == checkpointTransform)
            {
                Debug.Log("Player reach checkpoint " + playerCheckpointCount);
                playerCheckpointCount++;
                if (playerCheckpointCount >= checkpoints.Length)
                {
                    playerCheckpointCount = 1;
                    playerLapsCount++;
                    Debug.Log("Player lap " + playerLapsCount);
                }
            }
        }
    }
}
