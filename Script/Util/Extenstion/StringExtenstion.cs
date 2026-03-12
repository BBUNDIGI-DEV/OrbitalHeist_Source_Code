using UnityEngine;

public static class StringExtenstion
{
    public static string[] SplitWithIgnoreBrace(this string originalString, char ignoreOpenBrace, char ignoreCloseBrace, params char[] split)
    {
        int count = 0;
        int braceCount = 0;
        for (int i = 0; i < originalString.Length; i++)
        {
            if (originalString[i] == ignoreOpenBrace)
            {
                braceCount++;
            }
            else if (originalString[i] == ignoreCloseBrace)
            {
                braceCount--;
            }

            if (braceCount != 0)
            {
                continue;
            }

            for (int j = 0; j < split.Length; j++)
            {
                if(split[j] == originalString[i])
                {
                    count++;
                    break;
                }
            }
        }


        if(count == 0)
        {
            return new string[1] { originalString };
        }
        Debug.Assert(braceCount == 0, "Wrong Brace Count");
        string[] splitString = new string[count + 1];
        int start = 0;
        int splitStringIndex = 0;
        for (int i = 0; i < originalString.Length; i++)
        {
            if (originalString[i] == ignoreOpenBrace)
            {
                braceCount++;
            }
            else if(originalString[i] == ignoreCloseBrace)
            {
                braceCount--;
            }

            if (braceCount != 0)
            {
                continue;
            }

            for (int j = 0; j < split.Length; j++)
            {
                if (split[j] == originalString[i])
                {
                    splitString[splitStringIndex] = originalString.Substring(start, i - start);
                    start = i + 1;
                    splitStringIndex++;
                    break;
                }
            }
        }

        splitString[splitString.Length - 1] = originalString.Substring(start, originalString.Length - start);
        return splitString;
    }
}

