using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.Level
{
    public class LevelMoves : LevelController
    {
        [SerializeField] private int targetScore;
        [SerializeField] private int movesAmount;

        public int MovesAmount => movesAmount;
        public int TargetScore => targetScore;

        private int _usedMovesAmount;

        private void Start()
        {
            Type = LevelType.MOVES;
            
            levelHUD.SetLevelType(Type);
            levelHUD.SetScore((int)Score);
            levelHUD.SetTarget(TargetScore);
            levelHUD.SetRemaining(movesAmount);
        }

        public override void OnMove()
        {
            _usedMovesAmount++;
            levelHUD.SetRemaining((movesAmount - _usedMovesAmount));
            
            if (movesAmount - _usedMovesAmount != 0) return;
            
            if (Score >= (LevelType)targetScore)
                GameWin();
            else
                GameLose();
        }
    }
}