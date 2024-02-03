using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.NodeData
{
    using NodeView;

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

    public class DialogueNode : GVNodeData
    {
        [field: SerializeField] public CharacterData CharacterData { get; private set; }
        [field: SerializeField, TextArea]
        public string DialogueText { get; set; }

        public DialogueRecord GetData
        {
            get
            {
                if (InputObjectPortData.GetConnectedNode().Count == 0)
                {
                    return new(CharacterData, DialogueText);
                }
                else
                {
                    if (InputObjectPortData.GetConnectedNode()[0].Values == null)
                    {
                        Debug.Log("asfasgf");
                    }
                    if (InputObjectPortData.GetConnectedNode()[0].Values.TryGetValue(InputObjectPortData.PortGuid, out object value))
                    {
                        return new(CharacterData, (string)value);
                    }
                    else
                    {
                        throw new Exception("not found data.");
                    }
                }
            }
        }
        /*
        public override void Execute()
        {
            Debug.Log("dialogue node executing");
            DialogueManager.Instance.OnNewDialogueEventInvoke(GetData);
        }
        */
        // Flow Port
        [field: SerializeField] public PortData InputFlowPortData { get; private set; }
        [field: SerializeField] public PortData OutputFlowPortData { get; private set; }

        // object
        [field: SerializeField] public PortData InputObjectPortData { get; private set; }

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

            InputObjectPortData = InstantiatePortData(Direction.Input);
        }
    }

    [CustomGraphViewNode(typeof(DialogueNode))]
    public class CustomDialogueGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is DialogueNode dialogueNode)
            {
                // input port
                var inputFlowPort = GetInputFlowPort(dialogueNode.InputFlowPortData.PortGuid);
                inputContainer.Add(inputFlowPort);

                // output port
                Port outputPort = GetOutputFlowPort(dialogueNode.OutputFlowPortData.PortGuid);
                outputContainer.Add(outputPort);

                // object input
                Port inputObjectPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(string));
                inputObjectPort.viewDataKey = dialogueNode.InputObjectPortData.PortGuid;
                inputContainer.Add(inputObjectPort);

                // CharacterData ObjectField
                mainContainer.Insert(1, GetCharacterDataObjectField());
                
                // Custom extension
                VisualElement customVisualElement = new();
                var textArea = new TextField()
                {
                    multiline = true,
                    bindingPath = GetPropertyBindingPath("DialogueText")
                };
                customVisualElement.Add(textArea);
                
                extensionContainer.Add(customVisualElement);

                // start with expanded state
                RefreshExpandedState();
            }
        }
    }
}