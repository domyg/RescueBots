using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //Evento scaturito quando viene inviato un messaggio
    public static event Action<string> OnMessageReceived;

    //Metodo per inviare un messaggio
    public static void SendMessage(string message)
    {
        OnMessageReceived?.Invoke(message);
    }
}
