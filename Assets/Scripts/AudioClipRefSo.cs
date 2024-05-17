using UnityEngine;

namespace AshLight.BakerySim
{
    [CreateAssetMenu(fileName = "AudioClip_new", menuName = "ScriptableObject/AudioClipRef")]
    public class AudioClipRefSo : ScriptableObject
    {
        public AudioClip[] audioClips;

        public AudioClip GetRandomClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}