using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int count;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Добавляет элемент в кучу
    /// </summary>
    /// <param name="item">Элемент</param>
    public void Add(T item)
    {
        item.HeapIndex = count;
        items[count] = item;
        SortUp(item);
        count++;
    }

    /// <summary>
    /// Берёт первый элемент из кучи
    /// </summary>
    /// <returns>Элемент</returns>
    public T RemoveFirst()
    {
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    //public void UpdateItem(T item)
    //{
    //    SortUp(item);
    //}

    public int Count { get => count; }

    /// <summary>
    /// Проверяет элементы на сходство
    /// </summary>
    /// <param name="item">Элемент</param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if(childIndexLeft < count)
            {
                swapIndex = childIndexLeft;

                if(childIndexRight < count)
                {
                    if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if(item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if(item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    /// <summary>
    /// Меняет элементы местами
    /// </summary>
    /// <param name="itemA">Первый элемент</param>
    /// <param name="itemB">Второй элемент</param>
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
