using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Managers;
using Items;
using Unity.Netcode;
using Players;
namespace Events
{
    public class TaskAssignEvent : EventBase
    {
        public TaskCfgItem taskDataItem;

        const float oneRoundDelay = 3f;
        int delayRound = 0;
        public TaskAssignEvent(TaskCfgItem task, int delayRound = 0, string name = nameof(TaskAssignEvent), float delay = 0f) : base(name, delay)
        {
            taskDataItem = new(task);
            this.delayRound = delayRound;

            postEvent += (EventBase e) =>
            {
                if(NetworkManager.Singleton.IsClient)
                {
                    string assign = GameManager.Instance.LocalPlayer.playerType.ToString();
                    if (task.assign == "both" || assign == task.assign)
                    {
                        string popUpText = "Task Assigned: " + task.desc;
                        UIManager.Instance.DelayOpenPanel<UIPopUpBar>(oneRoundDelay * delayRound, popUpText);
                    }
                }
               
            };
        }
    }
}