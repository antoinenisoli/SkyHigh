using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoneyConverter
{
    private static readonly SortedDictionary<int, string> abbrevations = new SortedDictionary<int, string>
    {
         {1000,"K"},
         {10000000, "M" },
         {1000000000, "B" }
    };

    public static string Convert(float number)
    {
        // format this number to a money format defined in the abbreviations dictionary.
        for (int i = abbrevations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<int, string> pair = abbrevations.ElementAt(i);
            if (Mathf.Abs(number) >= pair.Key)
            {
                int roundedNumber = Mathf.FloorToInt(number / pair.Key);
                return roundedNumber.ToString() + pair.Value;
            }
        }

        return number.ToString();
    }
}
