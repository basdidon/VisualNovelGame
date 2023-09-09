using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseNode : Node
{
    public GVNodeData NodeData { get; private set; }
    public string NodeName { get; protected set; }

    protected abstract GVNodeData CreateNodeAsset();

    // recreate
    protected virtual void Setup(Vector2 position)
    {
        SetPosition(new Rect(position, Vector2.zero));
        userData = this;
    }

    // first time create
    public virtual void Initialize(Vector2 position,DialogueTree dialogueTree)
    {
        Setup(position);

        NodeName = "[BaseNode]";
        NodeData = CreateNodeAsset();
        NodeData.NodeType = GetType().Name;
        NodeData.GraphPosition = position;
        NodeData.name = GetType().Name;
        // save node asset
        AssetDatabase.AddObjectToAsset(NodeData, dialogueTree);
        AssetDatabase.SaveAssets();
        dialogueTree.Dialogues.Add(NodeData);
    }

    // load from GVNodeData
    public void LoadNodeData(GVNodeData nodeData)
    {
        Setup(nodeData.GraphPosition);

        NodeData = nodeData;
    }

    public virtual void Draw()
    {
        DrawHeader();
        // start with expanded state
        RefreshExpandedState();
    }

    void DrawHeader()
    {
        // textfield
        TextField dialogueNameTxt = new() { value = NodeName };
        titleContainer.Insert(1, dialogueNameTxt);
        dialogueNameTxt.style.display = DisplayStyle.None;
        // Title
        title = NodeName;
        VisualElement titleLabel = titleContainer.ElementAt(0);

        // double click event
        var clickable = new Clickable(ev =>
        {
            titleLabel.style.display = DisplayStyle.None;
            dialogueNameTxt.style.display = DisplayStyle.Flex;
            dialogueNameTxt.Focus();
            dialogueNameTxt.value = NodeName;
            dialogueNameTxt.RegisterCallback<FocusOutEvent>(ev =>
            {
                NodeName = dialogueNameTxt.value;
                title = NodeName;
                titleLabel.style.display = DisplayStyle.Flex;
                dialogueNameTxt.style.display = DisplayStyle.None;
            });
        });
        clickable.activators.Clear();
        clickable.activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 2 });  // double click
        titleLabel.AddManipulator(clickable);
    }
}

public abstract class DialogueBaseNode : BaseNode
{
    public int MaxLine { get; protected set; }
    public int MaxTextLength { get; protected set; }

    public override void Initialize(Vector2 position,DialogueTree dialogueTree)
    {
        base.Initialize(position,dialogueTree);

        MaxLine = 3;
        MaxTextLength = 25;
    }

    public override void Draw()
    {
        base.Draw();

        // input port
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "input";
        inputContainer.Add(inputPort);

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
        extensionContainer.Add(customVisualElement);

        // start with expanded state
        RefreshExpandedState();
    }

    void ValidateTextLine(InputEvent ev)
    {
        if (ev.newData.Length > MaxTextLength)
            (ev.currentTarget as TextField).value = ev.newData.Substring(0, MaxTextLength);
    }
}
