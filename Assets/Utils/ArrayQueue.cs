using System;
using System.Collections.Generic;

public class ArrayQueue<T>
{
	private readonly T[] _items;
	private readonly HashSet<T> _itemSet = new();
	private int _headIndex = 0;
	private int _count = 0;

	public int Capacity => _items.Length;
	public int Count => _count;

	public ref T this[int index] => ref _items[(_headIndex - index + _items.Length) % _items.Length];

	public ArrayQueue(int size, in T initialValue = default)
	{
		if (size <= 0)
			throw new ArgumentException("Size must be greater than zero.", nameof(size));

		_items = new T[size];
		Array.Fill(_items, initialValue);
	}

	public bool Contains(T item) => _itemSet.Contains(item);

	public void PushHash(in T item)
	{
		T overwritten = _items[_headIndex];
		if (_itemSet.Contains(overwritten))
			return;

		GeneralMethodPush(item);
	}

	public void Push(in T item)
	{
		T overwritten = _items[_headIndex];
		if (_itemSet.Contains(overwritten))
			_itemSet.Remove(overwritten);

		GeneralMethodPush(item);
	}

	private void GeneralMethodPush(in T item)
	{
		_items[_headIndex] = item;
		_itemSet.Add(item);

		_headIndex = (_headIndex + 1) % _items.Length;

		if (_count < _items.Length)
			_count++;
	}

	public T[] ToTrimedArray()
	{
		var result = new T[_count];
		for (int i = 0; i < _count; i++)
			result[i] = this[i];
		return result;
	}

	public T[] ToArray() => _items;
}