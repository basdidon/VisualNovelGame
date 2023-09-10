using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphViewNode : Node
{
    public GVNodeData NodeData { get; private set; }

    public void Initialize(GVNodeData nodeData)
    {
        NodeData = nodeData;
        SetPosition(new Rect(nodeData.GraphPosition, Vector2.zero));
        viewDataKey = nodeData.Id;
        userData = nodeData;
        title = name = nodeData.GetType().Name;
        DrawHeader();
    }

    void DrawHeader()
    {
        // textfield
        TextField dialogueNameTxt = new() { value = title };
        titleContainer.Insert(1, dialogueNameTxt);
        dialogueNameTxt.style.display = DisplayStyle.None;
        // Title
        VisualElement titleLabel = titleContainer.ElementAt(0);

        // double click event
        var clickable = new Clickable(ev =>
        {
            titleLabel.style.display = DisplayStyle.None;
            dialogueNameTxt.style.display = DisplayStyle.Flex;
            dialogueNameTxt.Focus();
            dialogueNameTxt.value = title;
            dialogueNameTxt.RegisterCallback<FocusOutEvent>(ev =>
            {
                title = dialogueNameTxt.value;
                titleLabel.style.display = DisplayStyle.Flex;
                dialogueNameTxt.style.display = DisplayStyle.None;
            });
        });
        clickable.activators.Clear();
        clickable.activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 2 });  // double click
        titleLabel.AddManipulator(clickable);
    }
}
