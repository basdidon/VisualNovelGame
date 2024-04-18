using System.Text;
using System.Text.RegularExpressions;

namespace H8.FlowGraph
{
    public static class StringHelper
    {
        public static string FieldNameToTextLabel(string text)
        {
            StringBuilder builder = new();

            if (string.IsNullOrEmpty(text))
                return builder.ToString();

            for(int i = 0; i < text.Length; i++)
            {
                if (i == 0)
                {
                    builder.Append(char.ToUpper(text[0]));
                    continue;
                }

                if (char.IsUpper(text[i]))
                    builder.Append(" ");

                builder.Append(text[i]);
            }

            return builder.ToString();
        }

        public static string ToPascalCase(string text)
        {
            StringBuilder builder = new();

            if (string.IsNullOrEmpty(text))
                return builder.ToString();

            for (int i = 0; i < text.Length; i++)
            {
                if (i == 0 || char.IsWhiteSpace(text[i - 1]))
                {
                    builder.Append(char.ToUpper(text[i]));
                }
                else
                {
                    builder.Append(char.ToLower(text[i]));
                }
            }

            return builder.ToString();
        }

        public static string GetBackingFieldName(string syntax)
        {
            Regex regex = new(@"<(?<FieldName>\w+)>k__BackingField");

            Match match = regex.Match(syntax);

            return match.Success ? match.Groups["FieldName"].Value : syntax;
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