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
        private List<KeyValuePair<string, Action<EventBase>>> handlersToBeAdded = new();
        private List<KeyValuePair<string, Action<EventBase>>> handlersToBeRemoved = new();


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

        private List<EventBase> eventList = new();
        private Dictionary<string, List<Action<EventBase>>> eventHandlers = new();

        public void Tick()
        {
            float currentTime = TimeManager.Instance.GetTimeUnScaled();
            List<EventBase> eventsToTrigger = new();

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

        public void Subscribe<T>(Action<T> handler) where T : EventBase
        {
            Subscribe(typeof(T).Name, (e) => handler((T)e));
        }

        public void Subscribe(string eventName, Action<EventBase> handler)
        {
            Debug.Log("Subscribing to event: " + eventName);
            if (isDealingWithHandler)
            {
                handlersToBeAdded.Add(new(eventName, handler));
                return;
            }
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new();
            }
            eventHandlers[eventName].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : EventBase
        {
            Unsubscribe(typeof(T).Name, (e) => handler((T)e));
        }

        public void Unsubscribe(string eventName, Action<EventBase> handler)
        {
            Debug.Log("Unsubscribing from event: " + eventName);
            if (isDealingWithHandler)
            {
                handlersToBeRemoved.Add(new(eventName, handler));
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

        public void ScheduleEvent(EventBase newEvent)
        {
            Debug.Log("Scheduling event: " + newEvent.name);
            eventList.Add(newEvent);
        }
    }
}
