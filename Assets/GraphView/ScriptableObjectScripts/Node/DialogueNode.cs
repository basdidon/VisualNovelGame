using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Graphview.NodeData
{
    using NodeView;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEditor;

    public record DialogueRecord
    {
        public CharacterData CharacterData { get; }
        public string DialogueText { get; }

        public DialogueRecord(CharacterData characterData,string dialogueText)
        {
            CharacterData = characterData;
            DialogueText = dialogueText ?? string.Empty;
        }
    }

    public class DialogueNode : NodeData, IExecutableNode
    {
        [field: SerializeField] public CharacterData CharacterData { get; private set; }
        [field: SerializeField, TextArea]
        public string DialogueText { get; set; }

        public DialogueRecord GetData => new(CharacterData, GetValueFromSyntax(DialogueText));

        // Flow Port
        [field: SerializeField] public PortData InputFlowPortData { get; private set; }
        [field: SerializeField] public PortData OutputFlowPortData { get; private set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            DialogueText = string.Empty;

            SaveChanges();
        }

        public override void OnInstantiatePortData()
        {
            InputFlowPortData = InstantiatePortData(Direction.Input);
            OutputFlowPortData = InstantiatePortData(Direction.Output);
        }


        public void Start()
        {
            Debug.Log("dialogue node executing");
            DialogueManager.Instance.OnNewDialogueEventInvoke(GetData);
        }

        public void Exit(){}

        public void Next()
        {
            DialogueManager.Instance.CurrentNode = OutputFlowPortData.GetConnectedNodeOfType<IExecutableNode>().FirstOrDefault();
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
                for(int i = 0; i < match.Groups.Count; i++)
                {
                    Debug.Log($"{i} : {match.Groups[i].Value}");
                }
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

    [CustomGraphViewNode(typeof(DialogueNode))]
    public class CustomDialogueGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(NodeData nodeData)
        {
            if (nodeData is DialogueNode dialogueNode)
            {
                // input port
                var inputFlowPort = GetInputFlowPort(dialogueNode.InputFlowPortData.PortGuid);
                inputContainer.Add(inputFlowPort);

                // output port
                Port outputPort = GetOutputFlowPort(dialogueNode.OutputFlowPortData.PortGuid);
                outputContainer.Add(outputPort);

                // CharacterData ObjectField
                mainContainer.Insert(1, GetCharacterDataObjectField());
                
                // Custom extension
                VisualElement customVisualElement = new();

                List<string> options = new() { "1", "2" };

                ListView listView = new(options, 5);

                listView.makeItem = () => 
                {
                    return new Label();    
                };

                listView.bindItem = (item, index) =>
                {
                    item.Q<Label>().text = options[index];
                };



                DropdownField dropdown = new(options, 0);

                string[] _options = new string[] { "Cube", "Sphere", "Plane" };

                var label = new Label();
                customVisualElement.Add(label);

                var textArea = new TextField()
                {
                    multiline = true,
                    bindingPath = GetPropertyBindingPath("DialogueText")
                };
                textArea.RegisterValueChangedCallback(e =>
                {
                    if (ValidateText(e.newValue))
                    {
                        dropdown.style.display = DisplayStyle.Flex;

                        var idx = EditorGUILayout.Popup(0, _options);
                        Debug.Log(idx);
                    }
                    else
                    {
                        dropdown.style.display = DisplayStyle.None;
                    }

                    label.text = DialogueNode.GetValueFromSyntax(e.newValue);
                });
                customVisualElement.Add(textArea);
                customVisualElement.Add(dropdown);
                //customVisualElement.Add(listView);
                
                extensionContainer.Add(customVisualElement);

                // start with expanded state
                RefreshExpandedState();
            }
        }

        bool ValidateText(string text)
        {
            Regex regex = new(@"\[[^\[\]]*$");
            Match match = regex.Match(text);

            if (match.Success)
            {
                Debug.Log($"S:{text}");
                return true;
            }
            else
            {
                Debug.Log($"US:{text}");
                return false;
            }
        }
    }
}