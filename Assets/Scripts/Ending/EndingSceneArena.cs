using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ending
{
    public class EndingSceneArena : MonoBehaviour
    {
        public event UnityAction OnPrincessEnter;
        public event UnityAction OnKnightEnter;
        public event UnityAction OnKnightExit;
        public event UnityAction OnPrincessExit;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter");
            if (other.CompareTag("Player"))
            {
                if(other.TryGetComponent(out Player player))
                {
                    if (player.playerType == Items.ItemAccessbility.knight)
                    {
                        OnKnightEnter?.Invoke();
                    }
                    else if (player.playerType == Items.ItemAccessbility.princess)
                    {
                        OnPrincessEnter?.Invoke();
                    }

                    if(player.IsLocalPlayer)
                    {
                        if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                        {
                            questProgressModifier.AddProgress();
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out Player player))
                {
                    if (player.playerType == Items.ItemAccessbility.knight)
                    {
                        OnKnightExit?.Invoke();
                    }
                    else if (player.playerType == Items.ItemAccessbility.princess)
                    {
                        OnPrincessExit?.Invoke();
                    }

                    if (player.IsLocalPlayer)
                    {
                        if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                        {
                            questProgressModifier.DecreaseProgress();
                        }
                    }
                }
            }
        }
    }
}