using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    //Riferimento al pulsante "Quit" del menu di morte
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        //Aggiunge il listener al pulsante "Quit"
        quitButton.onClick.AddListener(QuitGame);
    }

    //Quando viene premuto il pulsante "Quit" del menu di morte viene caricata la StartScene
    public void QuitGame()
    {
        SceneTransitionManager.singleton.GoToSceneAsync(0);
    }
}