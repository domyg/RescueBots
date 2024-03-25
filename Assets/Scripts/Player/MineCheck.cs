using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MineCheck : MonoBehaviour
{
    //Riferimento al menu di morte
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject head;
    void Update()
    {
        transform.position = head.transform.position;
    }
    private void OnTriggerEnter(Collider collider)
    {
        //Se il giocatore entra in contatto con una mina, viene mostrato il menu di morte
        if (collider.gameObject.tag == "Mine")
        {
            GetComponent<AudioSource>().Play();
            deathMenu.SetActive(true);

            //SceneTransitionManager.singleton.GoToSceneAsync(0);
        }
    }
}
