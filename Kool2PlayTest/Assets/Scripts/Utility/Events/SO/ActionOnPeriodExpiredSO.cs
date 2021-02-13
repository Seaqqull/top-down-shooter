using UnityEngine;
using System;


namespace Kool2Play.Utility.Events
{
    [CreateAssetMenu(menuName = "Action/OnPeriodExpired")]
    public class ActionOnPeriodExpiredSO : ActionBoolSO
    {
        [SerializeField] [Range(0.0f, short.MaxValue)] private float _period;

        private bool _isInitialized;

        private TimeSpan _timeFromExecution;
        private TimeSpan _currentTime;

        private float _periodBaked;


        protected override bool CheckPossibility()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                _timeFromExecution = new TimeSpan(System.DateTime.Now.Ticks);
                _periodBaked = 1000 * _period;

                return false;
            }

            _currentTime = new TimeSpan(System.DateTime.Now.Ticks);

            if (!((_currentTime - _timeFromExecution).TotalMilliseconds >= _periodBaked))
                return false;

            _timeFromExecution = _currentTime;

            return true;
        }
    }
}
