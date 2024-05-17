using System;
using UnityEngine;

namespace AshLight.BakerySim
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public EventHandler OnGamePaused;
        public EventHandler OnGameResumed;

        private bool _isGamePaused;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InputManager.Instance.OnPause += InputManager_OnPause;
        }

        private void InputManager_OnPause(object sender, EventArgs e)
        {
            TogglePause();
        }

        public void TogglePause()
        {
            _isGamePaused = !_isGamePaused;
            if (_isGamePaused)
            {
                Time.timeScale = 0;
                OnGamePaused?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1;
                OnGameResumed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}