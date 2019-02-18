using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public RPGTalk RpgTalk;
    public GameController GameController;
    public Image NameInput;

    private static DialogManager _instance;
    private bool hasPlayedIntro, hasPlayedFirstManager, hasPlayedSecondManager;
    private string playerName;
    private Store _store;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _store = FindObjectOfType<Store>();
        NameInput.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameController.IsGamePaused)
        {
            return;
        }

        if (!hasPlayedIntro && GameController.SecondsSinceStart > 5)
        {
            IntroDialog();
            hasPlayedIntro = true;
        }

        if (!hasPlayedFirstManager && GameController.SecondsSinceStart > 10)
        {
            FirstManagerDialog();
            hasPlayedFirstManager = true;
        }

        if (!hasPlayedSecondManager && GameController.SecondsSinceStart > 15)
        {
            SecondManagerDialog();
            hasPlayedSecondManager = true;
        }
    }

    public void InvokeDialog(string dialog)
    {
        Invoke(dialog, 0f);
    }

    public void IntroDialog()
    {
        GameController.PauseGame(true);
        RpgTalk.lineToStart = 2;
        RpgTalk.lineToBreak = 3;
        RpgTalk.callbackScript = this;
        RpgTalk.callbackFunction = "GetPlayerName";
        RpgTalk.NewTalk();
    }

    public void GetPlayerName()
    {
        NameInput.gameObject.SetActive(true);
        var inputField = NameInput.GetComponentInChildren<InputField>();
        inputField.Select();
        var submitEvent = new InputField.SubmitEvent();
        submitEvent.AddListener((value) => IdentifyPlayerDialog(value));
        inputField.onEndEdit = submitEvent;
    }

    public void IdentifyPlayerDialog(string playerName)
    {
        NameInput.gameObject.SetActive(false);
        RpgTalk.variables[0].variableValue = playerName;
        RpgTalk.lineToStart = 4;
        RpgTalk.lineToBreak = 4;
        RpgTalk.callbackScript = this;
        RpgTalk.callbackFunction = "ResumeGame";
        RpgTalk.NewTalk();
    }

    public void FirstManagerDialog()
    {
        GameController.PauseGame(true);
        RpgTalk.lineToStart = 7;
        RpgTalk.lineToBreak = 11;
        RpgTalk.callbackScript = this;
        RpgTalk.callbackFunction = "ResumeGame";
        RpgTalk.NewTalk();
    }

    public void SecondManagerDialog()
    {
        GameController.PauseGame(true);
        RpgTalk.lineToStart = 14;
        RpgTalk.lineToBreak = 23;
        RpgTalk.callbackScript = this;
        RpgTalk.callbackFunction = "ResumeGame";
        RpgTalk.NewTalk();
    }

    public void FirstBossDialog()
    {
        GameController.PauseGame(true);
        RpgTalk.lineToStart = 26;
        RpgTalk.lineToBreak = 30;
        RpgTalk.callbackScript = this;
        RpgTalk.callbackFunction = "ResumeGame";
        RpgTalk.NewTalk();
    }


    public void ResumeGame()
    {
        if (!GameController.IsGameOver && !_store.IsOpen)
        {
            GameController.PauseGame(false);
        }
    }


}
