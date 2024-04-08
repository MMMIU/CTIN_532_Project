using Inputs;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Normal)]
    public class UIPauseMenu : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            inputReader.CloseUIPanelEvent += Close;
            // set canvas group to interactable and block raycast
            if (TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        public override void OnUIDisable()
        {
            base.OnUIDisable();
            inputReader.CloseUIPanelEvent -= Close;
            // set canvas group to non interactable and not block raycast
            if (TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void SetMasterVolume(float value)
        {
            SFXManager.Instance.SetMasterVolume(value);
        }

        public void SetMusicVolume(float value)
        {
            SFXManager.Instance.SetMusicVolume(value);
        }

        public void SetSoundVolume(float value)
        {
            SFXManager.Instance.SetSoundVolume(value);
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }
    }
}
