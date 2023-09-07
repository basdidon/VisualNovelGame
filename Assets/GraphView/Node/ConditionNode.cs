using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ConditionNode : BaseNode
{
    List<string> Choices { get; set; }

    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

        Choices = new List<string>();
        NodeType = NodeTypes.DialogueNode;
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
            TextField choiceTxtField = new() { value = choice } ;

            choicePort.Add(choiceTxtField);
            choicePort.Add(deleteChoiceBtn);

            outputContainer.Add(choicePort);
        }
    }
}
