using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi
{
    public class HanoiSpotLight : MonoBehaviour
    {
        Animator animator;
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            EventManager.Instance.Subscribe<HanoiBarrierPassEvent>(DoPassEvent);
        }

        private void DoPassEvent(EventBase e)
        {
            animator.SetTrigger("show");
        }

    }
}