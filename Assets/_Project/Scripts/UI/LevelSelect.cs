using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchThreeGame._Project.Scripts.UI
{
    public class LevelSelect : MonoBehaviour
    {
        [SerializeField] private ButtonPlayerPrefs[] buttons;
        public void OnButtonPress(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        private void Start()
        {
            foreach (var button in buttons)
            {
                var score = PlayerPrefs.GetInt(button.playerPrefKey, 0);

                for (var i = 1; i <= 3; i++)
                {
                    var star = button.gameObject.transform.Find("star" + i);
                    star.gameObject.SetActive(i <= score);
                }
            }
        }
    }
}