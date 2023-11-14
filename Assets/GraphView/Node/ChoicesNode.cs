using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace Graphview.NodeData
{
    public class ChoicesNode : GVNodeData
    {
        [field: SerializeField] List<GVNodeData> Children { get; set; }

        public override string[] OutputPortGuids => choices.Select(choice => choice.OutputPortGuid).ToArray();

        [SerializeField] List<Choice> choices;
        public string[] Choices => choices.Select(choice => choice.Name).ToArray();

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            Children = new List<GVNodeData>();
            choices = new();
        }

        public override void Draw(Node node)
        {
            Button addCondition = new() { text = "Add Choice" };
            addCondition.clicked += () => OnAddChoice(node);
            node.mainContainer.Insert(1, addCondition);

            DrawInputPort(node);

            // output port
            foreach (var choice in choices)
            {
                DrawChoicePort(node, choice);
            }
        }

        void DrawChoicePort(Node node, Choice choice)
        {
            Port choicePort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            choicePort.portName = string.Empty;
            choicePort.viewDataKey = choice.OutputPortGuid;
            Button deleteChoiceBtn = new() { text = "X" };
            TextField choiceTxtField = new() { value = choice.Name };
            choiceTxtField.style.width = 100;
            choicePort.Add(choiceTxtField);
            choicePort.Add(deleteChoiceBtn);

            node.outputContainer.Add(choicePort);

            choiceTxtField.RegisterCallback<InputEvent>((ev) =>
            {
                TextField txtField = ev.currentTarget as TextField;
                choice.Name = ev.newData;
            });

            deleteChoiceBtn.clicked += () =>
            {
                node.outputContainer.Remove(choicePort);
                choices.Remove(choice);
            };
        }

        public override void AddChild(GVNodeData child)
        {
            Children.Add(child);
        }

        public override void RemoveChild(GVNodeData child)
        {
            Children.Remove(child);
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return Children;
        }

        public void OnCompleted(int choiceIdx)
        {
            Debug.Log($"you choose : {choiceIdx}");
            if (choiceIdx >= 0 && choiceIdx < choices.Count)
            {
                DialogueManager.Instance.CurrentNode = choices[choiceIdx].Child;
                
            }
        }

        void OnAddChoice(Node node)
        {
            Choice choice = new();
            choices.Add(choice);

            DrawChoicePort(node, choice);
        }

        public void Disconnect(string portGuid)
        {
            var portIdx = choices.FindIndex(choice => choice.OutputPortGuid == portGuid);
            if (portIdx >= 0)
            {
                choices[portIdx].Child = null;
            }
        }

        public void Connect(string portGuid, GVNodeData child)
        {
            var portIdx = choices.FindIndex(choice => choice.OutputPortGuid == portGuid);
            if (portIdx >= 0)
            {
                choices[portIdx].Child = child;
            }
        }


        [System.Serializable]
        class Choice
        {
            [field: SerializeField] public string Name { get; set; }
            [field: SerializeField] public string OutputPortGuid { get; set; }
            [field: SerializeField] public GVNodeData Child { get; set; }

            public Choice()
            {
                Name = "new choice";
                OutputPortGuid = $"{Guid.NewGuid()}";
            }

        }
    }
}