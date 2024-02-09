using System;
using System.Collections;
using System.Collections.Generic;
namespace Utils
{
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private List<T> heap = new List<T>();

        public int Count => heap.Count;

        public void Enqueue(T item)
        {
            heap.Add(item);
            SiftUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Queue is empty");
            T item = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            SiftDown(0);
            return item;
        }

        public void UpdateItem(T item)
        {
            int index = heap.IndexOf(item);
            if (index == -1) throw new ArgumentException("Item not found in the queue");

            SiftUp(index);
            SiftDown(index);
        }

        public void Remove(T item)
        {
            int index = heap.IndexOf(item);
            if (index == -1) throw new ArgumentException("Item not found in the queue");

            heap[index] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            SiftDown(index);
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].CompareTo(heap[parentIndex]) >= 0) break;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void SiftDown(int index)
        {
            int lastIndex = heap.Count - 1;
            while (true)
            {
                int leftChildIndex = 2 * index + 1;
                int rightChildIndex = 2 * index + 2;
                int smallestIndex = index;

                if (leftChildIndex <= lastIndex && heap[leftChildIndex].CompareTo(heap[smallestIndex]) < 0)
                {
                    smallestIndex = leftChildIndex;
                }

                if (rightChildIndex <= lastIndex && heap[rightChildIndex].CompareTo(heap[smallestIndex]) < 0)
                {
                    smallestIndex = rightChildIndex;
                }

                if (smallestIndex != index)
                {
                    Swap(index, smallestIndex);
                    index = smallestIndex;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(int index1, int index2)
        {
            T temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


}