using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Util
{
    public class PanelTrigger : MonoBehaviour
    {
        [SerializeField]
        UIBase panel;

        [SerializeField]
        bool showOnce = false;

        public void ShowPanel(object data = null)
        {
            Type hintPanelType = panel.GetType();
            Debug.Log("ShowHint: " + hintPanelType);
            //UIManager.Instance.OpenPanel(panel, data);
            if (showOnce)
            {
                Destroy(this);
            }
        }
    }
}