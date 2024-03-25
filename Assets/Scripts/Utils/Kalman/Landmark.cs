using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    //Posizione del landmark
    public Vector2 position;
    
    public Landmark(Vector2 position)
    {
        this.position = position;
    }
    
    //Metodo per calcolare la distanza (afflitta da errore) dal landmark
    public float Distance(Vector2 position)
    {
        position = new Vector2(position.x + Random.Range(-0.5f, 0.5f), position.y + Random.Range(-0.5f, 0.5f));
        return Vector2.Distance(this.position, position);
    }
}
