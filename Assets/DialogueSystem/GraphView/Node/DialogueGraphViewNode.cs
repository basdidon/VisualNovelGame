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
                speakerTextField.AddToClassList("speaker-name-input");
                extensionContainer.Add(speakerTextField);

                VisualElement divider = new();
                divider.AddToClassList("divider");
                extensionContainer.Add(divider);

                // Custom extension
                VisualElement customVisualElement = new();

                string[] _options = new string[] { "Cube", "Sphere", "Plane" };

                var dialoguePreview = new Label();
                dialoguePreview.AddToClassList("dialogue-preview");
                customVisualElement.Add(dialoguePreview);

                var textArea = new TextField()
                {
                    multiline = true,
                    bindingPath = GetPropertyBindingPath("DialogueText")
                };
                textArea.RegisterValueChangedCallback(e =>
                {
                    dialoguePreview.text = DialogueNode.GetValueFromSyntax(e.newValue);
                });
                customVisualElement.Add(textArea);

                extensionContainer.Add(customVisualElement);

                // start with expanded state
                RefreshExpandedState();
            }
        }
        /*
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
        }*/
    }
}