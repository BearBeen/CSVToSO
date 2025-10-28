#if UNITY_EDITOR
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class FloatArrayConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        text = text.RemoveWhitespace();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        string[] strFloats = text.Split(Const.CSVToCSSetting.ARRAY_SEPARATOR);
        float[] result = new float[strFloats.Length];
        for (int i = 0; i < strFloats.Length; i++)
        {
            result[i] = float.Parse(strFloats[i]);
        }
        return result;
    }
}
#endif