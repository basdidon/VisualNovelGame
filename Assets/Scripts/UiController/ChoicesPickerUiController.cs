using UnityEngine;
using UnityEngine.UIElements;
using BasDidon.Dialogue;

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
        /*
        DialogueManager.Instance.OnNewDialogue += (_) =>
        {
            Hide();
        };
        DialogueManager.Instance.OnSelectChoices += (choices) =>
        {
            Display();
            SetChoices(choices.ChoicesText);
        };
        DialogueManager.Instance.OnFinish += Hide;
        */
    }

    public void SetChoices(string[] choices,bool[] isEnable)
    {
        // clear old choices
        while (Root[0].childCount > 0)
        {
            //Debug.Log("Remove()");
            Root[0].RemoveAt(0);
        }

        for(int i = 0; i < choices.Length; i++)
        {
            Button btn = new();
            btn.AddToClassList("choice-btn");       // style
            Root[0].Add(btn);                       // add to root
            btn.text = choices[i];
            btn.clicked += () => {
                DialogueManager.Instance.SelectChoice(btn.parent.IndexOf(btn));
            };

            if (isEnable[i] == false)
                btn.SetEnabled(false);
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
