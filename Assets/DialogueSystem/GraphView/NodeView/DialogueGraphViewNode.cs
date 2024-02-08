using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(DialogueNode))]
    public class DialogueGraphViewNode : GraphViewNode
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
                TextField speakerTextField = new()
                {
                    label = "Speaker",
                    bindingPath = GetPropertyBindingPath("SpeakerName"),
                };
                mainContainer.Insert(1, speakerTextField);

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