using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class ItemDataItem
    {
        public int item_uid;
        public ItemType item_type;
        public int item_sub_id;
        public string name;
        public string desc;
        public ItemAccessbility accessbility;
        public bool init_interactable;
        //public bool interactable;

        //public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        //{
        //    serializer.SerializeValue(ref item_uid);
        //    serializer.SerializeValue(ref item_type);
        //    serializer.SerializeValue(ref item_sub_id);
        //    serializer.SerializeValue(ref name);
        //    serializer.SerializeValue(ref desc);
        //    serializer.SerializeValue(ref accessbility);
        //    serializer.SerializeValue(ref init_interactable);
        //    serializer.SerializeValue(ref interactable);
        //}
    }

    public class ItemData
    {
        private List<ItemDataItem> m_itemDatas;

        public ItemData()
        {
            m_itemDatas = new List<ItemDataItem>();
        }

        public ItemDataItem GetData(int itemUid)
        {
            foreach (var itemData in m_itemDatas)
            {
                if (itemData.item_uid == itemUid)
                {
                    return itemData;
                }
            }
            return null;
        }

        public void LoadAllItems()
        {
            m_itemDatas.Clear();
            ItemCfg.Instance.LoadCfg();
            foreach (var itemCfgItem in ItemCfg.Instance.configs.Values)
            {
                ItemDataItem itemDataItem = new ItemDataItem();
                itemDataItem.item_uid = itemCfgItem.item_uid;
                //itemDataItem.item_type_name = itemCfgItem.item_type_name;
                itemDataItem.item_type = (ItemType)Enum.Parse(typeof(ItemType), itemCfgItem.item_type);
                itemDataItem.item_sub_id = itemCfgItem.item_sub_id;
                itemDataItem.name = itemCfgItem.name;
                itemDataItem.desc = itemCfgItem.desc;
                //itemDataItem.accessbility = itemCfgItem.accessbility;
                itemDataItem.accessbility = (ItemAccessbility)Enum.Parse(typeof(ItemAccessbility), itemCfgItem.accessbility);
                if (itemCfgItem.init_interactable == "yes")
                {
                    itemDataItem.init_interactable = true;
                }
                else
                {
                    itemDataItem.init_interactable = false;
                }
                m_itemDatas.Add(itemDataItem);
            }
        }
    }
}
