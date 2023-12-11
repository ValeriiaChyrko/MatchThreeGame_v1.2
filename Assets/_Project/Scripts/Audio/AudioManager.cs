using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace MatchThreeGame._Project.Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {
        public static AudioManager Instance { get; private set; }
        [SerializeField] private Transform soundContainer;
        [SerializeField] private AudioMixerGroup audioMixer;
        [SerializeField] private AudioSource backgroundMusic;
        
        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip deselect;
        [SerializeField] private AudioClip match;
        [SerializeField] private AudioClip noMatch;
        [SerializeField] private AudioClip woosh;
        [SerializeField] private AudioClip pop;

        private readonly List<AudioSource> _soundSources = new List<AudioSource>();
        private List<AudioClip> _musicClip = new List<AudioClip>();

        private Coroutine _controlMusicRoutine;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private AudioSource _audioSource;
        
        private void OnValidate() {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        }

        public void PlayClick() => _audioSource.PlayOneShot(click);
        public void PlayDeselect() => _audioSource.PlayOneShot(deselect);
        public void PlayMatch() => _audioSource.PlayOneShot(match);
        public void PlayNoMatch() => _audioSource.PlayOneShot(noMatch);
        public void PlayWoosh() => PlayRandomPitch(woosh);
        public void PlayPop() => PlayRandomPitch(pop);

        private void PlayRandomPitch(AudioClip audioClip) {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(audioClip);
            _audioSource.pitch = 1f;
        }
        
        public void PlayOnBackground(List<AudioClip> clips)
        {
            _musicClip = clips;
            backgroundMusic.clip = clips[0];
            backgroundMusic.Play();

            if (_controlMusicRoutine != null)
                StopCoroutine(_controlMusicRoutine);
            _controlMusicRoutine = StartCoroutine(ControlMusicRoutine());
        }
        
        private IEnumerator ControlMusicRoutine()
        {
            var musicIndex = 0;

            while (true)
            {
                yield return null;

                if (backgroundMusic.isPlaying != false) continue;
                
                musicIndex++;
                if (musicIndex >= _musicClip.Count)
                    musicIndex = 0;

                backgroundMusic.clip = _musicClip[musicIndex];
                backgroundMusic.Play();
            }
        }

        private AudioSource GetFreeSoundSource()
        {
            var freeAudioSource = _soundSources.Find(s => s.isPlaying == false);

            if (freeAudioSource != null)
                return freeAudioSource;

            freeAudioSource = soundContainer.AddComponent<AudioSource>();
            freeAudioSource.outputAudioMixerGroup = audioMixer;
            _soundSources.Add(freeAudioSource);
            return freeAudioSource;
        }
    }
}