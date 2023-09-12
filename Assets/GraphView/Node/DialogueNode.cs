using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : GVNodeData
{
    public int MaxLine => 3;
    public int MaxTextLength => 25;

    [SerializeField] string inputPortGuid;
    public override string InputPortGuid => inputPortGuid;
    [SerializeField] string outputPortGuid;
    public override string[] OutputPortGuids => new string[] { outputPortGuid };

    public Charecters Speaker { get; set; }
    
    [field:SerializeField] public string[] TextLine { get; set; }

    public override void Initialize(Vector2 position, DialogueTree dialogueTree)
    {
        Debug.Log("aaaa");

        base.Initialize(position, dialogueTree);

        TextLine = new string[MaxLine];
        for(int i =0;i<MaxLine;i++)
        {
            TextLine[i] = string.Empty;
        }
    }

    void ValidateTextLine(InputEvent ev,int lineIdx)
    {
        TextField txtField = ev.currentTarget as TextField;
        if (ev.newData.Length > MaxTextLength)
            txtField.value = ev.newData.Substring(0, MaxTextLength);
        TextLine[lineIdx] = txtField.value;
    }

    [field:SerializeField] GVNodeData Child { get; set; }

    public override void Draw(Node node)
    {
        // output port
        Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPortGuid ??= outputPort.viewDataKey;
        outputPort.viewDataKey = outputPortGuid;
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        // input port
        Port inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPortGuid ??= inputPort.viewDataKey;
        inputPort.viewDataKey = inputPortGuid;
        inputPort.portName = "input";
        node.inputContainer.Add(inputPort);

        // Custom extension
        VisualElement customVisualElement = new();
        Foldout txtFoldout = new() { text = "text" };

        foreach (var lineIdx in Enumerable.Range(0, MaxLine))
        {
            VisualElement container = new();
            Label lineLabel = new() { text = $"line_{lineIdx+1}" };
            TextField lineTxt = new()
            {
                value = TextLine[lineIdx] ?? string.Empty,
            };
            lineTxt.RegisterCallback<InputEvent>((ev)=>ValidateTextLine(ev,lineIdx));

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

    void OnValidate()
    {
        for (int i = 0; i < TextLine.Length; i++)
        {
            if (TextLine[i].Length > MaxTextLength)
            {
                TextLine[i] = TextLine[i].Substring(0, MaxTextLength);
            }
        }

        string[] _textLine = TextLine;              // can't pass property into ref parmeter, so i changed it to field
        if (TextLine.Length != MaxLine)
        {
            Array.Resize(ref _textLine, MaxLine);
            return;
        }
    }
}
