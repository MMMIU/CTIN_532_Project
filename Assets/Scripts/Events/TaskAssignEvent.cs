using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
namespace Events {
    public class TaskAssignEvent : BaseEvent
    {
        TaskDataItem taskDataItem;
        public TaskAssignEvent(TaskDataItem task, string name = "TaskAssignEvent", float delay = 0.1f) : base(name, delay)
        {
            taskDataItem = task;
            postEvent += (BaseEvent e) =>
            {
                string popUpText = "Task Assigned: " + taskDataItem.desc.ToString();
                UIManager.Instance.OpenPanel<UIPopUpBar>().SetPopUpText(popUpText);
            };
        }
    }
}