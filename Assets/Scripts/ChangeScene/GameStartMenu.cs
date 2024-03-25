using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Lo script GameStartMenu gestisce il menu iniziale del gioco, consentendo la navigazione tra diverse pagine dell'interfaccia utente.
public class GameStartMenu : MonoBehaviour
{
    //Pagine dell'interfaccia utente
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    //Pulsanti del menu principale
    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        //Abilita il menu principale all'avvio
        EnableMainMenu();

        //Aggiunge i listener ai pulsanti
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    //Metodo per uscire dal gioco
    public void QuitGame()
    {
        Application.Quit();
    }

    //Metodo per iniziare il gioco, nascondendo il menu e caricando la scena di gioco
    public void StartGame()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    //Metodo per nascondere tutte le pagine dell'interfaccia utente
    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    //Metodi per abilitare il menu principale e nascondere le altre pagine
    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }

    //Metodo per abilitare la pagina delle opzioni e nascondere le altre pagine
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }

    //Metodo per abilitare la pagina delle informazioni e nascondere le altre pagine
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
}
