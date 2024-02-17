using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BasDidon
{
    public static class StringHelper
    {
        public static string ToCapitalCase(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : char.ToUpper(text[0]) + text[1..];
        }

        public static string GetBackingFieldName(string syntax)
        {
            Regex regex = new(@"<\w+>k__BackingField");

            Match match = regex.Match(syntax);

            return match.Success ? match.Groups[1].Value : syntax;
        }

        public static string GetValueFromSyntax(string syntax)
        {
            // Define a regular expression pattern to extract the character ID
            Regex regex = new(@"\[(character|c):(\d+)\]", RegexOptions.IgnoreCase);

            // Match the pattern in the syntax
            Match match = regex.Match(syntax);

            // If a match is found
            if (match.Success)
            {
                // Extract the character ID from the matched group
                int characterId = int.Parse(match.Groups[2].Value);

                // Call a function to get the character name based on the ID
                string characterName = GetCharacterNameById(characterId);

                // Replace the placeholder in the syntax with the actual character name
                syntax = syntax.Replace(match.Value, $"<color=#A8A8D8><b>{characterName}</b></color>");
            }

            return syntax;
        }

        // Function to get the character name by ID (dummy implementation)
        static string GetCharacterNameById(int characterId)
        {
            // In a real scenario, you would retrieve the character name from a database or another data source
            // For the sake of this example, I'll return a hardcoded value based on the ID
            return characterId switch
            {
                0 => "jame",
                _ => "unknown"
            };
        }
    }
}