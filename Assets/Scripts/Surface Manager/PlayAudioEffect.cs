using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.ImpactSystem.Effects
{
    [CreateAssetMenu(menuName = "Impact System/Play Audio Effect", fileName = "PlayAudioEffect")]
    public class PlayAudioEffect : ScriptableObject
    {
        public AudioSource AudioSourcePrefab;
        public List<AudioClip> AudioClips = new List<AudioClip>();
        [Tooltip("Values are clamped to 0-1")]
        public Vector2 VolumeRange = new Vector2(0, 1);
    }
}