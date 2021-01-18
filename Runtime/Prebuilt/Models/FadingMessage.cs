using System;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Models;

namespace VanillaWorks.MessageLogger.Runtime.Prebuilt.Models
{
    [Serializable]
    public class FadingMessage : MessageData
    {
        
        private const float DefaultLifetime = 10f;
        
        [SerializeField] private float _startingTime;
        [SerializeField] private float _lifeTime;
        [SerializeField] private float _stoppedTime;
        [SerializeField] private bool _isTimerStopped;
        [SerializeField] private Color _color = Color.white;


        public float Age => Time.realtimeSinceStartup - _startingTime;
        public float RemainingTime => _isTimerStopped ? 1f : _lifeTime - Age;
        public bool IsExpired => RemainingTime < 0.0f;
        public float Alpha => RemainingTime < 0.6f ? RemainingTime : 1f;

        public Color TextColor
        {
            get
            {
                _color.a = Alpha;
                return _color;
            }
        }
        
        public FadingMessage(string messageText, float life = 0f) : base(messageText)
        {
            _lifeTime = DefaultLifetime;
            if (life != 0.0f)
                _lifeTime = life;
            _isTimerStopped = false;
        }

        public FadingMessage(string messageText, Color color, float life = 0f) : this(messageText, life) =>
            _color = color;
        
        
        public void Activate()
        {
            _startingTime = Time.realtimeSinceStartup;
        }

        public void StopTimer()
        {
            _isTimerStopped = true;
            _stoppedTime = Time.realtimeSinceStartup;
        }

        public void ResumeTimer()
        {
            _isTimerStopped = false;
            _startingTime += Time.realtimeSinceStartup - _stoppedTime;
        }
    }
}