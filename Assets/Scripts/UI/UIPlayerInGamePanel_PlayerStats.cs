using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPlayerInGamePanel_PlayerStats : MonoBehaviour
    {
        // Player Image
        [SerializeField]
        private Image playerImage;

        // player health slider
        [SerializeField]
        private Slider playerHealthSlider;

        // methods to update player stats
        public void UpdatePlayerStats(float health, float maxHealth, float energy, float maxEnergy)
        {
            playerHealthSlider.value = health / maxHealth;
        }

        public void UpdatePlayerImage(Sprite playerSprite)
        {
            playerImage.sprite = playerSprite;
        }

        public void UpdatePlayerHealth(float health, float maxHealth)
        {
            playerHealthSlider.value = health / maxHealth;
        }
    }
}