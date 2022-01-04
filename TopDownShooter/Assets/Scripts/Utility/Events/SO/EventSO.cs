using UnityEngine;


namespace TopDownShooter.Utility.Events
{
    public abstract class EventSO : ScriptableObject, Data.IListener
    {
        protected int _callsCount;

        public int CountOfCalls
        {
            get { return _callsCount; }
        }
        public bool WasExecuted
        { 
            get { return (_callsCount != 0); } 
        }


        protected abstract void OnRaise();


        public virtual void OnEvent()
        {
            OnRaise();
        }

        public virtual void OnEvent(Data.EventType eventType)
        {
            OnEvent();

            _callsCount++;
        }
    }
}
