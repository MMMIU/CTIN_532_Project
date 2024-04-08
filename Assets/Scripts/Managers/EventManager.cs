using System;
using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Events
{
    public class Singleton<T> where T : new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    (instance as Singleton<T>).OnInitialize();
                }
                return instance;
            }
        }

        protected virtual void OnInitialize()
        {
            Debug.Log("Singleton Initialized: " + typeof(T).Name);
        }

        public void EagerInit()
        {
            return;
        }
    }

    public partial class EventManager : Singleton<EventManager>
    {
        private bool isDealingWithHandler = false;
        private List<KeyValuePair<string, Delegate>> handlersToBeAdded = new();
        private List<KeyValuePair<string, Delegate>> handlersToBeRemoved = new();

        private List<EventBase> eventList = new();
        private Dictionary<string, List<Delegate>> eventHandlers = new();

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
                        handler.DynamicInvoke(e);
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
            string eventName = typeof(T).Name;
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
            string eventName = typeof(T).Name;
            Debug.Log("Unsubscribing from event: " + eventName);
            if (isDealingWithHandler)
            {
                handlersToBeRemoved.Add(new(eventName, handler));
                return;
            }
            if (eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName].Remove(handler);
                if (eventHandlers[eventName].Count == 0)
                {
                    eventHandlers.Remove(eventName);
                }
            }
        }

        private void DealWithHandlersToBeAddedOrRemoved()
        {
            foreach (var pair in handlersToBeAdded)
            {
                if (!eventHandlers.ContainsKey(pair.Key))
                {
                    eventHandlers[pair.Key] = new List<Delegate>();
                }
                eventHandlers[pair.Key].Add(pair.Value);
            }

            foreach (var pair in handlersToBeRemoved)
            {
                if (eventHandlers.ContainsKey(pair.Key))
                {
                    var handlersList = eventHandlers[pair.Key];
                    // assume no duplicate handlers
                    if (handlersList.Contains(pair.Value))
                    {
                        handlersList.Remove(pair.Value);
                        // if no handler left, remove the event
                        if (handlersList.Count == 0)
                        {
                            eventHandlers.Remove(pair.Key);
                        }
                    }
                }
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
