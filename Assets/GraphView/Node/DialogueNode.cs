using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueNode : DialogueBaseNode
{
    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);
    }

    public override void Draw()
    {
        base.Draw();

        // output port
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);

        RefreshExpandedState();
    }
}
