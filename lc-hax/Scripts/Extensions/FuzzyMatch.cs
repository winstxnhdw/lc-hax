using System;
using System.Collections.Generic;
using Quickenshtein;

namespace Hax;

internal static partial class Extensions
{
    /// <summary>
    ///     Find the longest common substring between two strings.
    /// </summary>
    /// <returns>the length of the longest common substring</returns>
    private static int LongestCommonSubstring(ReadOnlySpan<char> query, ReadOnlySpan<char> original)
    {
        var originalLength = original.Length;
        var queryLength = query.Length;

        var table = new int[2, originalLength + 1];
        var result = 0;

        for (var i = 1; i <= queryLength; i++)
        for (var j = 1; j <= originalLength; j++)
            if (query[i - 1] == original[j - 1])
            {
                table[i % 2, j] = table[(i - 1) % 2, j - 1] + 1;

                if (table[i % 2, j] > result) result = table[i % 2, j];
            }

            else
            {
                table[i % 2, j] = 0;
            }

        return result;
    }

    /// <summary>
    ///     The similarity is calculated by first, penalising the Levenshtein distance between the two strings.
    ///     This alone is often not enough, so a reward is given for each character in the longest common substring.
    /// </summary>
    /// <returns>the total similarity weight</returns>
    private static int GetSimilarityWeight(string query, string original)
    {
        var distancePenalty = Levenshtein.GetDistance(query, original);
        var commonalityReward = LongestCommonSubstring(query, original) * -2;

        return distancePenalty + commonalityReward;
    }

    /// <summary>
    ///     Find the closest match to the query from a list of strings.
    ///     If the string is empty or the list is empty, we return null.
    /// </summary>
    /// <returns>true if a match was found, false otherwise</returns>
    internal static bool FuzzyMatchInternal(this string query, ReadOnlySpan<string> strings, out string closestMatch)
    {
        if (strings.Length is 0 || string.IsNullOrWhiteSpace(query))
        {
            closestMatch = "";
            return false;
        }

        closestMatch = strings[0];
        var lowestWeight = GetSimilarityWeight(query, strings[0]);

        for (var i = 1; i < strings.Length; i++)
        {
            var totalWeight = GetSimilarityWeight(query, strings[i]);

            if (totalWeight < lowestWeight)
            {
                lowestWeight = totalWeight;
                closestMatch = strings[i];
            }
        }

        return true;
    }

    /// <summary>
    ///     Find the closest match to the query from a list of strings.
    ///     If the string is empty or the list is empty, we return null.
    /// </summary>
    /// <returns>true if a match was found, false otherwise</returns>
    internal static bool FuzzyMatch(this string query, IEnumerable<string> strings, out string closestMatch)
    {
        return query.FuzzyMatchInternal([.. strings], out closestMatch);
    }
}