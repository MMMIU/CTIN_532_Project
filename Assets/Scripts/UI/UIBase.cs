using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Inputs;
using Manager;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField]
        protected Animator animator;

        public virtual void OnUIAwake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public virtual void SetData(object data)
        {

        }

        public virtual void OnUIStart()
        {

        }

        public virtual void OnUIEnable()
        {
            if (animator != null)
            {
                animator.SetBool("open", true);
                animator.SetBool("close", false);
            }
        }

        public virtual void OnUIDisable()
        {
            if (animator != null)
            {
                animator.SetBool("open", false);
                animator.SetBool("close", true);
            }
        }

        public virtual void OnUIDestroy()
        {
            Destroy(gameObject);
        }

        public virtual void Close()
        {
            Debug.Log("Close");
            UIManager.Instance.Close(this);
        }
    }
}
