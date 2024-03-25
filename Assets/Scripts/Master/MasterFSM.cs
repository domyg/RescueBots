using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Definizione di una classe astratta per gli stati
public abstract class State
{
    public abstract void Enter();
    public abstract void UpdateState();
    public abstract void Exit();
}

//Stato di ricognizione, il Master Robot attende 10 secondi per valutare l'incremento delle mine in questo periodo
public class RecognitionState : State
{
    private GameObject masterObject;

    private MasterManager masterManager;

    private bool isReady;

    public RecognitionState(GameObject masterObject)
    {
        this.masterObject = masterObject;
    }

    public override void Enter()
    {
        Debug.Log("Enter RecognitionState");

        masterManager = masterObject.GetComponent<MasterManager>();

        //Il Master Robot attende 10 secondi per valutare l'incremento delle mine in questo periodo
        isReady = false;
        masterManager.StartCoroutine(Wait(10));
    }

    public override void UpdateState()
    {
        if (isReady)
        {
            int newMines = masterManager.GetNewMines();
            masterManager.UpdateCachedMines();

            //Se sono state rilevate nuove mine o se il numero di mine è superiore a 10, viene cambiato lo stato
            //Da ricordare che il Master Robot può esso stesso rilevare delle mine non rilevate prima dagli Spider Robot
            if (masterManager.IsMineDetected() || newMines > 10)
            {
                masterManager.SetMineDetected(false);
                masterManager.ChangeState(new SpawnState(masterObject));
            }
            else
            {
                masterManager.ChangeState(new SearchState(masterObject));
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit RecognitionState");
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isReady = true;
    }
}

//Stato di Spawn, il Master Robot crea 10 Spider Robot e poi passa allo stato di ricognizione
public class SpawnState : State
{
    private GameObject masterObject;

    private MasterManager masterManager;

    public SpawnState(GameObject masterObject)
    {
        this.masterObject = masterObject;
    }

    public override void Enter()
    {
        Debug.Log("Enter SpawnState");

        masterManager = masterObject.GetComponent<MasterManager>();

        //Spawn di 10 Spider Robot e cambio di stato a ricognizione
        masterManager.SpawnSpiders(10);

        masterManager.ChangeState(new RecognitionState(masterObject));
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        Debug.Log("Exit SpawnState");
    }
}

//Stato di ricerca, il Master Robot calcola un nuovo percorso con A*
public class SearchState : State
{
    private GameObject masterObject;

    private MasterManager masterManager;

    private bool isReady;

    public SearchState(GameObject masterObject)
    {
        this.masterObject = masterObject;
    }

    public override void Enter()
    {
        Debug.Log("Enter SearchState");

        masterManager = masterObject.GetComponent<MasterManager>();

        isReady = false;
        StartSearch();
    }

    public override void UpdateState()
    {
        if (isReady)
        {
            int newMines = masterManager.GetNewMines();

            //Se sono state rilevate più di 10 mine, allora l'ambiente è ancora troppo pericoloso, meglio fare un'altra ricognizione
            if (newMines > 10)
            {
                masterManager.ChangeState(new SpawnState(masterObject));
            }
            else
            {
                masterManager.ChangeState(new WalkState(masterObject));
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit SearchState");
    }

    public void StartSearch()
    {
        masterObject.GetComponent<PathFinder>().UpdatePath();
        isReady = true;
    }
}

//Stato di cammino, il Master Robot si muove lungo il percorso calcolato con A*
public class WalkState : State
{
    private GameObject masterObject;

    private MasterManager masterManager;

    private NavMeshAgent agent;

    private Vector3 position;
    private Vector3 nextPosition;

    public WalkState(GameObject masterObject)
    {
        this.masterObject = masterObject;
    }

    public override void Enter()
    {
        Debug.Log("Enter WalkState");

        masterManager = masterObject.GetComponent<MasterManager>();

        agent = masterObject.GetComponent<NavMeshAgent>();
    }

    public override void UpdateState()
    {
        int newMines = masterManager.GetNewMines();

        //Se sono state rilevate più di 10 nuove mine o se viene rilevata una mina vicina, allora si torna subito allo stato di ricognizione
        if (masterManager.IsMineDetected() || newMines > 10)
        {
            agent.SetDestination(position);
            masterManager.ChangeState(new RecognitionState(masterObject));
        }

        //Movimento del Master Robot lungo il percorso
        if (!agent.pathPending && agent.remainingDistance < 0.01f)
        {
            //Posizione del Master Robot
            position = masterObject.transform.position;
            masterManager.AddNodeToPath(new Node((int)position.x, (int)position.z));

            //Nuova posizione da raggiungere
            nextPosition = masterObject.GetComponent<PathFinder>().GetNextPositionInPath();

            //Se il Master Robot ha raggiunto la destinazione o se non ci sono altre posizioni da raggiungere, allora si va allo stato di Goal
            if (masterManager.HasReachedGoal() || nextPosition == new Vector3(-1, -1, -1))
            {
                masterManager.ChangeState(new GoalState(masterObject));
            }
            //Altrimenti si imposta la destinazione al nodo successivo
            else
            {
                agent.SetDestination(nextPosition);
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit WalkState");
    }
}

//Stato di Goal, il Master Robot ha raggiunto la destinazione e si ferma
public class GoalState : State
{
    private GameObject masterObject;

    public GoalState(GameObject masterObject)
    {
        this.masterObject = masterObject;
    }

    public override void Enter()
    {
        Debug.Log("Enter GoalState");
    }

    public override void UpdateState()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exit GoalState");
    }
}