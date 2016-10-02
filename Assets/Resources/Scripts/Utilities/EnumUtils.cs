using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumUtils {
    
    #region Flags
    /// <summary>
    /// Compare to see if the provided flag a part of the overall enum value
    /// </summary>
    /// <param name="aValue">Source enum value to find the flag in</param>
    /// <param name="aFlag">Flag value</param>
    public static bool HasFlag(this Enum aValue, Enum aFlag) {
        return (Convert.ToUInt64(aValue) & Convert.ToUInt64(aFlag)) == Convert.ToUInt64(aFlag);
    }

    /// <summary>
    /// Compare to see if the provided flag a part of the overall enum value
    /// </summary>
    /// <param name="aValue">Source enum value to find the flag in</param>
    public static IEnumerable<Enum> GetFlags(this Enum aValue) {
        return GetFlags(aValue, Enum.GetValues(aValue.GetType()).Cast<Enum>().ToArray());
    }

    /// <summary>
    /// Compare to see if the provided flag a part of the overall enum value
    /// </summary>
    /// <param name="aValue">Source enum value to find the flag in</param>
    public static IEnumerable<Enum> GetIndividualFlags(this Enum aValue) {
        return GetFlags(aValue, GetFlagValues(aValue.GetType()).ToArray());
    }

    private static IEnumerable<Enum> GetFlags(Enum aValue, Enum[] aValues) {
        ulong bits = Convert.ToUInt64(aValue);
        List<Enum> results = new List<Enum>();
        for (int i = aValues.Length - 1; i >= 0; i--) {
            ulong mask = Convert.ToUInt64(aValues[i]);
            if (i == 0 && mask == 0L) {
                break;
            }

            if ((bits & mask) == mask) {
                results.Add(aValues[i]);
                bits -= mask;
            }
        }

        if (bits != 0L) {
            return Enumerable.Empty<Enum>();
        } else if (Convert.ToUInt64(aValue) != 0L) {
            return results.Reverse<Enum>();
        } else if (bits == Convert.ToUInt64(aValue) && aValues.Length > 0 && Convert.ToUInt64(aValues[0]) == 0L) {
            return aValues.Take(1);
        }
        return Enumerable.Empty<Enum>();
    }

    private static IEnumerable<Enum> GetFlagValues(Type aEnumType) {
        ulong flag = 0x1;
        foreach (var value in Enum.GetValues(aEnumType).Cast<Enum>()) {
            ulong bits = Convert.ToUInt64(value);
            if (bits == 0L) {
                continue; // skip the zero value
            }

            while (flag < bits) {
                flag <<= 1;
            }

            if (flag == bits) {
                yield return value;
            }
        }
    }
    #endregion

}
