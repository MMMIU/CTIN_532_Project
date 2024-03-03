using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class TimeManager
    {
        private static TimeManager instance = null;
        public static TimeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimeManager();
                }
                return instance;
            }
        }

        public const float secondsPerDay = 12f;

        public const float timeScale = 1f;

        public const float startingHour = 8f;

        [SerializeField]
        private float timeElapsedUnScaled = 0f;
        [SerializeField]
        private float timeElapsed = 0f;

        public void Tick()
        {
            timeElapsedUnScaled += Time.unscaledDeltaTime;
            timeElapsed += Time.deltaTime;
        }

        /// <summary>
        /// Returns the time elapsed since Game begin
        /// </summary>
        /// <returns></returns>
        public float GetTimeUnScaled()
        {
            return timeElapsedUnScaled;
        }

        public float GetTime()
        {
            return timeElapsed;
        }

        /// <summary>
        /// Returns the hour of the day
        /// </summary>
        /// <returns></returns>
        public float GetHour()
        {
            return (startingHour + (timeElapsed / secondsPerDay) * 24) % 24;
        }

        /// <summary>
        /// Returns the day number since Game begin
        /// </summary>
        /// <returns></returns>
        public float GetDay()
        {
            return Mathf.Floor((startingHour / 24 * secondsPerDay + timeElapsed) / secondsPerDay);
        }

        /// <summary>
        /// Returns the time of day as a float
        /// </summary>
        /// <returns></returns>
        public float GetTimeOfDay()
        {
            return (startingHour / 24 * secondsPerDay + timeElapsed) % secondsPerDay;
        }



    }
}
