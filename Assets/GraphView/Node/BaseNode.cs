using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseNode : Node
{
    public enum NodeTypes { DialogueNode, ConditionNode }

    string DialogueName { get; set; }
    public NodeTypes NodeType { get; protected set; }
    public int MaxLine { get; protected set; }
    public int MaxTextLength { get; protected set; }

    public virtual void Initialize(Vector2 position)
    {
        DialogueName = "hello boy";
        MaxLine = 3;
        MaxTextLength = 25;
        SetPosition(new Rect(position,Vector2.zero));
    }

    public virtual void Draw()
    {
        TextField dialogueNameTxt = new() { value = DialogueName };
        titleContainer.Insert(1, dialogueNameTxt);
        dialogueNameTxt.style.display = DisplayStyle.None;
        // Title
        title = DialogueName;
        VisualElement titleLabel = titleContainer.ElementAt(0);
        var clickable = new Clickable(ev =>
        {
            titleLabel.style.display = DisplayStyle.None;
            dialogueNameTxt.Focus();
            dialogueNameTxt.value = DialogueName;
            dialogueNameTxt.RegisterCallback<FocusOutEvent>(ev =>
            {
                DialogueName = dialogueNameTxt.value;
                title = DialogueName;
            });
        });
        clickable.activators.Clear();
        clickable.activators.Add(new ManipulatorActivationFilter(){ button = MouseButton.LeftMouse, clickCount = 2 });  // double click
        titleLabel.AddManipulator(clickable);
       


        // input port
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,typeof(bool));
        inputPort.portName = "input";
        inputContainer.Add(inputPort);

        // Custom extension
        VisualElement customVisualElement = new();
        Foldout txtFoldout = new() { text = "text" };

        foreach(var lineIdx in Enumerable.Range(1, MaxLine))
        {
            VisualElement container = new();
            Label lineLabel = new() { text = $"line_{lineIdx}" };
            TextField lineTxt = new();
            lineTxt.RegisterCallback<InputEvent>(ev => {
                if(ev.newData.Length > MaxTextLength)
                {
                    lineTxt.value = ev.newData.Substring(0, MaxTextLength);
                }    
            });

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
}
