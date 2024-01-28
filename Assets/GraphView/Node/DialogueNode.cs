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

        [SerializeField] string inputPortGuid;
        public override string[] InputPortGuids => new string[] { inputPortGuid };

        [SerializeField] string outputPortGuid;
        public override string[] OutputPortGuids => new string[] { outputPortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            DialogueText = string.Empty;

            inputPortGuid = Guid.NewGuid().ToString();
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
                SerializedObject SO = new(dialogueNode);
                mainContainer.Bind(SO);

                var inputFlowPort = GetInputFlowPort();
                inputFlowPort.viewDataKey = dialogueNode.InputPortGuids[0];
                inputContainer.Add(inputFlowPort);

                // output port
                Port outputPort = GetOutputFlowPort();
                outputPort.viewDataKey = nodeData.OutputPortGuids[0];
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