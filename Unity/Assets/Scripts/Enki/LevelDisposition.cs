using UnityEngine;
using System.Collections;

public class LevelDisposition : MonoBehaviour
{

    public GameObject[,] tiles = new GameObject[500, 500];

	// Use this for initialization
	void Awake ()
    {

        GameObject[] tilesTemp = GameObject.FindGameObjectsWithTag("Tiles");

        for (int i = 0; i < tilesTemp.Length; i++)
        {
            tiles[(int)tilesTemp[i].transform.position.x, (int)tilesTemp[i].transform.position.y] = tilesTemp[i];
            Debug.Log("Tile (" + (int)tilesTemp[i].transform.position.x + ", " + (int)tilesTemp[i].transform.position.z + ") registered");
        }


	}
	
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {

        }

	}
}
