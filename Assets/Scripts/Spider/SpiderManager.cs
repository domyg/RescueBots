using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderManager : MonoBehaviour
{
    //ID dello Spider Robot
    private string spiderID;

    private NavMeshAgent agent;

    private Animator animator;

    //Posizione del Robot e il filtro di Kalman
    private Vector3 position;
    private Vector3 lastPosition;
    private KalmanFilter kalmanFilter;

    //Variabili per la generazione di una destinazione casuale
    public Vector3 center;
    public float radius;

    //Certificato contenente la chiave Master di cifratura
    private Certificate certificate;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", true);

        kalmanFilter = new KalmanFilter(new Vector2(transform.position.x, transform.position.z));

        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Posizione del Robot calcolata con il filtro di Kalman
        position = GetPosition();

        //Se il Robot è fermo o ha raggiunto la destinazione precedente, calcola una nuova destinazione casuale
        if (!agent.pathPending && agent.remainingDistance < 0.1f || Vector3.Distance(position, lastPosition) < 0.001f)
        {
            Vector3 randomDestination = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * radius + center;
            agent.SetDestination(randomDestination);
        }

        lastPosition = position;
    }

    //Metodo per ottenere la posizione del Robot calcolata con il filtro di Kalman
    public Vector3 GetPosition()
    {
        Vector2 position = kalmanFilter.UpdatePosition(new Vector2(transform.position.x, transform.position.z));
        return new Vector3(position.x, transform.position.y, position.y);
    }

    //Metodo per settare il certificato della chiave Master (usato dal Master Robot)
    public void SetCertificate(Certificate certificate)
    {
        this.certificate = certificate;
    }

    //Metodo per ottenere il certificato della chiave Master (usato dallo Spider Robot nel modulo Communication per fare lo scambio della chiave di sessione)
    public Certificate GetCertificate()
    {
        return certificate;
    }

    //Metodo per settare l'ID dello Spider Robot
    public void SetSpiderID(string spiderID)
    {
        this.spiderID = spiderID;
    }

    //Metodo per ottenere l'ID dello Spider Robot (usato per le comunicazioni con il Master Robot)
    public string GetSpiderID()
    {
        return spiderID;
    }
}
