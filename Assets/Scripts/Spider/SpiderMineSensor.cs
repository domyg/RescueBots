using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SpiderMineSensor : MonoBehaviour
{
    private SpiderManager spiderManager;
    private SpiderCommunication spiderCommunication;

    AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        spiderManager = transform.parent.gameObject.GetComponent<SpiderManager>();
        spiderCommunication = transform.parent.gameObject.GetComponent<SpiderCommunication>();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Se viene rilevata una mina, segnala la posizione al Master Robot
        if (collider.gameObject.tag == "Mine")
        {
            //Suono di rilevamento mina dello Spider Robot
            audioSource.Play();

            //Segnalazione della posizione al Master Robot
            Vector3 position = spiderManager.GetPosition();
            spiderCommunication.SendMineAlert(position);

            //Segnalazione sul Tablet
            if (collider.gameObject.GetNamedChild("MapObject").layer != 7)
            {
                collider.gameObject.GetNamedChild("MapObject").layer = 7;
                collider.gameObject.GetNamedChild("MapObject").transform.position = position;
            }
            else
            {
                collider.gameObject.GetNamedChild("MapObject").transform.position = collider.gameObject.GetNamedChild("MapObject").transform.position * 0.9f + position * 0.1f;
            }
        }
    }
}
