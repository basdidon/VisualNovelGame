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
}

public class Choice
{
    public string Text { get; private set; }
    public Action Callback { get; private set; }

    public Choice(string _text, Action callback)
    {
        Text = _text;
        Callback = callback;
    }
}

public class ChoicesPickerUiController : IUiController
{
    VisualElement Root { get; set; }

    public ChoicesPickerUiController(VisualElement root)
    {
        Root = root;
    }

    public void SetChoices(Choice[] choices)
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
            btn.text = choices[i].Text;
            btn.clicked += choices[i].Callback;
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
