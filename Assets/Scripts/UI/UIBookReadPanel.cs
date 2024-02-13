using Inputs;
using Manager;
using Managers;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Game)]
    public class UIBookReadPanel : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

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
            QuestManager.Instance.AddProgressServerRpc(1, 1, 1);
            base.Close();
        }
    }
}
