using System;
using UnityEngine;

namespace Nebuloic
{
    public class Timer 
    {
        
        private float _timeInSeconds;

        public Timer(float timeInSeconds) { 
            TimeInSeconds = timeInSeconds;
        }

        public float TimeInSeconds { get => _timeInSeconds; private set => _timeInSeconds = Mathf.Max(value, 0); }
        public event Action OnTimerEnd;


        public void Update(float deltaTime) {
            TimeInSeconds -= deltaTime;
            if (TimeInSeconds <= 0) {
                OnTimerEnd?.Invoke();
            }
        }


    }
}