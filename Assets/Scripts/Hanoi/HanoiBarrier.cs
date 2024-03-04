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
            StartCoroutine(DisableCollider());
        }

        IEnumerator DisableCollider()
        {
            yield return new WaitForSeconds(2f);
            coll.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            new HanoiBarrierPassEvent();
        }
    }
}