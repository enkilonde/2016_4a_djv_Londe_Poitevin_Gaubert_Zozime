using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using Debug = UnityEngine.Debug;

public class ForecastEngine
{

    // TODO : trouver des valeurs correctes pour ces constantes
    public const float THRESHOLD = 0.1f;
    public const float FIXED_STEP = 0.016f;

    public struct Node
    {
        public int index;
        public int parentIndex;
        public int childrenStateIndex;
        public float heuristicValue;
        public GameState gameState;
    }

    // TODO : définir un type de retour (probablement un tableau de node)
    public static List<Node> FindBestActions(Node start, Node goal, int maxIteration)
    {
        int iterations = 0;

        List<Node> closedList = new List<Node>(maxIteration);
        List<Node> openList = new List<Node>(maxIteration);

        int childrenCount = 0;
        Node[] children = new Node[VehicleProperties.GetInputCombinationCount()];

        GameState goalState = generateGoalGameState(start.gameState);


        openList.Add(start);

        while (openList.Count > 0 && iterations >= maxIteration)
        {
            Node currentNode = PopPrioritaryNodeInList(openList);
            currentNode.index = closedList.Count;

            childrenCount = generateChildren(currentNode, children, goalState);
            iterations += childrenCount;
            for (int i = 0; i < childrenCount; i++)
            {
                GameState currentChildState = children[i].gameState;
                Node? closedListClosestNode = findClosestNode(closedList, currentChildState, THRESHOLD);
                Node? openListclosestNode = findClosestNode(openList, currentChildState, THRESHOLD);

                // si l'enfant parcouru a un équivalent (très proche) dans la closed list avec un cout moins intéressant
                if (closedListClosestNode.HasValue &&
                     closedListClosestNode.Value.gameState.elapsedTime > currentNode.gameState.elapsedTime)
                {
                    continue;
                }

                if (openListclosestNode.HasValue &&
                    openListclosestNode.Value.gameState.elapsedTime > currentNode.gameState.elapsedTime)
                {
                    continue;
                }

                if (getDistanceBetweenStates(currentChildState, goalState) >
                    getDistanceBetweenStates(currentNode.gameState, goalState))
                {
                    continue;
                }

                currentChildState.elapsedTime = currentNode.gameState.elapsedTime + FIXED_STEP;
                openList.Add(children[i]);
            }
            closedList.Add(currentNode);
        }


        // Find closest to goalState among the last batch of children
        float minDistance = float.MaxValue;
        Node? closestNodeToGoal = null;

        for (int i = 0; i < childrenCount; i++)
        {
            float currentDistance = getDistanceBetweenStates(children[i].gameState, goalState);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestNodeToGoal = children[i];
            }
        }

        if (closestNodeToGoal.HasValue)
        {
            // Reconstruct path
            Stack<Node> bestPathStack = new Stack<Node>();
            Node currentNode = closestNodeToGoal.Value;
            while (currentNode.parentIndex != -1)
            {
                bestPathStack.Push(currentNode);
                currentNode = children[currentNode.parentIndex];
            }

            bestPathStack.Reverse();
            return bestPathStack.ToList();
        }

        return null;
    }

    // TODO : à implémenter
    private static GameState generateGoalGameState(GameState currentGameState)
    {
        return default(GameState);
    }

    /**
     * Génère les enfants de currentNode dans childrenArray avec pour objectif goalState
     */
    private static int generateChildren(Node currentNode, Node[] childrenArray, GameState goalState)
    {
        int indexOffset = 0;
        int childrenCount = 0;
        for (int i = 0; i <= (int)VehicleProperties.GetAllFlagsOnVehicleAction(); i++)
        {
            VehicleAction currentActionsCombination = (VehicleAction) i;

            // Ignore les cas où les combinaisons d'actions sont : (left, right) et (accelerate, brake)
            if ((currentActionsCombination & VehicleAction.LEFT) == VehicleAction.LEFT &&
                (currentActionsCombination & VehicleAction.RIGHT) == VehicleAction.RIGHT ||
                (currentActionsCombination & VehicleAction.ACCELERATE) == VehicleAction.ACCELERATE &&
                (currentActionsCombination & VehicleAction.BRAKE) == VehicleAction.BRAKE)
            {
                indexOffset--;
                continue;
            }

            // TODO : gérer la copie profonde des membres de la structure (notamment pour banana pool)
            Node currentChild = currentNode;
            currentChild.gameState = currentNode.gameState;
            currentChild.gameState.player.UpdateVehicle(currentChild.gameState.player.action, true);
            currentChild.gameState.AI.UpdateVehicle(currentActionsCombination, true);
            currentChild.gameState.elapsedTime += FIXED_STEP;
            currentChild.parentIndex = currentNode.index;
            currentChild.heuristicValue = getDistanceBetweenStates(currentChild.gameState, goalState);
            childrenArray[i + indexOffset] = currentChild;

            childrenCount++;
        }

        return childrenCount;
    }

    /**
     * Retourne null ou le node avec le GameState le plus proche de stateToFind dans la liste
     * nodelist.
     */
    private static Node? findClosestNode(List<Node> nodeList, GameState stateToFind, float threshold)
    {
        float minDistance = float.MaxValue;
        Node closestNode = default(Node);

        foreach (Node n in nodeList)
        {
            float currentDistance = getDistanceBetweenStates(n.gameState, stateToFind);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestNode = n;
            }
        }

        Debug.Log("Min distance is : " + minDistance);

        if (minDistance <= threshold)
        {
            return closestNode;
        }
        return null;
    }

    /**
     * (Calcul de l'heuristique)
     * Distance entre deux Game State
     */
    private static float getDistanceBetweenStates(GameState startState, GameState goalState)
    {
        return (goalState.elapsedTime - startState.elapsedTime) * (goalState.elapsedTime - startState.elapsedTime) +
               (goalState.AI.position - goalState.AI.position).sqrMagnitude;
    }

    private static Node PopPrioritaryNodeInList(List<Node> nodeLists)
    {
        Node PrioritaryNode = nodeLists[0];
        float minHeuristic = nodeLists[0].heuristicValue;
        int indexOfPriorityNode = 0;
        for (int i = 1; i < nodeLists.Count; i++)
        {
            if (nodeLists[i].heuristicValue < minHeuristic)
            {
                PrioritaryNode = nodeLists[i];
                minHeuristic = nodeLists[i].heuristicValue;
                indexOfPriorityNode = i;
            }
        }
        nodeLists.RemoveAt(indexOfPriorityNode);
        return PrioritaryNode;
    }
}
