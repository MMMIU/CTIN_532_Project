using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Items
{
    public class ItemCfgItem
    {
        // item uid
        public int item_uid;
        // item type
        public string item_type;
        public int item_sub_id;
        // item name
        public string name;
        // item description
        public string desc;
        // item can be accessed by player type
        public string accessbility;
        // item can be interacted, yes = initial interactable, no = initial not interactable, null = not interactable at all
        public string init_interactable;
    }

    public class ItemCfg
    {

        private Dictionary<int, ItemCfgItem> m_cfg;
        public Dictionary<int, ItemCfgItem> configs
        {
            get
            {
                return m_cfg;
            }
        }
        private static ItemCfg instance;
        public static ItemCfg Instance
        {
            get
            {
                if (instance == null)
                    instance = new ItemCfg();
                return instance;
            }
        }

        public void LoadCfg()
        {
            m_cfg = new Dictionary<int, ItemCfgItem>();
            var txt = Resources.Load<TextAsset>("Configs/Item/item_cfg").text;
            var jd = LitJson.JsonMapper.ToObject<JsonData>(txt);

            for (int i = 0, cnt = jd.Count; i < cnt; ++i)
            {
                var itemJd = jd[i];
                ItemCfgItem cfgItem = LitJson.JsonMapper.ToObject<ItemCfgItem>(itemJd.ToJson());
                m_cfg[cfgItem.item_uid] = cfgItem;
            }
        }

        public ItemCfgItem GetCfgItem(int item_uid)
        {
            if (m_cfg.ContainsKey(item_uid))
                return m_cfg[item_uid];
            return null;
        }
    }
}
