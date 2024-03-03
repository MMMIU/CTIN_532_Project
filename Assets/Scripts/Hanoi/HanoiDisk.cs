using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi
{
    public class HanoiDisk : MonoBehaviour
    {
        public int diskIndex;

        [SerializeField]
        HanoiTower currentTower = null;
        public void SetCurrentTower(HanoiTower tower)
        {
            currentTower = tower;
        }

        [SerializeField]
        HanoiTower targetTower = null;
        public void SetTargetTower(HanoiTower tower)
        {
            targetTower = tower;
        }

        [SerializeField]
        Vector3 oldPosition;

        [SerializeField]
        Animator animator;

        public Rigidbody rb;
        public Collider col;
        bool movable = false;

        public bool Movable
        {
            get { return movable; }
            set
            {
                movable = value;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            animator = GetComponent<Animator>();
            movable = false;
        }

        public void SetSelected(bool selected)
        {
            animator.SetBool("show", selected);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!movable)
            {
                return;
            }
            if (other.gameObject.TryGetComponent(out HanoiTower tower))
            {
                if (targetTower != currentTower)
                {
                    targetTower = tower;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!movable)
            {
                return;
            }
            if (other.gameObject.TryGetComponent(out HanoiTower tower))
            {
                if (tower == targetTower)
                {
                    targetTower = null;
                }
            }
        }

        public void SetAsFreeDisk(bool free)
        {
            if(!movable)
            {
                return;
            }
            rb.isKinematic = free;
            col.isTrigger = free;
            if (free)
            {
                oldPosition = transform.position;
            }
            else
            {
                if (targetTower != null && targetTower.Push(this))
                {
                    currentTower.Pop();
                    currentTower = targetTower;
                }
                else
                {
                    SetPosition(oldPosition);
                }
            }
        }

        public void SetPosition(Vector3 position)
        {
            if (!movable)
            {
                return;
            }
            rb.position = position;
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}