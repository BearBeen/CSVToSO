using System;

public static partial class StringExtension
{
    public static string NormalizeUnityPath(this string input)
    {
        ReadOnlySpan<char> inputSpan = input.AsSpan();
        Span<char> resultSpan = stackalloc char[inputSpan.Length];
        int resultLength = 0;
        char c;
        for (int i = 0; i < inputSpan.Length; i++)
        {
            c = inputSpan[i];
            if (c == '\\')
            {
                resultSpan[resultLength++] = '/';
            }
            else
            {
                resultSpan[resultLength++] = c;
            }
        }
        return new string(resultSpan[..resultLength]);
    }

    public static string RemoveWhitespace(this string input)
    {
        ReadOnlySpan<char> inputSpan = input.AsSpan();
        Span<char> resultSpan = stackalloc char[inputSpan.Length];
        int resultLength = 0;
        char c;
        for (int i = 0; i < inputSpan.Length; i++)
        {
            c = inputSpan[i];
            if (!char.IsWhiteSpace(c))
            {
                resultSpan[resultLength++] = c;
            }
        }
        return new string(resultSpan[..resultLength]);
    }

    public static string RemoveWhitespaceAndExtra(this string input, char extra1, char extra2)
    {
        ReadOnlySpan<char> inputSpan = input.AsSpan();
        Span<char> resultSpan = stackalloc char[inputSpan.Length];
        int resultLength = 0;
        char c;
        for (int i = 0; i < inputSpan.Length; i++)
        {
            c = inputSpan[i];
            if (!char.IsWhiteSpace(c) && c != extra1 && c != extra2)
            {
                resultSpan[resultLength++] = c;
            }
        }
        return new string(resultSpan[..resultLength]);
    }
}
