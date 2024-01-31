using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;


namespace Graphview.NodeData
{
    using NodeView;
    using System;

    public class QuestionNode : GVNodeData
    {
        // prop
        [field: SerializeField] public CharacterData CharacterData { get; private set; }
        [field: SerializeField, TextArea]
        public string QuestionText { get; set; }

        // port
        [SerializeField] PortData inputFlowPortData;
        public PortData InputFlowPortData => inputFlowPortData;

        [SerializeField] PortData inputChoicesPortData;
        public PortData InputChoicesPortData => inputChoicesPortData;

        [SerializeField] PortData outputFlowPortData;
        public PortData OutputFlowPortData => outputFlowPortData;

        // port guid
        public override string[] InputPortGuids => new string[] { InputFlowPortData.PortGuid, InputChoicesPortData.PortGuid };

        public override string[] OutputPortGuids => new string[] { OutputFlowPortData.PortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            inputFlowPortData = new(dialogueTree, Direction.Input);
            inputChoicesPortData = new(dialogueTree, Direction.Input);
            outputFlowPortData = new(dialogueTree, Direction.Output);
        }

        public override void Execute(){}

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return OutputFlowPortData.ConnectedNode;
        }
    }

    [CustomGraphViewNode(typeof(QuestionNode))]
    public class QuestionsGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if(nodeData is QuestionNode questionNode)
            {
                SerializedObject SO = new(questionNode);
                mainContainer.Bind(SO);

                // CharacterData ObjectField
                mainContainer.Insert(1, GetCharacterDataObjectField());

                Port inputFlowPort = GetInputFlowPort(questionNode.InputFlowPortData.PortGuid);
                inputContainer.Add(inputFlowPort);

                Port inputChoicesPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(ChoicesGraphData)); // change to choice later
                inputChoicesPort.portName = "Choices";
                inputContainer.Add(inputChoicesPort);

                Port outputFlowPort = GetOutputFlowPort(questionNode.OutputFlowPortData.PortGuid);
                outputContainer.Add(outputFlowPort);

                Port outputSelectedIdx = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
                outputSelectedIdx.portName = "selectedIndex";
                outputContainer.Add(outputSelectedIdx);

                // Custom extension
                VisualElement customVisualElement = new();

                var textArea = new TextField()
                {
                    bindingPath = GetPropertyBindingPath("QuestionText"),
                    multiline = true,
                };
                customVisualElement.Add(textArea);

                extensionContainer.Add(customVisualElement);

                RefreshExpandedState();
                
            }
        }
    }

}