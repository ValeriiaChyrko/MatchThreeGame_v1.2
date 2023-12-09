using System.Collections;
using MatchThreeGame._Project.Scripts.Enums;
using MatchThreeGame._Project.Scripts.GridPiece;
using MatchThreeGame._Project.Scripts.UI;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.Level
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GridController grid;
        
        [SerializeField] private int scoreOneStar;
        [SerializeField] private int scoreTwoStar;
        [SerializeField] private int scoreThreeStar;
        
        [SerializeField] public HUD levelHUD;

        private bool _isWin;

        public int ScoreOneStar => scoreOneStar;
        public int ScoreTwoStar => scoreTwoStar;
        public int ScoreThreeStar => scoreThreeStar;

        protected LevelType Type { get; set; }
        protected LevelType Score { get; set; }

        private void Start()
        {
            levelHUD.SetScore((int)Score);
        }

        protected virtual void GameWin()
        {
            _isWin = true;
            grid.GameOver();
            StartCoroutine(WaitForGridFill());
        }

        protected virtual void GameLose()
        {
            _isWin = false;
            grid.GameOver();
            StartCoroutine(WaitForGridFill());
        }
        
        public virtual void OnMove()
        {
            Debug.Log("Player swap!");
        }
        
        public virtual void OnPieceCleared(Piece piece)
        {
            Score += piece.Score;
            levelHUD.SetScore((int)Score);
        }

        protected IEnumerator WaitForGridFill()
        {
            while (grid.IsFilling)
                yield return 0;

            if (_isWin)
                levelHUD.OnGameWin((int)Score);
            else
                levelHUD.OnGameLose((int)Score);
        }
    }
}
