using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetIdealPath : MonoBehaviour
{
    struct Coordinates
    {
        public int x;
        public int y;

        public static bool areEqual(Coordinates a, Coordinates b)
        {
            if (a.x == b.x) { if (a.y == b.y) return true; }
            return false;
        }
    }

    public List<Vector3> pathPoints = new List<Vector3>();

    LevelManager levelManager;
    public Transform IAtransform;
    public int laps;

    void Start()
    {
        levelManager = GetComponent<LevelManager>();
        pathPoints = CalculatePath(IAtransform.position, levelManager.checkpoints);
    }

    List<Vector3> CalculatePath(Vector3 startPosition, Transform[] checkPoints)
    {
        List<Vector3> idealLap = new List<Vector3>(10 * checkPoints.Length);
        List<Vector3> idealPath = new List<Vector3>(10 * checkPoints.Length * laps);
        
        //Debug.Log("Compute start to 0"); // OK
        idealPath.AddRange(CalculatePathBetween2Position(startPosition, checkPoints[0].position));
        //Debug.Log("Compute 0 to 1"); // OK
        idealPath.AddRange(CalculatePathBetween2Position(checkPoints[0].position, checkPoints[1].position));
        //Debug.Log("Compute 1 to 2"); // OK
        idealPath.AddRange(CalculatePathBetween2Position(checkPoints[1].position, checkPoints[2].position));
        //Debug.Log("Compute 2 to finish"); // OK
        idealPath.AddRange(CalculatePathBetween2Position(checkPoints[2].position, checkPoints[3].position));

        /*idealPath.AddRange(CalculatePathBetween2Position(startPosition, checkPoints[0].position)); // start to first CP
        
        for (int i = 1; i < checkPoints.Length; i++)
        {
            idealLap.AddRange(CalculatePathBetween2Position(checkPoints[i-1].position, checkPoints[i].position));
        }
        
        idealPath.AddRange(idealLap); // start to 1 lap

        for (int i = 1; i < laps; i++)
        {
            idealPath.AddRange(CalculatePathBetween2Position(checkPoints[checkPoints.Length-1].position, checkPoints[0].position)); // start to 1 lap & 1 CP
            idealPath.AddRange(idealLap); // start to N lap
        }
        */
        return idealPath;
    }

    List<Vector3> CalculatePathBetween2Position(Vector3 startPosition, Vector3 targetPosition)
    {
        List<Vector3> startPath = new List<Vector3>(20);
        startPath.Add(startPosition);

        List<List<Vector3>> pathsList = new List<List<Vector3>>(10);
        pathsList.Add(startPath);

        List<Coordinates> closedCoordinates = new List<Coordinates>(30);

        Coordinates currentCoordinates;
        Coordinates targetCoordinates = pointToCoordinates(targetPosition);

        List<Coordinates> childrenCoordonates = new List<Coordinates>(4);

        while (pathsList.Count > 0)
        {
            // pathList[0] is always used as current path
            currentCoordinates = pointToCoordinates(pathsList[0][pathsList[0].Count - 1]);

            childrenCoordonates.Clear();
            childrenCoordonates = getPossibleChilds(currentCoordinates);

            foreach (Coordinates childCoord in childrenCoordonates)
            {
                if (Coordinates.areEqual(childCoord, targetCoordinates))
                {
                    //pathsList[0].Add(coordinatesToPointCentered(childCoord));
                    return pathsList[0];
                }

                bool isChildOK = true;
                foreach (Coordinates closedCoord in closedCoordinates)
                {
                    if (Coordinates.areEqual(childCoord, closedCoord)) isChildOK = false;
                    break;
                }

                if (isChildOK)
                {
                    List<Vector3> newPath = new List<Vector3>(pathsList[0]);
                    newPath.Add(coordinatesToPointCentered(childCoord));
                    pathsList.Add(newPath);
                }
            }
            pathsList.RemoveAt(0);

            closedCoordinates.Add(currentCoordinates);
        }

        return null;
    }

    List<Coordinates> getPossibleChilds(Coordinates parentCoord)
    {
        List<Coordinates> validChildsCoortinates = new List<Coordinates>(4);
        string parentTileName;
        float parentTileRotation;


        GameObject tile = LevelManager.GetTileAt(parentCoord.x, parentCoord.y);
        if (tile == null)
        {
            Debug.LogWarning("You try to find children, but the parent tile is unexistant ! Please put the AI on a tile.");
            return validChildsCoortinates;
        }

        parentTileName = tile.name;
        parentTileRotation = tile.transform.rotation.eulerAngles.y;
        
        if (isTheTileAtOffsetIsConnected(parentTileName, parentTileRotation, 1, 0))
        {
            Coordinates childCoord1;
            childCoord1.x = parentCoord.x + 1; childCoord1.y = parentCoord.y;
            validChildsCoortinates.Add(childCoord1);
        }
        if (isTheTileAtOffsetIsConnected(parentTileName, parentTileRotation, -1, 0))
        {
            Coordinates childCoord2;
            childCoord2.x = parentCoord.x - 1; childCoord2.y = parentCoord.y;
            validChildsCoortinates.Add(childCoord2);
        }
        if (isTheTileAtOffsetIsConnected(parentTileName, parentTileRotation, 0, 1))
        {
            Coordinates childCoord3;
            childCoord3.x = parentCoord.x; childCoord3.y = parentCoord.y + 1;
            validChildsCoortinates.Add(childCoord3);
        }
        if (isTheTileAtOffsetIsConnected(parentTileName, parentTileRotation, 0, -1))
        {
            Coordinates childCoord4;
            childCoord4.x = parentCoord.x; childCoord4.y = parentCoord.y - 1;
            validChildsCoortinates.Add(childCoord4);
        }

        return validChildsCoortinates;
    }

    bool isTheTileAtOffsetIsConnected(string parentTileName, float parentTileRotation, int xOffset, int yOffset)
    {
        switch (parentTileName)
        {
            case "roadtile_curve":
                if (Mathf.Abs(parentTileRotation) < 1 || (Mathf.Abs(Mathf.Abs(parentTileRotation)- 360) < 1))
                {
                    if (yOffset == 1 || xOffset == 1) return true;
                }
                else if (Mathf.Abs(Mathf.Abs(parentTileRotation) - 90) < 1)
                {
                    if (yOffset == -1 || xOffset == 1) return true;
                }
                else if (Mathf.Abs(Mathf.Abs(parentTileRotation) - 180) < 1)
                {
                    if (yOffset == -1 || xOffset == -1) return true;
                }
                else if (Mathf.Abs(Mathf.Abs(parentTileRotation) - 270) < 1)
                {
                    if (yOffset == 1 || xOffset == -1) return true;
                }
                break;

            default:
                if (Mathf.Abs(parentTileRotation) < 1 || Mathf.Abs(parentTileRotation - 180) < 1)
                {
                    if (yOffset == 1 || yOffset == -1) return true;
                }
                else if (Mathf.Abs(parentTileRotation - 90) < 1 || Mathf.Abs(parentTileRotation - 270) < 1)
                {
                    if (xOffset == 1 || xOffset == -1) return true;
                }
                break;
        }
        return false;
    }

    Vector3 coordinatesToPointCentered(Coordinates coord)
    {
        GameObject tile = LevelManager.GetTileAt(coord.x, coord.y);
        if (tile != null)
        {
            return tile.transform.position;
        }

        return new Vector3(coord.x * 10f, 0f, coord.y * 10f);
    }

    Vector3 coordinatesToPoint(Coordinates coord)
    {
        return new Vector3(coord.x * 10f, 0f, coord.y * 10f);
    }

    Coordinates pointToCoordinates(Vector3 point)
    {
        Coordinates coord;
        coord.x = (int)point.x / 10;
        coord.y = (int)point.z / 10;
        return coord;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pathPoints[i], 0.5f);
            if (i != 0) Gizmos.DrawLine(pathPoints[i-1], pathPoints[i]);
        }
    }
}
