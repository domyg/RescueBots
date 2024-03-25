using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    //Lato della mappa
    public static int size;
    //Numero di mine
    public int mines;

    //Posizioni di partenza e arrivo
    public static Vector3 start;
    private List<Node> safeStart;

    public static Vector3 goal;
    private List<Node> safeGoal;

    //Prefab di mine e safe area
    [SerializeField] GameObject safePrefab;
    [SerializeField] GameObject minePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Inizializzazione della mappa
        size = 50;
        mines = size * size / 25;

        InitializeSafeAreas();
        InitializeMines();
    }

    //Inizializzazione delle aree 5x3 sicure
    private void InitializeSafeAreas()
    {
        start = new Vector3(Random.Range(2, size - 2), 0, 0);
        safeStart = new List<Node>();

        for (int x = (int)start.x - 2; x <= (int)start.x + 2; x++)
        {
            for (int z = (int)start.z; z <= 2; z++)
            {
                safeStart.Add(new Node(x, z));
                Instantiate(safePrefab, new Vector3(x + 0.5f, 0, z + 0.5f), Quaternion.identity);
            }
        }

        goal = new Vector3(Random.Range(2, size - 2), 0, size - 1);
        safeGoal = new List<Node>();
        
        for (int x = (int)goal.x - 2; x <= (int)goal.x + 2; x++)
        {
            for (int z = (int)goal.z; z >= size - 3; z--)
            {
                safeGoal.Add(new Node(x, z));
                Instantiate(safePrefab, new Vector3(x + 0.5f, 0, z + 0.5f), Quaternion.identity);
            }
        }
    }

    //Inizializzazione delle mine in punti random della mappa (esclusi i punti di partenza e arrivo)
    private void InitializeMines()
    {
        Vector3 randomPosition;
        for (int i = 0; i < mines; i++)
        {
            do
            {
                randomPosition = new Vector3(Random.Range(0, size), -0.1f, Random.Range(0, size));
            } while (safeStart.Contains(new Node((int) randomPosition.x, (int) randomPosition.z)) || safeGoal.Contains(new Node((int) randomPosition.x, (int) randomPosition.z)));

            randomPosition = new Vector3((int) randomPosition.x + 0.5f, randomPosition.y, (int) randomPosition.z + 0.5f);

            Instantiate(minePrefab, randomPosition, Quaternion.identity);
        }
    }

}
