using UnityEngine;

namespace DefaultNamespace
{
    public class OvenStationSound : MonoBehaviour
    {
        [SerializeField] private AudioClipRefsDictionarySo audioClipRefsDictionarySo;
        [SerializeField] private OvenStation ovenStation;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
            GameManager.Instance.OnGameResumed += GameManager_OnGameResumed;
            
            ovenStation.OnStateChanged += OvenStation_OnStateChanged;
            _audioSource.loop = true;
            _audioSource.clip = audioClipRefsDictionarySo.oven.GetRandomClip();
        }

        private void OvenStation_OnStateChanged(object sender, OvenStation.State state)
        {
            bool playSound = state is OvenStation.State.Processing or OvenStation.State.Burning;
            if (playSound)
            {
                PlayOvenSound();
            }
            else
            {
                StopOvenSound();
            }
        }

        private void PlayOvenSound()
        {
            _audioSource.Play();
        }
        
        private void StopOvenSound()
        {
            _audioSource.Stop();
        }
        
        private void GameManager_OnGamePaused(object sender, System.EventArgs e)
        {
            _audioSource.Pause();
        }
        
        private void GameManager_OnGameResumed(object sender, System.EventArgs e)
        {
            _audioSource.UnPause();
        }
    }
}