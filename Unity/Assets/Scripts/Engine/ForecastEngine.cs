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
    }

    private VehicleAction[] validActions =
    {
        VehicleAction.NO_INPUT,
        VehicleAction.ACCELERATE,
        VehicleAction.BRAKE,
        VehicleAction.LEFT,
        VehicleAction.RIGHT,

        VehicleAction.ACCELERATE | VehicleAction.LEFT,
        VehicleAction.ACCELERATE | VehicleAction.RIGHT,

        VehicleAction.BRAKE | VehicleAction.RIGHT,
        VehicleAction.BRAKE | VehicleAction.LEFT
    };

    public GameStateManager gameStateManager;

    private List<Node> openList;
    private List<Node> closedList;
    private Node[] childrenList;

    private GameState goalState;

    public int maxIterations;
    private int iteration;


    public void Awake()
    {
        Reset();
        openList = new List<Node>(maxIterations);
        closedList = new List<Node>(maxIterations);
        childrenList = new Node[validActions.Length];
    }

    public void Reset()
    {
        iteration = 0;
        openList.Clear();
    }

    public void SetGoalState(GameState state)
    {
        goalState = state;
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
            int currentNodeIndex = GetPrioritaryIndex();
            Node currentNode = openList[currentNodeIndex];
            openList.RemoveAt(currentNodeIndex);

            GenerateChildren(ref currentNode);
            foreach (Node child in childrenList)
            {
                openList.Add(child);
            }

            closedList.Add(currentNode);
        }

        return VehicleAction.NO_INPUT;
    }

    private int GetPrioritaryIndex()
    {
        // todo (prend en compte le cout + heuristique)
        return 0;
    }

    private float GetHeuristicValue(ref GameState start, ref GameState goal)
    {
        return Vector3.SqrMagnitude(goal.AI.position - start.AI.position) / (goal.AI.maxSpeed * goal.AI.maxSpeed);
    }

    private void GenerateChildren(ref Node node)
    {
        for (int i = 0; i < validActions.Length; i++)
        {
            VehicleAction action = validActions[i];
            childrenList[i].cost = node.cost + 1;
            childrenList[i].state = node.state;
            childrenList[i].state = gameStateManager.ComputeGameState(childrenList[i].state, action);

            if (node.rootIndex == -1)
            {
                childrenList[i].rootIndex = i + 1;
            }
            else
            {
                childrenList[i].rootIndex = node.rootIndex;
            }

            childrenList[i].heuristicValue = GetHeuristicValue(ref childrenList[i].state, ref goalState);
        }
    }

}
