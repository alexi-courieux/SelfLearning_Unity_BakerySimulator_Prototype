using UnityEngine;

namespace DefaultNamespace
{
    public class BlenderStationSound : MonoBehaviour
    {
        [SerializeField] private AudioClipRefsDictionarySo audioClipRefsDictionarySo;
        [SerializeField] private BlenderStation blenderStation;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            blenderStation.OnStateChanged += BlenderStation_OnStateChanged;
            _audioSource.loop = true;
            _audioSource.clip = audioClipRefsDictionarySo.blender.GetRandomClip();
        }

        private void BlenderStation_OnStateChanged(object sender, BlenderStation.State state)
        {
            bool playSound = state is BlenderStation.State.Processing;
            if (playSound)
            {
                PlayBlenderSound();
            }
            else
            {
                StopBlenderSound();
            }
        }

        private void PlayBlenderSound()
        {
            _audioSource.Play();
        }
        
        private void StopBlenderSound()
        {
            _audioSource.Stop();
        }

    }
}