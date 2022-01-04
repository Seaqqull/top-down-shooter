﻿using UnityEngine;


namespace Kool2Play.Utility.Events
{
    public abstract class ActionBoolBehaviour : MonoBehaviour, Data.IAction<bool>
    {
        [SerializeField] private bool _singleExecution = true;

        private bool _wasExecuted;

        public bool SingleExecution
        {
            get { return _singleExecution; }
        }
        public bool WasExecuted
        {
            get { return _wasExecuted; }
        }


        protected abstract bool CheckPossibility();


        public bool Perform()
        {

            if (_wasExecuted && _singleExecution) return false;

            if (!CheckPossibility())
                return false;

            _wasExecuted = true;
            return true;
        }
    }
}
