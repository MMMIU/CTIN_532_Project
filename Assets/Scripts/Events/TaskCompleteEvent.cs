using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Managers;
using Unity.Netcode;
namespace Events {
    public class TaskCompleteEvent : EventBase
    {
        public TaskCfgItem taskDataItem;
        public TaskCompleteEvent(TaskCfgItem task, string name = nameof(TaskCompleteEvent), float delay = 0f) : base(name, delay)
        {
            taskDataItem = new(task);
            postEvent += (EventBase e) =>
            {
                if (NetworkManager.Singleton.IsClient)
                {
                    string assign = GameManager.Instance.LocalPlayer.playerType.ToString();
                    if (task.assign == "both" || assign == task.assign)
                    {
                        string popUpText = "Task Complete: " + task.desc;
                        UIManager.Instance.OpenPanel<UIPopUpBar>().SetPopUpText(popUpText);
                    }
                }
            };
        }
    }
}