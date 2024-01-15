using System;
using Quickenshtein;

namespace Hax;

public static partial class Helper {
    static int LongestCommonSubstring(ReadOnlySpan<char> query, ReadOnlySpan<char> original) {
        int originalLength = original.Length;
        int queryLength = query.Length;

        int[,] table = new int[2, originalLength + 1];
        int result = 0;

        for (int i = 1; i <= queryLength; i++) {
            for (int j = 1; j <= originalLength; j++) {
                if (query[i - 1] == original[j - 1]) {
                    table[i % 2, j] = table[(i - 1) % 2, j - 1] + 1;

                    if (table[i % 2, j] > result) {
                        result = table[i % 2, j];
                    }
                }

                else {
                    table[i % 2, j] = 0;
                }
            }
        }

        return result;
    }

    public static string? FuzzyMatch(ReadOnlySpan<char> query, ReadOnlySpan<string> strings) {
        int lowestWeight = int.MaxValue;
        string? closestMatch = null;

        for (int i = 0; i < strings.Length; i++) {
            int distance = Levenshtein.GetDistance(query, strings[i]);
            int commonalityReward = LongestCommonSubstring(query, strings[i]) * -2;
            int totalWeight = distance + commonalityReward;

            if (totalWeight < lowestWeight) {
                lowestWeight = totalWeight;
                closestMatch = strings[i];
            }
        }

        return closestMatch;
    }
}
