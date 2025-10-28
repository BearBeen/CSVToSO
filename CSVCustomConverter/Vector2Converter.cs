#if UNITY_EDITOR
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

using UnityEngine;

public class Vector2Converter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        text = text.RemoveWhitespaceAndExtra('(', ')');
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        string[] strInts = text.Split(Const.CSVToCSSetting.ARRAY_SEPARATOR);
        Vector2 result = Vector2.zero;
        if (strInts.Length >= 2)
        {
            result.y = int.Parse(strInts[1]);
        }
        if (strInts.Length >= 1)
        {
            result.x = int.Parse(strInts[0]);
        }
        return result;
    }
}
#endif
