using System.Collections.Generic;

public class AStar
{

    // TODO : trouver des valeurs correctes pour ces constantes
    public const float THRESHOLD = 0.1f;
    public const float FIXED_STEP = 0.016f;

    public struct Node
    {
        public GameState gameState;
        public int parentFrameStateIndex;
        public int childrenStateIndex;
        public float heuristicValue;
    }

    // TODO : définir un type de retour (probablement un tableau de node)
    public static void FindGameStateTransitions(Node start, Node goal, int maxIteration)
    {
        int iterations = 0;
        List<Node> closedList = new List<Node>(maxIteration);
        List<Node> openList = new List<Node>(maxIteration);
        Node[] children = new Node[(int)VehicleAction.Count];
        float minDistance = float.MaxValue;

        openList.Add(start);

        while (openList.Count > 0 && iterations >= maxIteration)
        {
            Node currentNode = PopPrioritaryNodeInList(openList);
            float currentDistance = getDistanceBetweenStates(currentNode.gameState, goal.gameState);

            // TODO : remplir la "grosse liste" plutôt
            iterations = generateChildren(currentNode, children, iterations, maxIteration);
            for (int i = 0; i < children.Length; i++)
            {
                GameState currentChildState = children[i].gameState;
                Node? closedListClosestNode = findClosestNode(closedList, currentChildState, THRESHOLD);
                Node? openListclosestNode = findClosestNode(openList, currentChildState, THRESHOLD);

                if (!(closedListClosestNode != null &&
                    ((Node)closedListClosestNode).gameState.elapsedTime < currentNode.gameState.elapsedTime ||
                    openListclosestNode != null &&
                    ((Node)openListclosestNode).gameState.elapsedTime < currentNode.gameState.elapsedTime))
                {
                    currentChildState.elapsedTime = currentNode.gameState.elapsedTime + FIXED_STEP;
                    openList.Add(children[i]);
                }
            }
            closedList.Add(currentNode);
        }

    }

    // TODO : à implémenter
    private static Node generateGoalGameState(GameState currentGameState)
    {
        return default(Node);
    }

    private static int generateChildren(Node currentNode, Node[] childrenArray, int iterations, int maxIteration)
    {
        for (int i = 0; i < (int)VehicleAction.Count - 1; i++)
        {
            // Here we assume the player keep doing what he/she was doing on the last frame
            // TODO : penser à affecter currentGameState dans generatedGameState[i].lastFrameState
        }

        return default(int);
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
               (goalState.AI.transform.position - goalState.AI.transform.position).sqrMagnitude;
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
