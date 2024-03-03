using Managers;
using System;
using Unity.Netcode;

namespace Events
{
    public abstract class EventBase
    {
        public string name;
        public float originalTime;
        public float endTime;

        public Action<EventBase> preEvent;
        public Action<EventBase> doEventPreHandler;
        public Action<EventBase> doEventAfterHandler;
        public Action<EventBase> postEvent;


        public EventBase(string name, float delay)
        {
            this.name = name;
            this.originalTime = TimeManager.Instance.GetTimeUnScaled();
            this.endTime = originalTime + delay;

            EventManager.Instance.ScheduleEvent(this);
        }
    }
}