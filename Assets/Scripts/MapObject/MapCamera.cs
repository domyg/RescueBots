using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    //Oggetto da seguire con la camera
    public Transform target;

    //Posizione Y di default
    float defaultPosY;

    // Start is called before the first frame update
    void Start()
    {
        //Salva la posizione Y di default
        defaultPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Applica posizione
        transform.position = new Vector3(target.position.x, defaultPosY, target.position.z);
        //Applica rotazione
        transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, -90);
    }
}