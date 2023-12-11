using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MatchThreeGame._Project.Scripts.UI.Menus
{
    public class MusicSettings: MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider soundsSlider;
        [SerializeField] private Slider musicSlider;

        private const string MASTER_VOLUME_KEY = "master_volume";
        private const string SOUNDS_VOLUME_KEY = "sounds_volume";
        private const string MUSIC_VOLUME_KEY = "music_volume";

        private void Awake()
        {
            Time.timeScale = 0;
        }
        
        private void Start()
        {
            masterSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            soundsSlider.value = PlayerPrefs.GetFloat(SOUNDS_VOLUME_KEY, 1f);
            musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        }

        public void ChangeMasterVolume(float volume)
        {
            audioMixer.SetFloat("master-volume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
        }
        
        public void ChangeSoundsVolume(float volume)
        {
            audioMixer.SetFloat("sound-volume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat(SOUNDS_VOLUME_KEY, volume);
        }
        
        public void ChangeMusicVolume(float volume)
        {
            audioMixer.SetFloat("music-volume", Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        }
    }
}