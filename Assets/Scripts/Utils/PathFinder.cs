using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    //Prefab del percorso
    [SerializeField] GameObject pathPrefab;

    //Lato della mappa
    private int size;
    //Distanza di sicurezza dalle mine
    private int safePadding;

    //Posizione di arrivo
    private Vector3 goal;

    //Mappa
    private int[,] map;

    //Quantità di mine
    private int mines;

    //Percorso trovato
    private List<Node> path;

    // Start is called before the first frame update
    private void Start()
    {
        size = TerrainManager.size;
        safePadding = 1;

        goal = TerrainManager.goal;

        map = new int[size, size];
    }

    //Metodo per segnalare una mina
    public void ReportMine(int x, int z)
    {
        //Se la posizione rilevata è sicura, segnala la mina, altrimenti era già stata segnalata
        if (IsAreaSafe(x, z))
        {
            ReportArea(x, z);
            mines++;
        }
    }

    //Metodo per controllare se una posizione è sicura (3x3 di sicurezza)
    public bool IsAreaSafe(int x, int z)
    {
        for (int i = Math.Max(x - safePadding, 0); i <= Math.Min(x + safePadding, size - 1); i++)
        {
            for (int j = Math.Max(z - safePadding, 0); j <= Math.Min(z + safePadding, size - 1); j++)
            {
                if (map[i, j] == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //Metodo per segnalare un'area come pericolosa
    public void ReportArea(int x, int z)
    {
        for (int i = Math.Max(x - safePadding, 0); i <= Math.Min(x + safePadding, size - 1); i++)
        {
            for (int j = Math.Max(z - safePadding, 0); j <= Math.Min(z + safePadding, size - 1); j++)
            {
                map[i, j] = 1;
            }
        }
    }

    //Metodo per aggiornare il percorso (A*)
    public void UpdatePath()
    {
        AStar aStar = new AStar(size, map);

        path = aStar.FindPath(new Node((int)gameObject.transform.position.x, (int)gameObject.transform.position.z), new Node((int)goal.x, (int)goal.z));
    }

    //Metodo per fornire la posizione successiva nel percorso (utilizzato dal Master Robot)
    public Vector3 GetNextPositionInPath()
    {
        if (path != null)
        {
            //Se il percorso non è vuoto, restituisce la posizione successiva
            if (path.Count > 0)
            {
                Node node = path[0];
                path.RemoveAt(0);

                return new Vector3(node.x + 0.5f, 0, node.z + 0.5f);
            }
        }

        return new Vector3(-1, -1, -1);
    }

    //Metodo per ottenere il numero di mine segnalate
    public int GetMines()
    {
        return mines;
    }
}
