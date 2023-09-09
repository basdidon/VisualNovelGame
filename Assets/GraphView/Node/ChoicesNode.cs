using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class ConditionNode : DialogueBaseNode
{
    List<BaseNode> Choices { get; set; }

    public override void Initialize(Vector2 position,DialogueTree dialogueTree)
    {
        base.Initialize(position,dialogueTree);
        NodeName = "Dialogue";
        Choices = new List<BaseNode>();
    }

    protected override GVNodeData CreateNodeAsset()
    {
        throw new System.NotImplementedException();
    }

    public override void Draw()
    {
        base.Draw();

        Button addCondition = new() { text = "Add Choice" };
        mainContainer.Insert(1,addCondition);

        // output port
        foreach(var choice in Choices)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            Button deleteChoiceBtn = new() { text = "X" };
            TextField choiceTxtField = new() { value = choice.NodeName } ;

            choicePort.Add(choiceTxtField);
            choicePort.Add(deleteChoiceBtn);

            outputContainer.Add(choicePort);
        }
    }
}
