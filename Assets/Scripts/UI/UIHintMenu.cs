using Events;
using Inputs;
using Manager;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Normal)]
    public class UIHintMenu : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        [SerializeField]
        TextMeshProUGUI hintText;

        public override void SetData(object data)
        {
            base.SetData(data);
            if (data is string v)
            {
                hintText.text = v;
            }
        }

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            inputReader.CloseUIPanelEvent += Close;
        }

        public override void OnUIDisable()
        {
            base.OnUIDisable();
            inputReader.CloseUIPanelEvent -= Close;
        }

        public override void Close()
        {
            new HanoiControlStartEvent();
            base.Close();
        }
    }
}
