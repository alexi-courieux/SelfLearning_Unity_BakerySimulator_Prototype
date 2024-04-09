using UnityEngine;

namespace DefaultNamespace
{
    public class PhoneStationSound : MonoBehaviour
    {
        [SerializeField] private AudioClipRefsDictionarySo audioClipRefsDictionarySo;
        [SerializeField] private PhoneStation phoneStation;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
            GameManager.Instance.OnGameResumed += GameManager_OnGameResumed;
            
            phoneStation.OnStateChange += PhoneStation_OnStateChanged;
            _audioSource.loop = true;
            _audioSource.clip = audioClipRefsDictionarySo.phone.GetRandomClip();
        }

        private void PhoneStation_OnStateChanged(object sender, PhoneStation.State state)
        {
            bool playSound = state is PhoneStation.State.Ringing;
            if (playSound)
            {
                PlayPhoneSound();
            }
            else
            {
                StopPhoneSound();
            }
        }

        private void PlayPhoneSound()
        {
            _audioSource.Play();
        }
        
        private void StopPhoneSound()
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