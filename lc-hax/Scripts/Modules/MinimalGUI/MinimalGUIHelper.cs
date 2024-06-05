#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace hax;

static class MinimalGUIHelper {
    static int indexCounter = 0;

    internal static Dictionary<int, Dictionary<string, string>> Texts = new();

    internal static void AddText(string key, string text) {
        bool keyExists = false;

        foreach (KeyValuePair<int, Dictionary<string, string>> kvp in Texts)
            if (kvp.Value.ContainsKey(key)) {
                kvp.Value[key] = text;
                keyExists = true;
                break; // Stop searching after updating the text
            }

        if (!keyExists) {
            // If key doesn't exist, add new text
            if (!Texts.ContainsKey(indexCounter)) Texts[indexCounter] = new Dictionary<string, string>();

            Texts[indexCounter].Add(key, text);
            indexCounter++;
        }
    }

    internal static void Remove(string key) {
        // Find and remove the key
        KeyValuePair<int, Dictionary<string, string>> kvpToRemove = Texts.FirstOrDefault(kvp => kvp.Value.ContainsKey(key));
        if (!kvpToRemove.Equals(default(KeyValuePair<int, Dictionary<string, string>>))) {
            kvpToRemove.Value.Remove(key);

            // Remove entries with empty inner dictionaries and update indexes
            Texts = Texts.Where(kvp => kvp.Value.Count > 0)
                .Select((kvp, index) => new { kvp.Key, Value = kvp.Value, Index = index })
                .ToDictionary(x => x.Index, x => x.Value);

            indexCounter--; // Decrement indexCounter since we removed an entry
        }
    }

    internal static void Clear() {
        Texts.Clear();
        indexCounter = 0;
    }

}
