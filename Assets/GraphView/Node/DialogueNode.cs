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
        [field: SerializeField] public CharacterData CharacterData { get; set; }
        [field: SerializeField, TextArea]
        public string DialogueText { get;set; }
         
        public DialogueRecord GetData => new(CharacterData, DialogueText);

        public override void Execute()
        {
            DialogueManager.Instance.OnNewDialogueEventInvoke(GetData);
        }

        [SerializeField] string outputPortGuid;
        public override string[] OutputPortGuids => new string[] { outputPortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            DialogueText = string.Empty;

            outputPortGuid = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
        }

        [field: SerializeField] GVNodeData Child { get; set; }

        public override void AddChild(GVNodeData child)
        {
            Child = child;
        }

        public override void RemoveChild(GVNodeData child)
        {
            if (Child == child)
                Child = null;
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return new GVNodeData[] { Child };
        }
    }

    [CustomGraphViewNode(typeof(DialogueNode))]
    public class CustomDialogueGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is DialogueNode dialogueNode)
            {
                DrawInputPort();

                // output port
                Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
                outputPort.viewDataKey = nodeData.OutputPortGuids[0];
                outputPort.portName = "Output";
                outputContainer.Add(outputPort);

                // CharacterData ObjectField
                ObjectField characterDataObjectField = new()
                {
                    objectType = typeof(CharacterData),
                    value = dialogueNode.CharacterData,
                };
                characterDataObjectField.RegisterValueChangedCallback(e => dialogueNode.CharacterData = (CharacterData)e.newValue);
                
                mainContainer.Insert(1, characterDataObjectField);
                // Custom extension
                VisualElement customVisualElement = new();
                var textArea = new TextField()
                {
                    value = dialogueNode.DialogueText,
                    multiline = true,
                };
                textArea.RegisterValueChangedCallback((e) => dialogueNode.DialogueText = e.newValue);
                customVisualElement.Add(textArea);
                
                extensionContainer.Add(customVisualElement);

                // start with expanded state
                RefreshExpandedState();
            }
        }
    }
}