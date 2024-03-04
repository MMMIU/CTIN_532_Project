using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi
{
    public class HanoiCoin : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            new PrincessSkillUpgradeEvent();
            Destroy(gameObject);
        }
    }
}