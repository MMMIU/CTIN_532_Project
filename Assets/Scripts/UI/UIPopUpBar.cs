using Inputs;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

namespace UI
{
    [UISaveDic(false)]
    [UILayer(UIPanelLayer.PopUp)]
    public class UIPopUpBar : UIBase
    {
        [SerializeField]
        private TextMeshProUGUI popUpText;

        public override void OnUIEnable()
        {
            gameObject.SetActive(true);
            if (animator != null)
            {
                animator.SetTrigger("show");
            }
        }

        public override void OnUIDisable()
        {
            gameObject.SetActive(false);
        }

        public override void SetData(object data)
        {
            base.SetData(data);
            if (data is string)
            {
                SetPopUpText((string)data);
            }
        }

        public void SetPopUpText(string text)
        {
            popUpText.text = text;
        }

        public override void Close()
        {
            UIManager.Instance.Destroy(this);
        }
    }
}
