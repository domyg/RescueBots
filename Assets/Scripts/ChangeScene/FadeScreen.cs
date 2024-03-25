using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

//Lo script FadeScreen gestisce l'effetto di dissolvenza su uno schermo utilizzando un materiale Renderer.
public class FadeScreen : MonoBehaviour
{
    //Opzione per avviare la dissolvenza all'avvio
    public bool fadeOnStart = true;

    //Durata della dissolvenza
    public float fadeDuration = 2;

    //Colore della dissolvenza
    public Color fadeColor;

    //Curva di dissolvenza
    public AnimationCurve fadeCurve;

    //Nome della proprietà del colore del materiale
    public string colorPropertyName = "_Color";

    //Riferimento al Renderer
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        //Ottiene il componente Renderer
        rend = GetComponent<Renderer>();

        //Disabilita il rendering iniziale
        rend.enabled = false;

        //Avvia la dissolvenza all'avvio
        if (fadeOnStart)
            FadeIn();
    }

    //Metodo per avviare la dissolvenza in entrata
    public void FadeIn()
    {
        Fade(1, 0);
    }
    
    //Metodo per avviare la dissolvenza in uscita
    public void FadeOut()
    {
        Fade(0, 1);
    }

    //Metodo generico per avviare una dissolvenza con specifici valori di opacità in ingresso e in uscita
    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn,alphaOut));
    }

    //Coroutine per gestire la dissolvenza nel tempo
    public IEnumerator FadeRoutine(float alphaIn,float alphaOut)
    {
        //Abilita il rendering
        rend.enabled = true;

        //Timer per la dissolvenza
        float timer = 0;

        //Finché il timer non raggiunge la durata della dissolvenza, aggiorna il colore del materiale
        while(timer <= fadeDuration)
        {
            //Calcola il nuovo colore in base al timer e alla curva di dissolvenza
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, fadeCurve.Evaluate(timer / fadeDuration));

            //Aggiorna il colore del materiale
            rend.material.SetColor(colorPropertyName, newColor);

            //Aggiorna il timer
            timer += Time.deltaTime;
            yield return null;
        }

        //Aggiorna il colore del materiale all'ultimo frame
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor(colorPropertyName, newColor2);

        //Disabilita il rendering se la dissolvenza è in uscita
        if(alphaOut == 0)
            rend.enabled = false;
    }
}
