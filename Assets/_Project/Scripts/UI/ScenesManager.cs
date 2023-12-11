using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchThreeGame._Project.Scripts.UI
{
    public class ScenesManager : MonoBehaviour
    {
        private static ScenesManager Instance { get; set; }
        public static string CurrentSceneName { get; set; } = "Level 1";

        private const string MAIN_MENU_SCENE_KEY = "MainMenu";
        private const string SETTINGS_SCENE_KEY = "MusicSettingsMenu";
        private const string LEVEL_SELECTOR_SCENE_KEY = "LevelSelection";

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private static void LoadScene(string sceneKey, bool additive = false)
        {
            if (additive)
                SceneManager.LoadScene(sceneKey, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(sceneKey);
            
            SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        }

        public static void LoadMainMenu() => LoadScene(MAIN_MENU_SCENE_KEY);
        public static void UnloadMainMenu() => SceneManager.UnloadSceneAsync(MAIN_MENU_SCENE_KEY);
        
        public static void LoadSettingsMenu() => LoadScene(SETTINGS_SCENE_KEY);
        
        public static void LoadLevelSelectorMenu() => LoadScene(LEVEL_SELECTOR_SCENE_KEY);
        
        public static void LoadCurrentLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        public static void ClickBack()
        {
            if (SceneManager.sceneCount > 1)
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            else
                Application.Quit();
        }

        public static void ClickPlay() => SceneManager.LoadScene(CurrentSceneName);
    }
}