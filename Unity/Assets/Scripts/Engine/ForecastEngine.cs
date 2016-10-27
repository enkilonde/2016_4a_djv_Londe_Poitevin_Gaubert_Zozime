using System.Collections.Generic;
using UniRx.Examples;
using UnityEngine;

public class ForecastEngine : MonoBehaviour
{
    public struct Node
    {
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
    /* POUR MONTRER AU PROF 2 */
    public int framesPerIteration = 1;
    public int frameOffsetToIgnore = 20;
    public float distanceOffsetToIgnore = 0.1f;
    private int maxCurrentCost = 0;
    
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
        startNode.heuristicValue = GetHeuristicValue(ref startState, ref goalState);
        startNode.rootIndex = -1;
        startNode.state = startState;

        openList.Add(startNode);

        while (openList.Count > 0 && iteration < maxIterations)
        {
            int currentNodeIndex = GetPrioritaryIndex(openList);
            Node currentNode = openList[currentNodeIndex];
            openList.RemoveAt(currentNodeIndex);

            if (currentNode.cost < maxCurrentCost - frameOffsetToIgnore)
            {
                foreach (Node closedNode in closedList)
                {
                    if (Vector3.SqrMagnitude(currentNode.state.AI.position - closedNode.state.AI.position) < distanceOffsetToIgnore)
                    {
                        continue; // skip the end of the while
                    }
                }
            } 

                maxCurrentCost = Mathf.Min(maxCurrentCost, currentNode.cost + 1);
            GenerateChildren(ref currentNode);
            AddChildrenToOpenList();

            if (currentNode.rootIndex != -1)
            {
                closedList.Add(currentNode);
            }
        }

        //int bestNodeIndex = GetPrioritaryIndex(closedList);
        int bestNodeIndex = GetFinalPrioritaryIndex();
        tempGoalState = closedList[bestNodeIndex].state; // debug purpose

        bestNodeIndex = closedList[bestNodeIndex].rootIndex;

        Debug.Log(closedList[bestNodeIndex].state.AI.action); // Debug action


        return closedList[bestNodeIndex].state.AI.action;
    }

    private void AddChildrenToOpenList()
    {
        foreach (Node child in childrenList)
        {
            if (child.state.AI.ground != GroundType.Wall) { openList.Add(child); }
        }
    }

    private int GetPrioritaryIndex(List<Node> list)
    {
        float minValue = 999999f;
        int prioritaryIndex = 0;
        float fixedTime = Time.fixedDeltaTime;

        Node[] nodesList = list.ToArray();
        int listLenght = nodesList.Length;
        //Debug.Log(list.Count);
        for (int i = 0; i < listLenght; i++)
        {
            //Node nodeTemp = nodesList[i];
            float currentValue = nodesList[i].GetCurrentValue(fixedTime);
            
            if (currentValue < minValue)
            {
                minValue = currentValue;
                prioritaryIndex = i;
            }
        }
        return prioritaryIndex;
    }
    private int GetFinalPrioritaryIndex()
    {
        float minValue = 999999f;
        int prioritaryIndex = 0;

        float currentValue;

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
        if (GameStateManager.isEntityInGrass(start.AI.position) == GroundType.Grass)
        { groundFactor = 2f; }
        return groundFactor * Vector3.SqrMagnitude(goal.AI.position - start.AI.position) / (VehicleStaticProperties.maxSpeed * VehicleStaticProperties.maxSpeed);
        

        //return Vector3.SqrMagnitude(goal.AI.position - start.AI.position) / (VehicleStaticProperties.maxSpeed * VehicleStaticProperties.maxSpeed);
    }

    private void GenerateChildren(ref Node node)
    {
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

            /*childrenList[i].cost = node.cost + 1;
            childrenList[i].state = gameStateManager.ComputeGameState(node.state, action);*/
            
            if (node.rootIndex == -1)
            {
                childrenList[i].rootIndex = i;
            }
            else
            {
                childrenList[i].rootIndex = node.rootIndex;
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

        if (tempGoalState.AI.position != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameStateManager.gameState.AI.position, tempGoalState.AI.position);
            Gizmos.DrawSphere(tempGoalState.AI.position, 0.3f);
        }
    }

}
