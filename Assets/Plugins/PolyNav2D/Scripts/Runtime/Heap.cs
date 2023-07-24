using System;

namespace PolyNav
{

    public interface IHeapItem<T> : IComparable<T>
    {
        int heapIndex { get; set; }
    }

    public class Heap<T> where T : IHeapItem<T>
    {
        T[] elements;
        int elementsCount;

        public int Count => elementsCount;

        public Heap(int maxHeapSize) => elements = new T[maxHeapSize];
        public bool Contains(T item) => Equals(elements[item.heapIndex], item);

        public void Add(T item) {
            item.heapIndex = elementsCount;
            elements[elementsCount] = item;
            SortUp(item);
            elementsCount++;
        }

        public T RemoveFirst() {
            T firstItem = elements[0];
            elementsCount--;
            elements[0] = elements[elementsCount];
            elements[0].heapIndex = 0;
            SortDown(elements[0]);
            return firstItem;
        }

        void SortUp(T item) {
            int parentIndex = ( item.heapIndex - 1 ) / 2;
            while ( true ) {
                T parentItem = elements[parentIndex];
                if ( item.CompareTo(parentItem) > 0 ) {
                    Swap(item, parentItem);
                } else { break; }
                parentIndex = ( item.heapIndex - 1 ) / 2;
            }
        }

        void SortDown(T item) {
            while ( true ) {
                int childIndexLeft = item.heapIndex * 2 + 1;
                int childIndexRight = item.heapIndex * 2 + 2;
                int swapIndex = 0;
                if ( childIndexLeft < elementsCount ) {
                    swapIndex = childIndexLeft;
                    if ( childIndexRight < elementsCount ) {
                        if ( elements[childIndexLeft].CompareTo(elements[childIndexRight]) < 0 ) {
                            swapIndex = childIndexRight;
                        }
                    }
                    if ( item.CompareTo(elements[swapIndex]) < 0 ) {
                        Swap(item, elements[swapIndex]);
                    } else { return; }
                } else { return; }
            }
        }

        void Swap(T itemA, T itemB) {
            elements[itemA.heapIndex] = itemB;
            elements[itemB.heapIndex] = itemA;
            int itemAIndex = itemA.heapIndex;
            itemA.heapIndex = itemB.heapIndex;
            itemB.heapIndex = itemAIndex;
        }
    }
}