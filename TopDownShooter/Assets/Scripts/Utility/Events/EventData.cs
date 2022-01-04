using UnityEngine.Events;
using System;


namespace Kool2Play.Utility.Events.Data
{
    [Serializable]
    public enum EventType { None, GameInit, GameEnd, RoomLoad, RoomPreInit, RoomInit, RoomEnd, RoomClear, ConditionalTrigger }

    public interface IListener
    {
        int CountOfCalls { get; }
        bool WasExecuted { get; }


        void OnEvent(EventType eventType);
    }

    public interface IEventListener
    {
        UnityEvent Response { get; }
        EventSO Event { get; }
        

        void OnEnable();

        void OnDisable();

        void OnEventRaised();

    }

    public interface IAction<T>
    {
        T Perform();
    }
}
