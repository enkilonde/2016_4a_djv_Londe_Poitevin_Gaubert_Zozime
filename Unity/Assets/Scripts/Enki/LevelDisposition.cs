using UnityEngine;
using System.Collections;

public enum TileType { SRAIGHT, CURVED, CHECKPOINT};

public class LevelDisposition : MonoBehaviour
{

    public GameObject[,] tiles = new GameObject[500, 500];
    public Transform[] checkpoints;




	// Use this for initialization
	void Awake ()
    {

        GameObject[] tilesTemp = GameObject.FindGameObjectsWithTag("Tiles");


        for (int i = 0; i < tilesTemp.Length; i++)
        {
            tiles[(int)tilesTemp[i].transform.position.x / 10, (int)tilesTemp[i].transform.position.z / 10] = tilesTemp[i];
            Debug.Log("Tile (" + (int)tilesTemp[i].transform.position.x / 10 + ", " + (int)tilesTemp[i].transform.position.z / 10 + ") registered");
        }


	}
	

}
