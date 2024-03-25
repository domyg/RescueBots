using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class MasterManager : MonoBehaviour
{
    //Stato corrente del Master (FSM)
    private State currentState;

    //Variabili per la gestione del percorso
    private int size;
    private Vector3 start;
    private Vector3 goal;

    private PathFinder pathFinder;

    //Prefab del ragno
    [SerializeField] GameObject spiderPrefab;

    //Variabili per la gestione delle mine
    private int cachedMines;
    private bool mineDetected;

    //Prefab del percorso e lista dei nodi del percorso
    [SerializeField] GameObject pathPrefab;
    private List<Node> path;

    //Dizionario delle chiavi degli Spider Robot e dei relativi certificati
    private Dictionary<string, Certificate> certificates;

    // Start is called before the first frame update
    private void Start()
    {
        size = TerrainManager.size;
        start = TerrainManager.start;
        goal = TerrainManager.goal;

        pathFinder = GetComponent<PathFinder>();

        transform.position = new Vector3(start.x + 0.5f, 0, start.z + 0.5f);

        path = new List<Node>();

        certificates = new Dictionary<string, Certificate>();

        //Inizializzazione della FSM
        ChangeState(new SpawnState(gameObject));
    }

    // Update is called once per frame
    private void Update()
    {
        //Aggiornamento dello stato corrente
        currentState.UpdateState();
    }

    //Metodo per cambiare lo stato corrente della FSM
    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    //Metodo per spawnare N Spider Robot
    public void SpawnSpiders(int num)
    {
        for (int i = 0; i < num; i++)
        {
            //Spawn di uno Spider Robot
            GameObject spider = Instantiate(spiderPrefab, transform.position + transform.forward, Quaternion.identity);

            SpiderManager spiderManager = spider.GetComponent<SpiderManager>();

            //Assegnazione dell'area di ricerca dello Spider Robot
            spiderManager.center = new Vector3(Random.Range(5, size - 5), 0, Random.Range(5, size - 5));
            spiderManager.radius = 5;

            //Generazione di un nuovo ID e di una nuova chiave Master
            string spiderID = CryptographyManager.GenerateRandomUUID();
            byte[] masterKey = CryptographyManager.GenerateRandomBytes(32);
            byte[] masterIV = CryptographyManager.GenerateRandomBytes(16);

            //Creazione del certificato con ID, chiave, IV e conseguente aggiunta al dizionario e assegnazione allo Spider Robot
            certificates.Add(spiderID, new Certificate(spiderID, masterKey, masterIV));
            spiderManager.SetSpiderID(spiderID);
            spiderManager.SetCertificate(certificates[spiderID]);
        }
    }

    //Metodo per controllare se il Master Robot ha raggiunto la destinazione
    public bool HasReachedGoal()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(goal.x, goal.z)) < 0.3f;
    }

    //Metodo per ottenere il numero di nuove mine segnalate
    public int GetNewMines()
    {
        return pathFinder.GetMines() - cachedMines;
    }

    //Metodo per aggiornare il numero di mine segnalate
    public void UpdateCachedMines()
    {
        cachedMines = pathFinder.GetMines();
    }

    //Metodo per controllare se il Master Robot ha trovato una nuova mina
    public bool IsMineDetected()
    {
        return mineDetected;
    }

    //Metodo per segnalare che il Master Robot ha trovato una nuova mina (usato dal modulo MineSensor)
    public void SetMineDetected(bool mineDetected)
    {
        this.mineDetected = mineDetected;
    }

    //Metodo per mostrare il percorso sul Tablet
    public void AddNodeToPath(Node node)
    {
        path.Add(node);
        Instantiate(pathPrefab, new Vector3(node.x + 0.5f, 0, node.z + 0.5f), Quaternion.identity);
    }

    //Metodo per ottenere il certificato della chiave Master di uno Spider Robot (usato dal modulo Communication)
    public Certificate GetCertificate(string spiderID)
    {
        if (certificates.ContainsKey(spiderID))
            return certificates[spiderID];
        else
            return null;
    }
}
