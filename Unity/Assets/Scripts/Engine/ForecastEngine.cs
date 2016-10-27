using System.Collections.Generic;
using System.Linq;
using UniRx.Examples;
using UnityEngine;

public class ForecastEngine : MonoBehaviour
{
    public struct Node
    {
        public int parentIndex;
        public int rootIndex;
        public int cost;
        public float heuristicValue;
        public GameState state;
        //float currentValue = (nodeTemp.cost * nodeTemp.cost * fixedTime * fixedTime) + nodeTemp.heuristicValue;
        public float GetCurrentValue(float fixedTime)
        {
            return (cost * cost * fixedTime * fixedTime) + heuristicValue;
        }

    }

    private VehicleAction[] validActions =
    {
        VehicleAction.ACCELERATE,
        VehicleAction.LEFT,
        VehicleAction.RIGHT,

        VehicleAction.ACCELERATE | VehicleAction.LEFT,
        VehicleAction.ACCELERATE | VehicleAction.RIGHT,

        VehicleAction.BRAKE | VehicleAction.RIGHT,
        VehicleAction.BRAKE | VehicleAction.LEFT,
        VehicleAction.BRAKE,
        VehicleAction.NO_INPUT
    };

    public GameStateManager gameStateManager;

    private List<Node> openList;
    private List<Node> closedList;
    private Node[] childrenList;

    private GameState goalState;
    private GameState tempGoalState;

    public int maxIterations;
    private int iteration;


    public int framesPerIteration = 1;
    public int frameOffsetToIgnore = 20;
    public float distanceOffsetToIgnore = 0.1f;
    private int maxCurrentCost = 0;


    private List<Node> debugHierarchy;

    public void Awake()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();

        openList = new List<Node>(maxIterations + 9);
        closedList = new List<Node>(maxIterations + 9);
        childrenList = new Node[validActions.Length];
        Reset();
    }

    public void Reset()
    {
        iteration = 0;
        openList.Clear();
        closedList.Clear();
    }

    public void SetGoalState(GameState state)
    {
        goalState = state;
        tempGoalState = state;
    }

    public VehicleAction getBestAction(GameState startState)
    {
        Reset();

        Node startNode;
        startNode.cost = 0;
        startNode.parentIndex = -42; // unused value, but still set a magic number...just in case...
        startNode.heuristicValue = GetHeuristicValue(ref startState, ref goalState);
        startNode.rootIndex = -1;
        startNode.state = startState;

        openList.Add(startNode);

        while (openList.Count > 0 && iteration < maxIterations) //BOUCLE WHILE
        {
            int currentNodeIndex = GetPrioritaryIndex(openList);
            Node currentNode = openList[currentNodeIndex];
            openList.RemoveAt(currentNodeIndex);

            if (currentNode.cost < maxCurrentCost - frameOffsetToIgnore)
            {
                bool canIskip = false;
                foreach (Node closedNode in closedList)
                {
                    if (Vector3.SqrMagnitude(currentNode.state.AI.position - closedNode.state.AI.position) < distanceOffsetToIgnore)
                    {
                        canIskip = true;
                        break; // skip the end of the while
                    }
                }
                if (canIskip) continue;
            }

            maxCurrentCost = Mathf.Min(maxCurrentCost, currentNode.cost + 1);

            GenerateChildren(ref currentNode, closedList.Count);
            AddChildrenToOpenList();

            if (currentNode.rootIndex != -1)
            {
                closedList.Add(currentNode);
            }
            else
            {
                // On veut que les enfants de la premiere generation soient à l'origine de la closedList.
                for (int i = 0; i < childrenList.Length; i++)
                {
                    currentNodeIndex = 0;
                    currentNode = openList[currentNodeIndex];
                    openList.RemoveAt(currentNodeIndex);

                    GenerateChildren(ref currentNode, closedList.Count);
                    AddChildrenToOpenList();

                    closedList.Add(currentNode);
                }
            }
        }

        int bestNodeIndex = GetFinalPrioritaryIndex();

        debugHierarchy = findNodeHierarchy(closedList[bestNodeIndex]);

        tempGoalState = closedList[bestNodeIndex].state; // debug purpose
        bestNodeIndex = closedList[bestNodeIndex].rootIndex;

        Debug.Log(closedList[bestNodeIndex].state.AI.action); // Debug action

        if (closedList.Count == 0) return VehicleAction.NO_INPUT;

        //Debug.Log("CloseList Taille = " + closedList.Count + " , bestNodeIndex = " + bestNodeIndex);

        return closedList[bestNodeIndex].state.AI.action;
    }

    private List<Node> findNodeHierarchy(Node node)
    {
        List<Node> hierarchy = new List<Node>();

        Node currentNode = node;

        while (currentNode.parentIndex != -1)
        {
            int index = closedList.FindIndex(element => element.Equals(currentNode));
            if (index == currentNode.parentIndex)
            {
                Debug.Log("duh ?!");
                break;
            }
            hierarchy.Add(currentNode);
            currentNode = closedList[currentNode.parentIndex];
        }

        return hierarchy;
    }

    private void AddChildrenToOpenList()
    {
        int _l = childrenList.Length;
        for (int i = 0; i < _l; i++)
        {
            if (childrenList[i].state.AI.ground != GroundType.Wall) openList.Add(childrenList[i]);
        }
    }

    private int GetPrioritaryIndex(List<Node> list)
    {
        float minValue = 999999f;
        int prioritaryIndex = 0;
        float fixedTime = Time.fixedDeltaTime;

        Node[] nodesList = list.ToArray();
        int listLenght = nodesList.Length;

        for (int i = 0; i < listLenght; i++)
        {

            if (nodesList[i].GetCurrentValue(fixedTime) < minValue)
            {
                minValue = nodesList[i].GetCurrentValue(fixedTime);
                prioritaryIndex = i;
            }
        }
        return prioritaryIndex;
    }

    private int GetFinalPrioritaryIndex()
    {
        float minValue = 999999f;
        int prioritaryIndex = 0;


        float currentValue = 0;

        for (int i = 0; i < closedList.Count; i++)
        {
            currentValue = closedList[i].heuristicValue;
            if (currentValue < minValue)
            {

                minValue = currentValue;
                prioritaryIndex = i;
            }
        }
        return prioritaryIndex;
    }

    private float GetHeuristicValue(ref GameState start, ref GameState goal)
    {
        /* POUR MONTRER AU PROF (1) */
        float groundFactor = 1f;
        if(start.AI.ground == GroundType.Grass)
        { groundFactor = 2f; }
        return groundFactor * Vector3.SqrMagnitude(goal.AI.position - start.AI.position) / (VehicleStaticProperties.maxSpeed * VehicleStaticProperties.maxSpeed);


        //return Vector3.SqrMagnitude(goal.AI.position - start.AI.position) / (VehicleStaticProperties.maxSpeed * VehicleStaticProperties.maxSpeed);
    }

    private void GenerateChildren(ref Node node, int nodeFutureIndex)
    {
        //Debug.Log("root" + node.rootIndex);
        for (int i = 0; i < validActions.Length; i++)
        {
            VehicleAction action = validActions[i];

            /*if (isOppositeAction(action, node.state.AI.action))
            {
                continue;
            }*/

            /* POUR MONTRER AU PROF 2 */
            childrenList[i].cost = node.cost + i;
            childrenList[i].state = gameStateManager.ComputeGameState(node.state, action, framesPerIteration);

            if (node.rootIndex == -1)
            {
                childrenList[i].rootIndex = i;
                childrenList[i].parentIndex = -42;
            }
            else
            {
                childrenList[i].rootIndex = node.rootIndex;
                childrenList[i].parentIndex = nodeFutureIndex;
            }
            childrenList[i].heuristicValue = GetHeuristicValue(ref childrenList[i].state, ref goalState);

            iteration++;
        }
    }

    /// <summary>
    /// Totally Ineffective funtion
    /// </summary>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    /// <returns></returns>
    bool isOppositeAction(VehicleAction action1, VehicleAction action2)
    {
        switch (action1)
        {
            case VehicleAction.ACCELERATE:
                return (action2 == VehicleAction.BRAKE ||
                    action2 == (VehicleAction.BRAKE | VehicleAction.RIGHT) ||
                    action2 == (VehicleAction.BRAKE | VehicleAction.LEFT));
                break;
            case VehicleAction.BRAKE:
                return (action2 == VehicleAction.ACCELERATE ||
                    action2 == (VehicleAction.ACCELERATE | VehicleAction.RIGHT) ||
                    action2 == (VehicleAction.ACCELERATE | VehicleAction.LEFT));
                break;
            case VehicleAction.LEFT:
                return (action2 == VehicleAction.RIGHT ||
                    action2 == (VehicleAction.RIGHT | VehicleAction.ACCELERATE) ||
                    action2 == (VehicleAction.RIGHT | VehicleAction.BRAKE));
                break;
            case VehicleAction.RIGHT:
                return (action2 == VehicleAction.LEFT ||
                    action2 == (VehicleAction.LEFT | VehicleAction.ACCELERATE) ||
                    action2 == (VehicleAction.LEFT | VehicleAction.BRAKE));
                break;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (openList != null)
        {
            Gizmos.color = Color.red;
            foreach (Node node in openList)
            {
                Gizmos.DrawLine(gameStateManager.gameState.AI.position, node.state.AI.position);
            } 
        }

        if (debugHierarchy != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < debugHierarchy.Count - 1; i++)
            {
                Gizmos.DrawSphere(debugHierarchy[i].state.AI.position, 03f);
                Gizmos.DrawLine(debugHierarchy[i].state.AI.position, debugHierarchy[i + 1].state.AI.position);
            }
        }

        if (tempGoalState.AI.position != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameStateManager.gameState.AI.position, tempGoalState.AI.position);
            Gizmos.DrawSphere(tempGoalState.AI.position, 0.3f);
        }
    }

}
