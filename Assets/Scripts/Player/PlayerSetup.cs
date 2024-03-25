using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;


public class PlayerSetup : MonoBehaviour
{
    //Riferimento al tablet
    [SerializeField] GameObject tablet;

    // Start is called before the first frame update
    void Start()
    {
        //Posiziona il giocatore nella safe area di partenza
        transform.position = new Vector3(TerrainManager.start.x - 1, 0, TerrainManager.start.z + 1);
        transform.rotation = Quaternion.identity;

        //Posiziona il tablet davanti al giocatore
        Instantiate(tablet, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z + 2), Quaternion.Euler(270, 0, 0));
    }
}
