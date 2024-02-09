using Managers;
using System;
using Unity.Netcode;

namespace Events
{
    public abstract class BaseEvent
    {
        public string name;
        public float originalTime;
        public float endTime;

        public Action<BaseEvent> preEvent;
        public Action<BaseEvent> doEventPreHandler;
        public Action<BaseEvent> doEventAfterHandler;
        public Action<BaseEvent> postEvent;


        public BaseEvent(string name, float delay)
        {
            this.name = name;
            this.originalTime = TimeManager.Instance.GetTime();
            this.endTime = originalTime + delay;

            EventManager.Instance.ScheduleEvent(this);
        }
    }
}