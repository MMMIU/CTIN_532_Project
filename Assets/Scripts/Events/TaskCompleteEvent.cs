using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
namespace Events {
    public class TaskCompleteEvent : EventBase
    {
        TaskCfgItem taskDataItem;
        public TaskCompleteEvent(TaskCfgItem task, string name = nameof(TaskAssignEvent), float delay = 0f) : base(name, delay)
        {
            taskDataItem = new(task);
            postEvent += (EventBase e) =>
            {
                string popUpText = "Task Complete: " + taskDataItem.desc.ToString();
                UIManager.Instance.OpenPanel<UIPopUpBar>().SetPopUpText(popUpText);
            };
        }
    }
}