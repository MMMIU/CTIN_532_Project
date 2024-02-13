using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quest;
namespace UI
{
    public class UIQuestPanel_TaskItem : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI taskIdText;

        [SerializeField]
        private TMPro.TextMeshProUGUI assignText;

        [SerializeField]
        private TMPro.TextMeshProUGUI descText;

        [SerializeField]
        private TMPro.TextMeshProUGUI progressText;

        [SerializeField]
        private TMPro.TextMeshProUGUI awardText;


        public void SetTaskData(TaskDataItem taskDataItem)
        {
            taskIdText.text = taskDataItem.task_chain_id.ToString() + "-" + taskDataItem.task_sub_id.ToString();
            assignText.text = TaskCfg.Instance.GetCfgItem(taskDataItem.task_chain_id, taskDataItem.task_sub_id).assign;
            descText.text = TaskCfg.Instance.GetCfgItem(taskDataItem.task_chain_id, taskDataItem.task_sub_id).desc;
            progressText.text = taskDataItem.progress.ToString() + "/" + TaskCfg.Instance.GetCfgItem(taskDataItem.task_chain_id, taskDataItem.task_sub_id).target_amount.ToString();
        }
    }
}