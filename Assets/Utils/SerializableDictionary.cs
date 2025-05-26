using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
	[SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> entries = new();
	private Dictionary<TKey, TValue> _dictionary;

	public Dictionary<TKey, TValue> ToDictionary()
	{
		if (_dictionary == null)
		{
			_dictionary = new Dictionary<TKey, TValue>();
			foreach (var kv in entries)
			{
				if (!_dictionary.ContainsKey(kv.Key))
					_dictionary.Add(kv.Key, kv.Value);
			}
		}
		return _dictionary;
	}

	public void OnBeforeSerialize()
	{
		// nothing
	}

	public void OnAfterDeserialize()
	{
		_dictionary = null;
	}

	// Optional indexer
	public TValue this[TKey key] => ToDictionary()[key];

	// Optional: expose internal list for manual inspection or editing
	public List<SerializableKeyValuePair<TKey, TValue>> RawList => entries;
}

