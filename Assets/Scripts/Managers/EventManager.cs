using System;
using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Events
{
    public partial class EventManager
    {
        private static EventManager instance = null;

        private bool isDealingWithHandler = false;

        private Dictionary<string, Action<BaseEvent>> handlersToBeAdded = new();
        private Dictionary<string, Action<BaseEvent>> handlersToBeRemoved = new();


        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }

        private EventManager()
        {
            isDealingWithHandler = false;
            handlersToBeAdded = new();
            handlersToBeRemoved = new();
        }

        private List<BaseEvent> eventList = new();
        private Dictionary<string, List<Action<BaseEvent>>> eventHandlers = new();

        public void Tick()
        {
            float currentTime = TimeManager.Instance.GetTime();
            List<BaseEvent> eventsToTrigger = new();

            foreach (var e in eventList)
            {
                if (e.endTime <= currentTime)
                {
                    eventsToTrigger.Add(e);
                }
            }

            foreach (var e in eventsToTrigger)
            {
                Debug.Log("Triggering event: " + e.name);

                e.preEvent?.Invoke(e);

                e.doEventPreHandler?.Invoke(e);

                //handlers
                if (eventHandlers.ContainsKey(e.name))
                {
                    isDealingWithHandler = true;
                    foreach (var handler in eventHandlers[e.name])
                    {
                        handler(e);
                    }
                    isDealingWithHandler = false;
                }

                e.doEventAfterHandler?.Invoke(e);

                e.postEvent?.Invoke(e);
                eventList.Remove(e);
            }

            DealWithHandlersToBeAddedOrRemoved();
        }

        public void Subscribe(string eventName, Action<BaseEvent> handler)
        {
            Debug.Log("Subscribing to event: " + eventName);
            if (isDealingWithHandler)
            {
                handlersToBeAdded.Add(eventName, handler);
                return;
            }
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new();
            }
            eventHandlers[eventName].Add(handler);
        }

        public void Unsubscribe(string eventName, Action<BaseEvent> handler)
        {
            Debug.Log("Unsubscribing from event: " + eventName);
            if (isDealingWithHandler)
            {
                handlersToBeRemoved.Add(eventName, handler);
                return;
            }
            if (eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName].Remove(handler);
            }
        }

        private void DealWithHandlersToBeAddedOrRemoved()
        {
            foreach (var pair in handlersToBeAdded)
            {
                Subscribe(pair.Key, pair.Value);
            }
            foreach (var pair in handlersToBeRemoved)
            {
                Unsubscribe(pair.Key, pair.Value);
            }
            handlersToBeAdded.Clear();
            handlersToBeRemoved.Clear();
        }

        public void ScheduleEvent(BaseEvent newEvent)
        {
            Debug.Log("Scheduling event: " + newEvent.name);
            eventList.Add(newEvent);
        }
    }
}
