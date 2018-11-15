using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {

	T[] items;
	int count;

	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}

	public void Add(T item) {
		item.HeapIndex = count;
		items[count] = item;
		SortUp(item);
		count++;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	public int Count {
		get {
			return count;
		}
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		count--;

		items[0] = items[count];
		items[0].HeapIndex = 0;
		SortDown(items[0]);

		return firstItem;
	}

	void SortDown(T item) {
		int leftChildIndex = item.HeapIndex * 2 + 1;
		int rightChildIndex = item.HeapIndex * 2 + 2;
		int swapIndex;

		while(true) {
			if(leftChildIndex < count) {
				//swap with child of higher priority
				swapIndex = (rightChildIndex < count && items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0) ? rightChildIndex : leftChildIndex;
				if(item.CompareTo(items[swapIndex]) < 0) {
					Swap(item, items[swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}

			leftChildIndex = item.HeapIndex * 2 + 1;
			rightChildIndex = item.HeapIndex * 2 + 2;
		}
	}
	void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while(true) {
			T parent = items[parentIndex];
			if(item.CompareTo(parent) > 0) {
				Swap(item, parent);
			} else {
				return;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void Swap(T a, T b) {
		items[a.HeapIndex] = b;
		items[b.HeapIndex] = a;
		int oldIndex = a.HeapIndex;
		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = oldIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}