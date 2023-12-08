using MatchThreeGame._Project.Scripts.Enums;
using MatchThreeGame._Project.Scripts.GridPiece;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.Level
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GridController grid;
        
        [SerializeField] private int scoreOneStar;
        [SerializeField] private int scoreTwoStar;
        [SerializeField] private int scoreThreeStar;
        
        public LevelType Type { get; private set; }
        public LevelType Score { get; private set; }

        public virtual void GameWin()
        {
            Debug.Log("Player win!");
            grid.GameOver();
        }

        public virtual void GameLose()
        {
            Debug.Log("Player lose!");
        }
        
        public virtual void OnMove()
        {
            Debug.Log("Player swap!");
        }
        
        public virtual void OnPieceCleared(Piece piece)
        {
            Score += piece.Score;
            Debug.Log("Score: " + Score);
        }
    }
}
