using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hanoi
{
    public class HanoiTower : MonoBehaviour
    {
        // OnPushSccessfulEvent UnityAction
        public event UnityAction OnPushSccessfulEvent;

        private Stack<HanoiDisk> disks = new ();
        public Vector3 dropOffOffset;

        [SerializeField]
        Animator animator;
        public void SetSelected(bool selected)
        {
            animator.SetBool("show", selected);
        }

        public int DiskCount
        {
            get { return disks.Count; }
        }

        public void Clear()
        {
            disks.Clear();
        }

        public bool CanShine(HanoiDisk disk)
        {
            if (disks.Count > 0 && disks.Peek().diskIndex > disk.diskIndex)
            {
                Debug.Log("Invalid Move, top" + disks.Peek().diskIndex + " > incoming " + disk.diskIndex);
                return false;
            }
            return true;
        }

        public bool Push(HanoiDisk disk)
        {
            if (disks.Count > 0 && disks.Peek().diskIndex >= disk.diskIndex)
            {
                Debug.Log("Invalid Move, top" + disks.Peek().diskIndex + " > incoming " + disk.diskIndex);
                return false;
            }
            // set top disk not movable
            if (disks.Count > 0)
            {
                disks.Peek().Movable = false;
            }
            disks.Push(disk);
            disk.Movable = true;
            disk.SetPosition(transform.position + dropOffOffset);
            OnPushSccessfulEvent?.Invoke();
            Debug.Log("Pushed " + disk.diskIndex + " to " + name + " count " + disks.Count);
            return true;
        }

        public HanoiDisk Pop()
        {
            if (disks.Count == 0)
            {
                return null;
            }
            HanoiDisk disk = disks.Pop();
            if (disks.Count > 0)
            {
                disks.Peek().Movable = true;
            }
            return disk;
        }

        public HanoiDisk Peek()
        {
            if (disks.Count == 0)
            {
                return null;
            }
            return disks.Peek();
        }
    }
}