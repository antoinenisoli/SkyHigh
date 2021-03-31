using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Serializable]
    class Sound
    {
        public string soundType;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 0.5f;
        [Range(-1, 3)]
        public float pitch = 1;
        public float radius = 20;

        public Sound(AudioClip clip = default, string soundType = "SoundName", float volume = 0.3f, float pitch = 1)
        {
            this.soundType = soundType;
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
        }
    }

    public static SoundManager Instance;
    [SerializeField] List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] List<Sound> sounds = new List<Sound>();
    Dictionary<string, Sound> soundsLibrary = new Dictionary<string, Sound>();
    AudioSource lastSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var s in sounds)
            if (!soundsLibrary.ContainsKey(s.soundType.ToString()))
                soundsLibrary.Add(s.soundType.ToString(), s);
    }

    [ContextMenu(nameof(Convert))]
    public void Convert()
    {
        sounds.Clear();
        foreach (var item in clips)
        {
            Sound newSound = new Sound(item, item.name);
            sounds.Add(newSound);
        }

        clips.Clear();
    }

    public void PlayAudio(string name, bool destroy = true, float destructionDelay = default, float spatialBlend = 0, float soundRadius = default)
    {
        if (soundsLibrary.TryGetValue(name, out Sound thisSound))
        {
            GameObject sound = new GameObject
            {
                name = thisSound.clip.name
            };

            sound.transform.parent = transform;
            if (lastSource != null && lastSource.clip == thisSound.clip && lastSource.isPlaying)
                Destroy(lastSource.gameObject);

            lastSource = sound.AddComponent<AudioSource>();
            lastSource.clip = thisSound.clip;
            lastSource.volume = thisSound.volume;
            lastSource.pitch = thisSound.pitch;
            lastSource.dopplerLevel = 0;
            lastSource.spatialBlend = spatialBlend;
            lastSource.minDistance = soundRadius;
            lastSource.Play();

            if (destroy)
            {
                SelfDestroySFX selfDestroy = sound.AddComponent<SelfDestroySFX>();
                selfDestroy.Execute(destructionDelay);
            }
        }
        else if (string.IsNullOrEmpty(name))
            Debug.LogError("This name is invalid.");
        else
            Debug.LogError("There is no song at this name : " + name);
    }
}
