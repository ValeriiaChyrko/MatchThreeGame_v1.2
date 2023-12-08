using System;
using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;
using UnityEngine.Serialization;

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
            
            Debug.Log("Time: " + timeInSeconds + " seconds." +
                      "Target score " + TargetScore);
        }

        private void Update()
        {
            if (_isTimeOut) return;
            
            _timer += Time.deltaTime;
            
            if (_timer - timeInSeconds <= 0) return;
            
            if (Score >= (LevelType)targetScore)
                GameWin();
            else
                GameLose();
        }
    }
}