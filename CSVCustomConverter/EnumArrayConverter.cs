#if UNITY_EDITOR

using System;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class EnumArrayConverter<T> : DefaultTypeConverter where T : Enum
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        text = text.RemoveWhitespace();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        string[] strEnums = text.Split(Const.CSVToCSSetting.ARRAY_SEPARATOR);
        int[] ints = new int[strEnums.Length];
        T[] result = new T[strEnums.Length];
        for (int i = 0; i < strEnums.Length; i++)
        {
            if (int.TryParse(strEnums[i], out ints[i]))
            {
                result[i] = (T)Enum.ToObject(typeof(T), ints[i]);
            }
            else
            {
                result[i] = (T)Enum.Parse(typeof(T), strEnums[i]);
            }
        }
        return result;
    }
}
#endif