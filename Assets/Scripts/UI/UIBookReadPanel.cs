using Inputs;
using Managers;
using Quest;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Utils;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Game)]
    public class UIBookReadPanel : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        [SerializeField]
        TextMeshProUGUI contentText;

        public override void SetData(object data)
        {
            if(data is string content)
            {
                contentText.text = content;
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
            if(TryGetComponent<QuestProgressModifier>(out var qpm))
            {
                qpm.AddProgress();
            }
            base.Close();
        }
    }
}
