using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
namespace Events {
    public class TaskCompleteEvent : BaseEvent
    {
        TaskCfgItem taskDataItem;
        public TaskCompleteEvent(TaskCfgItem task, string name = "TaskAssignEvent", float delay = 0f) : base(name, delay)
        {
            taskDataItem = new(task);
            postEvent += (BaseEvent e) =>
            {
                string popUpText = "Task Complete: " + taskDataItem.desc.ToString();
                UIManager.Instance.OpenPanel<UIPopUpBar>().SetPopUpText(popUpText);
            };
        }
    }
}