using UnityEngine;

public enum TileType { SRAIGHT, CURVED, CHECKPOINT, TEE, CROSS };

public class Tile : MonoBehaviour
{
    //public GameObject boostPlate;
    public Transform transform;
    public TileType tileType;
    public int rotation;

    

    /*
    public bool hasBoostPlate()
    {
        return boostPlate != null;
    }
    */
}
