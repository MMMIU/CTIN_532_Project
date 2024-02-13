using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    using System.Collections.Generic;
    using UnityEngine;
    using LitJson;
    using Unity.Netcode;
    using Unity.Collections;

    public class TaskCfg
    {
        public void LoadCfg()
        {
            m_cfg = new Dictionary<int, Dictionary<int, TaskCfgItem>>();
            var txt = Resources.Load<TextAsset>("Configs/Quest/task_cfg").text;
            var jd = JsonMapper.ToObject<JsonData>(txt);

            for (int i = 0, cnt = jd.Count; i < cnt; ++i)
            {
                var itemJd = jd[i] as JsonData;
                var jdData = JsonMapper.ToObject(itemJd.ToJson());
                TaskCfgItem cfgItem = new TaskCfgItem();
                cfgItem.task_chain_id = (int)jdData["task_chain_id"];
                cfgItem.task_sub_id = (int)jdData["task_sub_id"];
                cfgItem.assign = (string)jdData["assign"];
                cfgItem.icon = (string)jdData["icon"];
                cfgItem.desc = (string)jdData["desc"];
                cfgItem.start_action = (string)jdData["start_action"];
                cfgItem.end_action = (string)jdData["end_action"];
                cfgItem.task_target = (string)jdData["task_target"];
                cfgItem.target_amount = (int)jdData["target_amount"];
                cfgItem.award = (string)jdData["award"];
                cfgItem.open_chain = (string)jdData["open_chain"];

                if (!m_cfg.ContainsKey(cfgItem.task_chain_id))
                {
                    m_cfg[cfgItem.task_chain_id] = new Dictionary<int, TaskCfgItem>();
                }
                m_cfg[cfgItem.task_chain_id].Add(cfgItem.task_sub_id, cfgItem);
            }
        }

        public TaskCfgItem GetCfgItem(int chainId, int taskSubId)
        {
            if (m_cfg.ContainsKey(chainId) && m_cfg[chainId].ContainsKey(taskSubId))
                return m_cfg[chainId][taskSubId];
            return null;
        }

        private Dictionary<int, Dictionary<int, TaskCfgItem>> m_cfg;
        public Dictionary<int, Dictionary<int, TaskCfgItem>> configs
        {
            get
            {
                return m_cfg;
            }
        }

        private static TaskCfg instance;
        public static TaskCfg Instance
        {
            get
            {
                if (null == instance)
                    instance = new TaskCfg();
                return instance;
            }
        }

        private TaskCfg()
        {
            LoadCfg();
        }
    }

    public class TaskCfgItem
    {
        public int task_chain_id;
        public int task_sub_id;
        public string assign;
        public string icon;
        public string desc;
        public string start_action;
        public string end_action;
        public string task_target;
        public int target_amount;
        public string award;
        public string open_chain;

        public TaskCfgItem()
        {
        }

        public TaskCfgItem(TaskCfgItem other)
        {
            task_chain_id = other.task_chain_id;
            task_sub_id = other.task_sub_id;
            assign = other.assign;
            icon = other.icon;
            desc = other.desc;
            start_action = other.start_action;
            end_action = other.end_action;
            task_target = other.task_target;
            target_amount = other.target_amount;
            award = other.award;
            open_chain = other.open_chain;
        }
    }

}


