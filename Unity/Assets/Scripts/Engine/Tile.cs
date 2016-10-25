using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform transform;
    public GameObject boostPlate;
    public Waypoint waypoint;


    public bool hasWaypoint()
    {
        return waypoint != null;
    }

    public bool hasBoostPlate()
    {
        return boostPlate != null;
    }
}
