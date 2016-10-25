using System.Collections.Generic;

public class AStar
{

    // TODO : trouver des valeurs correctes pour ces constantes
    public const float THRESHOLD = 0.1f;
    public const float FIXED_STEP = 0.016f;

    public struct Node
    {
        public GameState gameState;
        public GameState lastFrameState;
    }

    // TODO : définir un type de retour (probablement un tableau de node)
    public static void FindGameStateTransitions(Node start, Node goal, int maxIteration)
    {
        List<Node> closedList = new List<Node>();
        List<Node> openList = new List<Node>();
        float minDistance = float.MaxValue;

        openList.Add(start);

        while (openList.Count > 0)
        {
            Node currentNode = openList[openList.Count - 1];
            float currentDistance = getDistanceBetweenStates(currentNode.gameState, goal.gameState);
            openList.RemoveAt(openList.Count - 1);

            if (currentDistance < minDistance)
            {
                // TODO : repenser cette partie qui ne correspond pas à notre cas
                // TODO : retrouver le chemin des différents gamestate
            }

            Node[] children = generateChildren(currentNode);
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

    private static Node[] generateChildren(Node currentGameState)
    {
        // TODO : générer un enfant par combinaison d'input possible
        // TODO : penser à affecter currentGameState dans generatedGameState[i].lastFrameState
        return default(Node[]);
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
}
