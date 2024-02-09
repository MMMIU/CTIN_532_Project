using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class ItemLogic
    {
        private ItemData m_itemData;
        private static ItemLogic instance;
        public static ItemLogic Instance
        {
            get
            {
                if (null == instance)
                    instance = new ItemLogic();
                return instance;
            }
        }

        public ItemLogic()
        {
            m_itemData = new ItemData();
            m_itemData.LoadAllItems();
        }

        public ItemDataItem GetItemData(int itemUid)
        {
            return m_itemData.GetData(itemUid);
        }

    }
}