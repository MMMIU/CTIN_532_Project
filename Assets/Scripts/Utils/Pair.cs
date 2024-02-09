// Pair.cs
using System;

namespace Utils
{
    [Serializable]
    public class Pair<T, E>
    {
        public T First;
        public E Second;

        public Pair(T first, E second)
        {
            First = first;
            Second = second;
        }
    }
}
