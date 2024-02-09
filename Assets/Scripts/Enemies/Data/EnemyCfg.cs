using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemyCfgItem
    {
        // enemy uid
        public int enemy_uid;
        // enemy type
        public string enemy_type;
        public int enemy_sub_id;
        // enemy name
        public string name;
        // enemy description
        public string desc;
        // enemy health
        public int health;
        // enemy award
        public string award;
    }

    public class EnemyCfg
    {
        private Dictionary<int, EnemyCfgItem> m_cfg;
        public Dictionary<int, EnemyCfgItem> configs
        {
            get
            {
                return m_cfg;
            }
        }
        private static EnemyCfg instance;
        public static EnemyCfg Instance
        {
            get
            {
                if (instance == null)
                    instance = new EnemyCfg();
                return instance;
            }
        }

        public void LoadCfg()
        {
            m_cfg = new Dictionary<int, EnemyCfgItem>();
            var txt = Resources.Load<TextAsset>("Configs/Enemy/enemy_cfg").text;
            var jd = LitJson.JsonMapper.ToObject<JsonData>(txt);

            for (int i = 0, cnt = jd.Count; i < cnt; ++i)
            {
                var itemJd = jd[i];
                EnemyCfgItem cfgItem = LitJson.JsonMapper.ToObject<EnemyCfgItem>(itemJd.ToJson());
                m_cfg[cfgItem.enemy_uid] = cfgItem;
            }
        }

        public EnemyCfgItem GetCfgItem(int enemy_uid)
        {
            if (m_cfg.ContainsKey(enemy_uid))
            {
                return m_cfg[enemy_uid];
            }
            return null;
        }
    }
}