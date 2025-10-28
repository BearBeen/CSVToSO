using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using CsvHelper;

#endif

public abstract class NormalConfigBase : ScriptableObject
{
#if UNITY_EDITOR
    public abstract void SyncFromCSV(CsvReader csv);
#endif
}

public abstract class NormalConfig<T> : NormalConfigBase where T : NormalConfigStructure
{
    protected abstract List<T> data { get; }

    public T this[int index] => (index >= 0 && index < data.Count) ? data[index] : default;

    public T Get(Func<T, int> getIndex, int checkValue)
    {
        int min = 0, max = data.Count - 1, middle, middleValue;
        while (min <= max)
        {
            middle = min + (max - min) / 2;
            middleValue = getIndex(data[middle]);
            if (middleValue == checkValue)
            {
                return data[middle];
            }
            else if (checkValue > middleValue)
            {
                min = middle + 1;
            }
            else
            {
                max = middle - 1;
            }
        }
        return null;
    }

    public T Get(Predicate<T> check)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (check(data[i]))
            {
                return data[i];
            }
        }
        return null;
    }

    public List<T> GetAll(Predicate<T> check)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < data.Count; i++)
        {
            if (check(data[i]))
            {
                result.Add(data[i]);
            }
        }
        return result;
    }
}

public abstract class NormalConfigStructure
{
}
