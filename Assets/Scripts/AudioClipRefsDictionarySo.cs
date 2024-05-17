using UnityEngine;

namespace AshLight.BakerySim
{
    [CreateAssetMenu(fileName = "_AudioClipRefsSo", menuName = "ScriptableObject/_AudioClipRefDictionary", order = 0)]
    public class AudioClipRefsDictionarySo : ScriptableObject
    {
        public AudioClipRefSo oven;
        public AudioClipRefSo blender;
        public AudioClipRefSo phone;
    }
}