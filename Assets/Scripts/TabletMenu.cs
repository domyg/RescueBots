using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletMenu : MonoBehaviour
{
    //Quando viene premuto il pulsante "Exit" del Tablet viene caricata la StartScene
    public void ExitGame()
    {
        SceneTransitionManager.singleton.GoToSceneAsync(0);
    }
}
