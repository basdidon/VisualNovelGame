using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace BasDidon.Dialogue.VisualGraphView
{
    public record DialogueRecord
    {
        public string SpeakerName { get; }
        public string DialogueText { get; }

        public DialogueRecord(string speakerName,string dialogueText)
        {
            SpeakerName = speakerName;
            DialogueText = dialogueText ?? string.Empty;
        }
    }

    public class DialogueNode : ExecutableNode
    {
        [NodeField]
        public string SpeakerName;

        [TextArea, NodeField]
        public string DialogueText  = string.Empty;

        public DialogueRecord GetData => new(SpeakerName, GetValueFromSyntax(DialogueText));

        public override void OnEnter()
        {
            Debug.Log("dialogue node executing");
            DialogueManager.Instance.OnNewDialogueEventInvoke(GetData);
        }

        public override void OnExit(){}

        public void Next()
        {
            DialogueManager.Instance.CurrentNode = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("Output")).FirstOrDefault();
        }

        public static string GetValueFromSyntax(string syntax)
        {
            // Define a regular expression pattern to extract the character ID
            Regex regex = new(@"\[(character|c):(\d+)\]",RegexOptions.IgnoreCase);

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