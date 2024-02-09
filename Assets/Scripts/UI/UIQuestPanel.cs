using Inputs;
using Manager;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Quest;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Fixed)]
    public partial class UIQuestPanel : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        [SerializeField]
        private GameObject taskItemPrefab;

        [SerializeField]
        private Transform taskItemParent;

        private bool firstTime = true;

        public override void OnUIAwake()
        {
            base.OnUIAwake();
            if(firstTime)
            {
                firstTime = false;
                OnTaskDataChanged(null, QuestManager.Instance.TaskData);
            }
        }

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            inputReader.CloseUIPanelEvent += Close;
            inputReader.UI_CloseQuestPanelEvent += Close;
            OnTaskDataChanged(null, QuestManager.Instance.TaskData);
        }

        public override void OnUIDisable()
        {
            base.OnUIDisable();
            inputReader.CloseUIPanelEvent -= Close;
            inputReader.UI_CloseQuestPanelEvent -= Close;
        }

        public override void OnUIDestroy()
        {
            base.OnUIDestroy();
        }

        private void OnTaskDataChanged(TaskData oldTaskData, TaskData newTaskData)
        {
            Debug.Log("OnTaskDataChanged");
            if (newTaskData == null)
            {
                return;
            }

            // destroy all children except the first one
            for (int i = taskItemParent.childCount - 1; i > 0; i--)
            {
                Destroy(taskItemParent.GetChild(i).gameObject);
            }

            foreach (var item in newTaskData.taskDatas)
            {
                var taskItem = Instantiate(taskItemPrefab, taskItemParent).GetComponent<UIQuestPanel_TaskItem>();
                taskItem.SetTaskData(item);
            }
        }
    }
}
