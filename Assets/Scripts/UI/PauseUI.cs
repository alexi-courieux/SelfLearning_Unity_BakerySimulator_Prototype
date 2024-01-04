using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameResumed += GameManager_OnGameResumed;
        
        resumeButton.onClick.AddListener(GameManager.Instance.TogglePause);
        optionsButton.onClick.AddListener(() => Debug.Log("Options"));
        mainMenuButton.onClick.AddListener(() => Debug.Log("Main Menu"));
        quitButton.onClick.AddListener(Application.Quit);
        
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.OnGamePaused -= GameManager_OnGamePaused;
        GameManager.Instance.OnGameResumed -= GameManager_OnGameResumed;
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
    }
    
    private void GameManager_OnGameResumed(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
