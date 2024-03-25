using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Lo script SceneTransitionManager gestisce le transizioni tra le scene del gioco, inclusa la dissolvenza dello schermo.
public class SceneTransitionManager : MonoBehaviour
{
    //Riferimento all'oggetto FadeScreen per la dissolvenza
    public FadeScreen fadeScreen;
    
    //Singleton per garantire un'unica istanza di SceneTransitionManager
    public static SceneTransitionManager singleton;

    //Metodo chiamato durante l'inizializzazione dell'oggetto
    private void Awake()
    {
        if (singleton && singleton != this)
            Destroy(singleton);
        singleton = this;
    }

    //Metodo per avviare la transizione verso una nuova scena
    public void GoToScene(int sceneIndex)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        //Attiva la dissolvenza in uscita, dopo tot secondi carica la nuova scena
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        SceneManager.LoadScene(sceneIndex);
    }

    //Metodo per avviare la transizione verso una nuova scena in maniera asincrona
    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    //Coroutine per avviare la transizione verso una nuova scena in maniera asincrona
    IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        //Attiva la dissolvenza in uscita, dopo tot secondi carica la nuova scena
        fadeScreen.FadeOut();

        //Aspetta che la dissolvenza sia terminata
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0;
        while(timer <= fadeScreen.fadeDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        //Attiva la nuova scena
        operation.allowSceneActivation = true;
    }
}
