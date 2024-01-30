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
            DialogueText = dialogueText;
        }
    }

    public class DialogueNode : GVNodeData
    {
        [field: SerializeField] public CharacterData CharacterData { get; private set; }
        [field: SerializeField, TextArea]
        public string DialogueText { get; set; }
        
        public DialogueRecord GetData => new(CharacterData, DialogueText);

        public override void Execute()
        {
            DialogueManager.Instance.OnNewDialogueEventInvoke(GetData);
        }

        // port
        [SerializeField] PortData inputFlowPortData;
        public PortData InputFlowPortData => inputFlowPortData;

        [SerializeField] PortData outputFlowPortData;
        public PortData OutputFlowPortData => outputFlowPortData;

        public override string[] InputPortGuids => new string[] { InputFlowPortData.PortGuid };
        public override string[] OutputPortGuids => new string[] { OutputFlowPortData.PortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            DialogueText = string.Empty;

            inputFlowPortData = new(DialogueTree, Direction.Input);
            outputFlowPortData = new(DialogueTree, Direction.Output);

            SaveChanges();
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return new GVNodeData[] { OutputFlowPortData.ConnectedNode.Single() };
        }
    }

    [CustomGraphViewNode(typeof(DialogueNode))]
    public class CustomDialogueGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is DialogueNode dialogueNode)
            {
                SerializedObject SO = new(dialogueNode);
                mainContainer.Bind(SO);

                // input port
                var inputFlowPort = GetInputFlowPort(dialogueNode.InputPortGuids.Single());
                inputContainer.Add(inputFlowPort);

                // output port
                Port outputPort = GetOutputFlowPort(nodeData.OutputPortGuids.Single());
                outputContainer.Add(outputPort);

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