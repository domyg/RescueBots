using UnityEngine.Audio;
using System;
using UnityEngine;

//La classe Sound rappresenta un suono nell'AudioManager con attributi come il nome, il clip audio, il volume, il pitch e se deve essere riprodotto in loop.
[System.Serializable]
public class Sound
{
    //Nome del suono
    public string name;

    //Clip audio
    public AudioClip clip;

    //Volume del suono compreso tra 0 e 1
    [Range(0,1)]
    public float volume = 1;

    //Pitch del suono compreso tra -3 e 3
    [Range(-3,3)]
    public float pitch = 1;

    //Se il suono deve essere riprodotto in loop
    public bool loop = false;

    //Riferimento all'AudioSource
    public AudioSource source;

    public Sound()
    {
        volume = 1;
        pitch = 1;
        loop = false;
    }
}

//La classe AudioManager gestisce la riproduzione e l'arresto di suoni attraverso la creazione di oggetti AudioSource.
public class AudioManager : MonoBehaviour
{
    //Array di suoni
    public Sound[] sounds;

    //Istanza dell'AudioManager statica per garantire che sia unica
    public static AudioManager instance;

    //Metodo per inizializzare l'AudioManager
    void Awake()
    {
        //Garantisce che ci sia una sola istanza dell'AudioManager
        if (instance == null)
            instance = this;
        else
        {
            //Se esiste già un'istanza dell'AudioManager, distrugge l'oggetto corrente
            Destroy(gameObject);
            return;
        }

        //Mantiene l'AudioManager tra le scene
        DontDestroyOnLoad(gameObject);

        //Per ogni suono nell'array, crea un oggetto AudioSource e gli assegna i valori del suono
        foreach (Sound s in sounds)
        {
            if(!s.source)
                s.source = gameObject.AddComponent<AudioSource>();

            //Imposta la clip audio, il volume, il pitch e se deve essere riprodotto in loop
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //Metodo per riprodurre un suono dato il nome
    public void Play(string name)
    {
        //Trova il suono con il nome dato
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            //Stampa un messaggio di errore se il suono non è stato trovato
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        //Riproduce il suono
        s.source.Play();
    }

    //Metodo per fermare la riproduzione di un suono dato il nome
    public void Stop(string name)
    {
        //Trova il suono con il nome dato
        Sound s = Array.Find(sounds, sound => sound.name == name);

        //Ferma la riproduzione del suono
        s.source.Stop();
    }
}