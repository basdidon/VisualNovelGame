using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class StartNode : BaseNode
{
    public override void Initialize(Vector2 position,DialogueTree dialogueTree)
    {
        base.Initialize(position,dialogueTree);
        name = "start-node";
        NodeName = "StartNode";
    }

    public override void Draw()
    {
        base.Draw();

        capabilities -= Capabilities.Deletable; // by defualt node can be delete,so i remove deletable

        titleButtonContainer.style.display = DisplayStyle.None;
        // output port
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);

        RefreshExpandedState();
    }

    protected override GVNodeData CreateNodeAsset()
    {
        return ScriptableObject.CreateInstance<SingleOutputNodeData>();
    }
}
