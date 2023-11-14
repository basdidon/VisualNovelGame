using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public interface IUiController
{
    public void Display();
    public void Hide();
    public bool IsDisplay { get; }
}

public class ChoicesPickerUiController : IUiController
{
    VisualElement Root { get; set; }

    public bool IsDisplay => Root.style.display == DisplayStyle.Flex;

    public ChoicesPickerUiController(VisualElement root)
    {
        Root = root;

        DialogueManager.Instance.OnNewDialogue += (_,_) =>
        {
            Hide();
        };
        DialogueManager.Instance.OnSelectChoices += (choices,callback) =>
        {
            Display();
            SetChoices(choices,callback);
        };
    }

    public void SetChoices(string[] choices,DialogueManager.OnCompleted<int> onSelected)
    {
        // clear old choices
        while (Root[0].childCount > 0)
        {
            Debug.Log("Remove()");
            Root[0].RemoveAt(0);
        }

        for(int i = 0; i < choices.Length; i++)
        {
            Button btn = new();
            btn.AddToClassList("choice-btn");       // style
            Root[0].Add(btn);                       // add to root
            btn.text = choices[i];
            btn.clicked += () => onSelected(btn.parent.IndexOf(btn));
        }
    }

    public void Display()
    {
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
    }
}
