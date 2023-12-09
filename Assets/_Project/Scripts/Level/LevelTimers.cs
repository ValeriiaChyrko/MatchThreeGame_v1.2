using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.Level
{
    public class LevelTimers : LevelController
    {
        [SerializeField] private int timeInSeconds;
        [SerializeField] private int targetScore;
        
        public int TimeInSeconds => timeInSeconds;
        public int TargetScore => targetScore;

        private float _timer;
        private bool _isTimeOut;

        private void Start()
        {
            Type = LevelType.TIMER;
            
            levelHUD.SetLevelType(Type);
            levelHUD.SetScore((int)Score);
            levelHUD.SetTarget(TargetScore);
            levelHUD.SetRemaining($"{timeInSeconds / 60}:{timeInSeconds % 60:00}");
        }

        private void Update()
        {
            if (_isTimeOut) return;
            
            _timer += Time.deltaTime;
            levelHUD.SetRemaining($"{Mathf.Max(0, timeInSeconds - _timer) / 60:0}:{Mathf.Max(0, timeInSeconds - _timer) % 60:00}");
                
            if (_timer - timeInSeconds <= 0) return;
            
            if (Score >= (LevelType)targetScore)
                GameWin();
            else
                GameLose();
        }
    }
}