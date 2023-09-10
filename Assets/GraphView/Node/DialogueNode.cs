using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : GVNodeData
{
    public int MaxLine { get; protected set; }
    public int MaxTextLength { get; protected set; }

    public override void Initialize(Vector2 position, DialogueTree dialogueTree)
    {
        base.Initialize(position, dialogueTree);

        MaxLine = 3;
        MaxTextLength = 25;
    }

    void ValidateTextLine(InputEvent ev)
    {
        if (ev.newData.Length > MaxTextLength)
            (ev.currentTarget as TextField).value = ev.newData.Substring(0, MaxTextLength);
    }

    [field:SerializeField] GVNodeData Child { get; set; }

    public override void Draw(Node node)
    {
        // output port
        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // input port
        Port inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "input";
        node.inputContainer.Add(inputPort);

        // Custom extension
        VisualElement customVisualElement = new();
        Foldout txtFoldout = new() { text = "text" };

        foreach (var lineIdx in Enumerable.Range(1, MaxLine))
        {
            VisualElement container = new();
            Label lineLabel = new() { text = $"line_{lineIdx}" };
            TextField lineTxt = new();
            lineTxt.RegisterCallback<InputEvent>(ValidateTextLine);

            container.Add(lineLabel);
            container.Add(lineTxt);
            container.AddToClassList("line-container");

            txtFoldout.Add(container);
        }

        customVisualElement.Add(txtFoldout);
        node.extensionContainer.Add(customVisualElement);

        // start with expanded state
        node.RefreshExpandedState();
    }

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
