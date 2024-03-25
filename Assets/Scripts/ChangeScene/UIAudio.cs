using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Lo script UIAudio gestisce la riproduzione di suoni associati a interazioni utente (click e hover) su elementi UI.
public class UIAudio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //Nomi dei file audio per le diverse interazioni
    public string clickAudioName;
    public string hoverEnterAudioName;
    public string hoverExitAudioName;

    //Metodo per la riproduzione di un suono di click
    public void OnPointerClick(PointerEventData eventData)
    {
        if(clickAudioName != "")
        {
            AudioManager.instance.Play(clickAudioName);
        }
    }

    //Metodo per la riproduzione di un suono di hover (quando il cursore del mouse passa sopra un elemento UI)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverEnterAudioName != "")
        {
            AudioManager.instance.Play(hoverEnterAudioName);
        }
    }

    //Metodo per la riproduzione di un suono di hover (quando il cursore del mouse esce da un elemento UI)
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverExitAudioName != "")
        {
            AudioManager.instance.Play(hoverExitAudioName);
        }
    }
}
