namespace EinBotDB;

public static class StringExtensions
{
    public static string ToAlphaNumericDash(this string str)
    {
        char[] charArr = str.ToCharArray();
        charArr = Array.FindAll<char>(charArr, (ch => (char.IsLetterOrDigit(ch) || ch.Equals('-'))));
        return new string(charArr);
    }

    public static string ToAlphaNumeric(this string str)
    {
        char[] charArr = str.ToCharArray();
        charArr = Array.FindAll<char>(charArr, (ch => (char.IsLetterOrDigit(ch))));
        return new string(charArr);
    }
}
