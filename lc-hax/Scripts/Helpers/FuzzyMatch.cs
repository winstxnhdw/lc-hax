using Quickenshtein;

namespace Hax;

public static partial class Helper {
    public static string? FuzzyMatch(string query, string[] strings) {
        int lowestDistance = int.MaxValue;
        string? closestMatch = null;

        for (int i = 0; i < strings.Length; i++) {
            int distance = Levenshtein.GetDistance(query, strings[i]);

            if (distance < lowestDistance) {
                lowestDistance = distance;
                closestMatch = strings[i];
            }
        }

        return closestMatch;
    }
}
