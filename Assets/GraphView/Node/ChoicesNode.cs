using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ChoicesNode : GVNodeData
{
    [field: SerializeField] List<GVNodeData> Children { get; set; }

    [SerializeField] string inputPortGuid;
    public override string InputPortGuid => inputPortGuid;
    [SerializeField] List<string> outputPortGuids;
    public override string[] OutputPortGuids => outputPortGuids.ToArray();

    public override void Initialize(Vector2 position, DialogueTree dialogueTree)
    {
        base.Initialize(position, dialogueTree);
        Children = new List<GVNodeData>();
    }

    public override void Draw(Node node)
    {
        Button addCondition = new() { text = "Add Choice" };
        node.mainContainer.Insert(1, addCondition);

        // output port
        foreach (var child in Children)
        {
            Port choicePort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            Button deleteChoiceBtn = new() { text = "X" };
            TextField choiceTxtField = new() { value = child.name };

            choicePort.Add(choiceTxtField);
            choicePort.Add(deleteChoiceBtn);

            node.outputContainer.Add(choicePort);
        }
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

    public override void Execute()
    {
        throw new NotImplementedException();
    }
}
