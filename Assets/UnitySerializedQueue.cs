using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitySerializedQueue<T> : Queue<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<T> data = new();

    public void OnBeforeSerialize()
    {
        while(TryDequeue(out T item))
        {
            data.Add(item);
        }
    }

    public void OnAfterDeserialize()
    {
        foreach(var item in data)
        {
            this.Enqueue(item);
        }
    }

}
