using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi
{
    public class HanoiBarrier : MonoBehaviour
    {
        Animator animator;
        Collider coll;

        void Start()
        {
            animator = GetComponent<Animator>();
            coll = GetComponent<Collider>();
            EventManager.Instance.Subscribe<HanoiWinEvent>(DoHanoiWin);
        }

        void DoHanoiWin(HanoiWinEvent e)
        {
            animator.SetBool("hide", true);
        }

        public void DisableCollAndPass()
        {
            coll.enabled = false;
            new HanoiBarrierPassEvent();
        }
    }
}