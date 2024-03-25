using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MasterMineSensor : MonoBehaviour
{
    private MasterManager masterManager;
    private PathFinder pathFinder;

    // Start is called before the first frame update
    void Start()
    {
        masterManager = transform.parent.gameObject.GetComponent<MasterManager>();
        pathFinder = transform.parent.gameObject.GetComponent<PathFinder>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Se viene rilevata una mina, viene segnalata sia al modulo Manager sia al modulo PathFinder e viene mostrata sul Tablet
        if (collider.gameObject.tag == "Mine")
        {
            masterManager.SetMineDetected(true);
            pathFinder.ReportMine((int)collider.gameObject.transform.position.x, (int)collider.gameObject.transform.position.z);
            collider.gameObject.GetNamedChild("MapObject").layer = 7;
            
        }
    }
}
