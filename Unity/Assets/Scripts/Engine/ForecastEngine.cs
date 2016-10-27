﻿using System.Collections.Generic;
using UniRx.Examples;
using UnityEngine;

public class ForecastEngine : MonoBehaviour
{
    public GameObject sphereDebugPrefab;
    void SpawnSpherePrefab(Vector3 position, Vector3 target)
    {
        GameObject newSphere = (GameObject)GameObject.Instantiate(sphereDebugPrefab);
        newSphere.transform.position = position;
        newSphere.transform.localScale = new Vector3(0.01f, 2f / Vector3.Distance(target, position), 0.01f);
    }

    public struct Node
    {
        public int rootIndex;
        public int cost;
        public float heuristicValue;
        public GameState state;
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
    
    public void Awake()
    {
        openList = new List<Node>(maxIterations);
        closedList = new List<Node>(maxIterations);
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

            GenerateChildren(ref currentNode);
            AddChildrenToOpenList();

            if (currentNode.rootIndex != -1)
            {
                closedList.Add(currentNode);
            }
            else
            {
                //Debug.Log(iteration);
            }
            iteration++;
        }

        int bestNodeIndex = GetPrioritaryIndex(closedList);
        tempGoalState = closedList[bestNodeIndex].state;

        bestNodeIndex = closedList[bestNodeIndex].rootIndex;

        //Debug.Log(closedList[bestNodeIndex].state.AI.action); // Debug action
        return closedList[bestNodeIndex].state.AI.action;
    }

    private void AddChildrenToOpenList()
    {
        foreach (Node child in childrenList)
        {
            openList.Add(child);
        }
    }

    private int GetPrioritaryIndex(List<Node> list)
    {
        float minValue = float.MaxValue;
        int prioritaryIndex = 0;

        for (int i = 0; i < list.Count; i++)
        {
            float currentValue = (list[i].cost * list[i].cost * Time.fixedDeltaTime * Time.fixedDeltaTime) + list[i].heuristicValue;
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
        return Vector3.SqrMagnitude(goal.AI.position - start.AI.position) / (start.AI.maxSpeed * start.AI.maxSpeed);
    }

    private void GenerateChildren(ref Node node)
    {
        for (int i = 0; i < validActions.Length; i++)
        {
            VehicleAction action = validActions[i];
            childrenList[i].cost = node.cost + 1;
            childrenList[i].state = gameStateManager.ComputeGameState(node.state, action);
            
            if (node.rootIndex == -1)
            {
                childrenList[i].rootIndex = i;
            }
            else
            {
                childrenList[i].rootIndex = node.rootIndex;
            }
            childrenList[i].heuristicValue = GetHeuristicValue(ref childrenList[i].state, ref goalState);

        }
    }

    void OnDrawGizmos()
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