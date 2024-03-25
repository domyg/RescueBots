using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SetOptionFromUI : MonoBehaviour
{
    //Riferimento allo slider del volume
    public Scrollbar volumeSlider;

    private void Start()
    {
        //Aggiunge il listener allo slider del volume
        volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
    }

    //Imposta il volume globale al valore impostato dallo slider
    public void SetGlobalVolume(float value)
    {
        AudioListener.volume = value;
    }
}
