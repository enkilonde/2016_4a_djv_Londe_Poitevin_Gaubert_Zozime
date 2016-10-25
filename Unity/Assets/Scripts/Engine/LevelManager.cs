using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /**
        Structure de la piste
    **/
    private Tile[] trackTiles;
    private int trackWidth;
    private int trackHeight;

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




}
