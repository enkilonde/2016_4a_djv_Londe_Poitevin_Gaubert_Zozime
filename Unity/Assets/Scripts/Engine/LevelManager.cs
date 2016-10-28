using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    /**
        Structure de la piste
    **/
    public static int levelWidth = 20;
    public static int levelHeight = 20;
    static private GameObject[] tiles;


    /**
        Nombre de tours effectués par les joueurs
        et nombre de tours total
    **/
    public int trackLapsCount = 3;

    public Transform[] checkpoints; /**
                                     * Don't count START checkpoint
                                     * End with FINISH checkpoint
                                     */
    private int playerCheckpointCount = 0;
    private int AICheckpointCount = 0;

    private int playerLapsCount { get { return (playerCheckpointCount / checkpoints.Length) + 1; } }
//    private int AILapsCount { get { return (AICheckpointCount / checkpoints.Length) + 1; } }

    private int maxBananasCount;

    public Transform RankingCanvas;
    public Text LapsUICounter;
    public Text CheckpointsUICounter;

    PauseManager pauseManager;

    void Awake()
    {
        pauseManager = GetComponent<PauseManager>();

        tiles = new GameObject[levelWidth * levelHeight];
        GameObject[] tilesTemp = GameObject.FindGameObjectsWithTag("Tiles");

        for (int i = 0; i < tilesTemp.Length; i++)
        {
            SetTileAt((int)tilesTemp[i].transform.position.x / 10, (int)tilesTemp[i].transform.position.z / 10, tilesTemp[i]);
            //Debug.Log("Tile (" + (int)tilesTemp[i].transform.position.x / 10 + ", " + (int)tilesTemp[i].transform.position.z / 10 + ") registered");
        }

        // Initialize UI
        LapsUICounter.text = "Laps : " + playerLapsCount + " / " + trackLapsCount;
        CheckpointsUICounter.text = "Checkpoints : " + playerCheckpointCount % checkpoints.Length + " / " + (checkpoints.Length - 1);
    }

    static public GameObject GetTileAt(int x, int y)
    {
        return tiles[y * levelHeight + x];
    }

    static public void SetTileAt(int x, int y, GameObject go)
    {
        tiles[y * levelHeight + x] = go;
    }


    public void PassCheckpoint(Transform checkpointTransform, bool isIA = false)
    {
        if (checkpointTransform == null)
        {
            Debug.LogError("Passcheckpoint received a null transform !");
            return;
        }

        if (isIA)
        {
            if (checkpoints[AICheckpointCount % checkpoints.Length] == checkpointTransform)
            {
                AICheckpointCount++;
                ActualiseRanking();

                if (AICheckpointCount >= trackLapsCount * checkpoints.Length)
                {
                    pauseManager.EndTrack(false);
                }
            }
        }
        else
        {
            if (checkpoints[playerCheckpointCount % checkpoints.Length] == checkpointTransform)
            {
                playerCheckpointCount++;
                LapsUICounter.text = "Laps : " + playerLapsCount + " / " + trackLapsCount;
                CheckpointsUICounter.text = "Checkpoints : " + playerCheckpointCount % checkpoints.Length + " / " + (checkpoints.Length - 1);
                ActualiseRanking();
                if (playerCheckpointCount >= trackLapsCount * checkpoints.Length)
                {
                    pauseManager.EndTrack(true);
                }
            }
        }
    }

    private void ActualiseRanking()
    {
        if (playerCheckpointCount > AICheckpointCount)
        {
            RankingCanvas.GetChild(0).gameObject.SetActive(true);
            RankingCanvas.GetChild(1).gameObject.SetActive(false);
        }
        else if (playerCheckpointCount < AICheckpointCount)
        {
            RankingCanvas.GetChild(0).gameObject.SetActive(false);
            RankingCanvas.GetChild(1).gameObject.SetActive(true);
        }
    }
}
