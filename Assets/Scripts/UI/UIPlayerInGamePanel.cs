using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI
{
    [UILayer(UIPanelLayer.Fixed)]
    public class UIPlayerInGamePanel : UIBase
    {
        [SerializeField]
        UIPlayerInGamePanel_PlayerStats playerStats;

        public override void OnUIAwake()
        {
            base.OnUIAwake();
            playerStats.UpdatePlayerImage(GameManager.Instance.LocalPlayer.playerSprite);
            OnPlayerDataUpdate(null, GameManager.Instance.LocalPlayer.playerData.Value);
            GameManager.Instance.LocalPlayer.playerData.OnValueChanged += OnPlayerDataUpdate;
        }

        public override void OnUIDestroy()
        {
            GameManager.Instance.LocalPlayer.playerData.OnValueChanged -= OnPlayerDataUpdate;
            base.OnUIDestroy();
        }

        public void OnPlayerDataUpdate(PlayerData old, PlayerData updated)
        {
            Debug.Log("OnPlayerDataUpdate: " + updated.playerHealth + " " + updated.playerMaxHealth + " " + updated.playerEnergy + " " + updated.playerMaxEnergy);
            playerStats.UpdatePlayerStats(updated.playerHealth, updated.playerMaxHealth, updated.playerEnergy, updated.playerMaxEnergy);
        }
    }
}