using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseNode : Node
{
    public GVNodeData NodeData { get; set; }
    public string DialogueName { get; protected set; }

    public virtual void Initialize(Vector2 position)
    {
        DialogueName = "[BaseNode]";
        SetPosition(new Rect(position,Vector2.zero));
        userData = this;
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
        TextField dialogueNameTxt = new() { value = DialogueName };
        titleContainer.Insert(1, dialogueNameTxt);
        dialogueNameTxt.style.display = DisplayStyle.None;
        // Title
        title = DialogueName;
        VisualElement titleLabel = titleContainer.ElementAt(0);

        // double click event
        var clickable = new Clickable(ev =>
        {
            titleLabel.style.display = DisplayStyle.None;
            dialogueNameTxt.style.display = DisplayStyle.Flex;
            dialogueNameTxt.Focus();
            dialogueNameTxt.value = DialogueName;
            dialogueNameTxt.RegisterCallback<FocusOutEvent>(ev =>
            {
                DialogueName = dialogueNameTxt.value;
                title = DialogueName;
                titleLabel.style.display = DisplayStyle.Flex;
                dialogueNameTxt.style.display = DisplayStyle.None;
            });
        });
        clickable.activators.Clear();
        clickable.activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 2 });  // double click
        titleLabel.AddManipulator(clickable);
    }
}

public class DialogueBaseNode : BaseNode
{
    public int MaxLine { get; protected set; }
    public int MaxTextLength { get; protected set; }
    public DialogueNodeData DialoguesData { get; set; }

    public override void Initialize(Vector2 position)
    {
        base.Initialize(position);

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
