using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Linq;

public class DialogueUiController: IUiController
{
    VisualElement Root { get; set; }

    Label SpeakerNameLabel { get; set; }
    Label[] LineLabels { get; set; }

    public bool IsDisplay => Root.resolvedStyle.display == DisplayStyle.Flex;

    // text anim
    readonly Sequence mySequence;

    public DialogueUiController(VisualElement panel)
    {
        Root = panel;

        SpeakerNameLabel = Root.Q<Label>("speaker-name-txt");
        LineLabels = new Label[]{
            Root.Q<Label>("line-1-txt"),
            Root.Q<Label>("line-2-txt"),
            Root.Q<Label>("line-3-txt")
        };

        mySequence = DOTween.Sequence();
    }

    public void Display()
    {
        Root.AddToClassList("bottomsheet--up");
    }

    public void Hide()
    {
        Root.RemoveFromClassList("bottomsheet--up");
    }

    public void Next()
    {
        if (!IsDisplay)
            return;

        if (mySequence.IsActive() && mySequence.IsPlaying())
        {
            mySequence.Complete();
        }
        else
        {
            DialogueManager.Instance.ExecuteNextNode();
        }
    }

    public void SetDialogue(string speakerName,string[] textLine)
    {
        Display();

        SpeakerNameLabel.text = speakerName;
        for(int i = 0; i < LineLabels.Length; i++)
        {
            LineLabels[i].text = textLine.ElementAtOrDefault(i) ?? string.Empty;
        }

        /*
        mySequence = DOTween.Sequence();

        var tweens = LineLabels.Select((v, i) => DOTween.To(() => v.text, x => v.text = x, textLine[i], textLine[i].Length > 0 ? txtAnimSpeed * textLine[i].Length : 0).SetEase(Ease.Linear));

        mySequence.AppendInterval(IsDisplay ? 0 : 1);

        foreach (var tween in tweens)
            mySequence.Append(tween);

        mySequence.OnPlay(() =>
        {
            Debug.Log("on play");
            SpeakerNameLabel.text = "SomeOne";

            LineLabels[0].text = string.Empty;
            LineLabels[1].text = string.Empty;
            LineLabels[2].text = string.Empty;
        });

        */  
    }
}
