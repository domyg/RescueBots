using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nodo utilizzato dall'algoritmo A*
public class Node : IComparable<Node>
{
    public int x;
    public int z;
    public int g;
    public double h;
    public Node parent;

    public Node(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    //Calcolo della funzione di costo
    public double f
    {
        get
        {
            return g + h;
        }
    }

    //Comparazione tra nodi (per l'ordinamento della lista aperta)
    public int CompareTo(Node other)
    {
        if (other == null)
        {
            return 1;
        }

        return f.CompareTo(other.f);
    }

    //Metodo per controllare se due nodi sono uguali (stessa posizione)
    public override bool Equals(object obj)
    {
        Node other = obj as Node;
        return other != null && x == other.x && z == other.z;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ z.GetHashCode();
    }
}