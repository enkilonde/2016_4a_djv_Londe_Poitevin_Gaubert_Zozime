using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /**
        Structure de la piste
    **/
    private Tile[] trackTiles;
    private int trackWidth;
    private int trackHeight;
    public GameObject[,] tiles = new GameObject[500, 500];

    public VehicleProperties player;


    /**
        Nombre de tours effectués par les joueurs
        et nombre de tours total
    **/
    private int trackLapsCount;
    private int playerLapsCount;
    private int AILapsCount;

    private int maxBananasCount;

    public Tile getTiles(int x, int y)
    {
        return trackTiles[y * trackWidth + x];
    }

    void Awake()
    {
        GameObject[] tilesTemp = GameObject.FindGameObjectsWithTag("Tiles");

        for (int i = 0; i < tilesTemp.Length; i++)
        {
            tiles[(int)tilesTemp[i].transform.position.x / 10, (int)tilesTemp[i].transform.position.z / 10] = tilesTemp[i];
            Debug.Log("Tile (" + (int)tilesTemp[i].transform.position.x / 10 + ", " + (int)tilesTemp[i].transform.position.z / 10 + ") registered");
        }
    }





}
