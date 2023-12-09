using MatchThreeGame._Project.Scripts.Enums;
using MatchThreeGame._Project.Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MatchThreeGame._Project.Scripts.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private LevelController level;
        [SerializeField] private GameOverScreen gameOverScreen;
            
        [SerializeField] private TextMeshProUGUI remainingText;
        [SerializeField] private TextMeshProUGUI remainingSubText;
        [SerializeField] private TextMeshProUGUI targetText;
        [SerializeField] private TextMeshProUGUI targetSubText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image[] stars;

        private int _starIdx;

        private void Start()
        {
            for (var i = 0; i < stars.Length; i++)
                stars[i].enabled = i == 0;
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();

            var visibleStar = 0;

            if (score >= level.ScoreOneStar && score < level.ScoreTwoStar)
                visibleStar = 1;
            else if (score >= level.ScoreTwoStar && score < level.ScoreThreeStar)
                visibleStar = 2;
            else if (score >= level.ScoreThreeStar)
                visibleStar = 3;
            
            for (var i = 0; i < stars.Length; i++)
                stars[i].enabled = i == visibleStar;

            _starIdx = visibleStar;
        }

        public void SetTarget(int target)
        {
            targetText.text = target.ToString();
        }
        
        public void SetRemaining(int remaining)
        {
            remainingText.text = remaining.ToString();
        }
        
        public void SetRemaining(string remaining)
        {
            remainingText.text = remaining;
        }
        
        public void SetLevelType(LevelType type)
        {
            switch (type)
            {
                case LevelType.MOVES:
                    remainingSubText.text = "moves remaining";
                    targetSubText.text = "target score";
                    break;
                case LevelType.TIMER:
                    remainingSubText.text = "times remaining";
                    targetSubText.text = "target score";
                    break;
            }
        }

        public void OnGameWin(int score)
        {
            gameOverScreen.ShowWin(score, _starIdx);
            if (_starIdx > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, _starIdx);
        }
        
        public void OnGameLose(int score)
        {
            gameOverScreen.ShowLose();
        }
        
    }
}
