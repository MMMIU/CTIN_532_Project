using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
namespace Events
{
    public class TaskAssignEvent : EventBase
    {
        const float oneRoundDelay = 3f;
        TaskDataItem taskDataItem;
        int delayRound = 0;
        public TaskAssignEvent(TaskDataItem task, int delayRound = 0, string name = nameof(TaskAssignEvent), float delay = 0f) : base(name, delay)
        {
            taskDataItem = task;
            this.delayRound = delayRound;

            postEvent += (EventBase e) =>
            {
                string popUpText = "Task Assigned: " + TaskCfg.Instance.GetCfgItem(taskDataItem.task_chain_id, taskDataItem.task_sub_id).desc;
                UIManager.Instance.DelayOpenPanel<UIPopUpBar>(oneRoundDelay * delayRound, popUpText);
            };
        }
    }
}