using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MatchThreeGame._Project.Scripts.UI
{
    [RequireComponent(typeof(Animation))]
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject screenParent;
        [SerializeField] private GameObject screenGrid;
        [SerializeField] private TextMeshProUGUI loseText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image[] stars;

        private void Start()
        {
            screenParent.SetActive(false);
            
            foreach (var star in stars)
                star.enabled = false;
        }

        public void ShowLose()
        {
            screenGrid.SetActive(false);
            screenParent.SetActive(true);
            scoreText.enabled = false;
            
            GetComponent<Animation>().Play();
        }
        
        public void ShowWin(int score, int starCount)
        {
            screenGrid.SetActive(false);
            screenParent.SetActive(true);
            loseText.enabled = false;

            scoreText.text = score.ToString();
            scoreText.enabled = false;
            
            GetComponent<Animation>().Play();

            StartCoroutine(ShowWinCoroutine(starCount));
        }

        private IEnumerator ShowWinCoroutine(int starCount)
        {
            scoreText.enabled = true;
            yield return new WaitForSeconds(0.5f);

            if (starCount == 0) stars[0].enabled = true;
            if (starCount > stars.Length) yield break;
            for (var i = 1; i <= starCount; i++)
            {
                stars[i].enabled = true;
                stars[i-1].enabled = false;
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void OnReplayClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void OnNextClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}