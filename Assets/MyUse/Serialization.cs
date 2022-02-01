//感谢 http://kou-yeung.hatenablog.com/entry/2015/12/31/014611
//虽然没用上^_^
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}

[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}

[Serializable]
public class SerializationBitArray : ISerializationCallbackReceiver
{
    [SerializeField]
    string flags;

    BitArray target;
    public BitArray ToBitArray() { return target; }

    public SerializationBitArray(BitArray target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        var ss = new System.Text.StringBuilder(target.Length);
        for (var i = 0; i < target.Length; ++i)
        {
            ss.Insert(0, target[i] ? '1' : '0');
        }
        flags = ss.ToString();
    }

    public void OnAfterDeserialize()
    {
        target = new BitArray(flags.Length);
        for (var i = 0; i < flags.Length; ++i)
        {
            target.Set(flags.Length - i - 1, flags[i] == '1');
        }
    }
}
