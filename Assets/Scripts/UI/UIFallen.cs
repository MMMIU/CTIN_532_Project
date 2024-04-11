using Inputs;
using Items;
using Managers;
using Quest;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Normal)]
    public class UIFallen : UIBase
    {
        [SerializeField]
        InputReader inputReader;

        [SerializeField]
        TextMeshProUGUI fallenText;

        [SerializeField]
        TextMeshProUGUI gameOverText;

        [SerializeField]
        Button button;

        public override void SetData(object data)
        {
            base.SetData(data);
            ItemAccessbility playerType = (ItemAccessbility)data;
            if (playerType == ItemAccessbility.princess)
            {
                fallenText.text = "Princess has been fallen...";
                gameOverText.text = "It over...";
                button.gameObject.SetActive(true);
            }
            else
            {
                fallenText.text = "Knight has been fallen...";
                if (QuestManager.Instance.CheckTaskExist(6, 1))
                {
                    gameOverText.text = "But it's not over yet...";
                    button.gameObject.SetActive(false);
                }
                else
                {
                    gameOverText.text = "It over...";
                    button.gameObject.SetActive(true);
                }
            }
        }

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            UIManager.Instance.CloseAllBlockPanel(this);
        }

        public override void OnUIDisable()
        {
            base.OnUIDisable();
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }
    }
}
