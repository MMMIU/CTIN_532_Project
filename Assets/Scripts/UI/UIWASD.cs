using Inputs;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
    [UILayer(UIPanelLayer.Normal)]
    public class UIWASD : UIBase
    {
        [SerializeField]
        InputReader inputReader;

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            inputReader.MoveEvent += ClosePanel;
        }

        public override void OnUIDisable()
        {
            base.OnUIDisable();
            inputReader.MoveEvent -= ClosePanel;
        }

        private void ClosePanel(Vector2 obj)
        {
            UIManager.Instance.Close(this);
        }

    }
}
