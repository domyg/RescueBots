using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//Lo script seguente è responsabile per impostare il tipo di movimento (snap o continuo) in base alle preferenze del giocatore.
public class SetTurnTypeFromPlayerPref : MonoBehaviour
{
    //Riferimenti ai provider di movimento snap e continuo nell'Editor di Unity
    public ActionBasedSnapTurnProvider snapTurn;
    public ActionBasedContinuousTurnProvider continuousTurn;

    // Start is called before the first frame update
    void Start()
    {
        ApplyPlayerPref();
    }

    //Metodo per applicare le preferenze del giocatore per il tipo di movimento
    public void ApplyPlayerPref()
    {
        // Verifica se PlayerPrefs contiene la chiave "turn"
        if (PlayerPrefs.HasKey("turn"))
        {
            //Se il valore è 0, abilita il movimento snap e disabilita il movimento continuo
            int value = PlayerPrefs.GetInt("turn");
            if(value == 0)
            {
                snapTurn.leftHandSnapTurnAction.action.Enable();
                snapTurn.rightHandSnapTurnAction.action.Enable();
                continuousTurn.leftHandTurnAction.action.Disable();
                continuousTurn.rightHandTurnAction.action.Disable();
            }
            //Se il valore è 1, abilita il movimento continuo e disabilita il movimento snap
            else if(value == 1)
            {
                snapTurn.leftHandSnapTurnAction.action.Disable();
                snapTurn.rightHandSnapTurnAction.action.Disable();
                continuousTurn.leftHandTurnAction.action.Enable();
                continuousTurn.rightHandTurnAction.action.Enable();
            }
        }
    }
}
