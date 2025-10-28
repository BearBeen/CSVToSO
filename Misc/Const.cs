//should not wrap with my CSVToSO name space, as converter would like to use these constant
//since converter is part of the main config, spoil it with my name space may become bad. partial is flexible enough
public static partial class Const
{
    public static partial class CSVToCSSetting
    {
        public static readonly char ARRAY_SEPARATOR = ',';
        public static readonly string[] DEFAULT_ASSEMBLY_NAME = { "Assembly-CSharp", "CSVToSO", "CSVToSO_Editor"};
    }
}
