using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private int size;
    private int[,] map;

    public AStar(int size, int[,] map)
    {
        this.size = size;
        this.map = map;
    }

    //Metodo per ottenere i vicini di un nodo
    private List<Node> GetNeighbors(Node node)
    {
        List <Node> neighbors = new List <Node>();

        neighbors.Add(new Node(node.x, node.z + 1));
        neighbors.Add(new Node(node.x + 1, node.z));
        neighbors.Add(new Node(node.x - 1, node.z));
        neighbors.Add(new Node(node.x, node.z - 1));

        return neighbors;
    }

    public List<Node> FindPath(Node start, Node goal)
    {
        //Lista aperta e chiusa
        PriorityQueue<Node> openList = new PriorityQueue<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        //Aggiunta del nodo di partenza alla lista aperta
        openList.Enqueue(start);

        //Finché la lista aperta non è vuota si continua a cercare
        while (openList.Count > 0)
        {
            //Prende il nodo con priorità minima
            Node currentNode = openList.Dequeue();
            //Aggiunge il nodo alla lista chiusa
            closedList.Add(currentNode);

            //Se il nodo corrente è il nodo di arrivo, si è trovato il percorso
            if (currentNode.Equals(goal))
            {
                List<Node> path = new List<Node>();

                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }

                path.Reverse();

                return path;
            }

            //Per ogni vicino del nodo corrente, se è valido e non è già stato visitato, si calcolano g, h e f e si aggiunge il nodo alla lista aperta
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!closedList.Contains(neighbor) && neighbor.x >= 0 && neighbor.x < size && neighbor.z >= 0 && neighbor.z < size && map[neighbor.x, neighbor.z] == 0)
                {
                    neighbor.g = currentNode.g + 1;
                    neighbor.h = Math.Abs(neighbor.x - goal.x) + Math.Abs(neighbor.z - goal.z);
                    neighbor.parent = currentNode;

                    if (!openList.UpdatePriority(neighbor))
                    {
                        openList.Enqueue(neighbor);
                    }
                }
            }
        }

        return null;
    }
}
