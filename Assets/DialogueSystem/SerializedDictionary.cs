using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keyData = new();

	[SerializeField]
	private List<TValue> valueData = new();

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		Clear();
		for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
		{
			this[keyData[i]] = valueData[i];
		}
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		keyData.Clear();
		valueData.Clear();

		foreach (var item in this)
		{
			keyData.Add(item.Key);
			valueData.Add(item.Value);
		}
	}
}

