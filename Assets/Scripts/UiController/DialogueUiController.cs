using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Linq;

public class DialogueUiController: IUiController
{
    VisualElement Root { get; set; }

    Label SpeakerNameLabel { get; set; }
    Label[] LineLabels { get; set; }


    DialogueNode DialogueNode { get; set; }

    // Input
    InputAction TapAction { get; set; }

    public bool IsDisplay => Root.style.display == DisplayStyle.Flex;

    // text anim
    readonly float txtAnimSpeed = .2f;  //per character
    Sequence mySequence;

    // Event
    public event DialogueManager.OnCompleted OnDisplayCompletedEvent;

    public DialogueUiController(VisualElement panel, InputAction tapAction)
    {
        Root = panel;

        SpeakerNameLabel = Root.Q<Label>("speaker-name-txt");
        LineLabels = new Label[]{
            Root.Q<Label>("line-1-txt"),
            Root.Q<Label>("line-2-txt"),
            Root.Q<Label>("line-3-txt")
        };

        mySequence = DOTween.Sequence();

        TapAction = tapAction;
        TapAction.performed += delegate
        {
            Debug.Log("tap");
            if (mySequence.IsActive() && mySequence.IsPlaying())
            {
                mySequence.Complete();
            }
            else
            {

                //DialogueManager.Instance.Next();
                OnDisplayCompletedEvent?.Invoke();
            }
        };

        DialogueManager.Instance.OnNewDialogue += OnNewDialogueHandle;
        DialogueManager.Instance.OnFinish += Hide;
    }

    /*
    public void DisplayDialogue(DialogueNode dialogueNode,OnCompleted onCompleted)
    {
        DialogueNode = dialogueNode;
        OnDisplayCompletedEvent += onCompleted;
        Display();
    }*/

    public void Display()
    {
        Root.AddToClassList("bottomsheet--up");
        TapAction.Enable();
    }

    public void Hide()
    {
        Root.RemoveFromClassList("bottomsheet--up");
        TapAction.Disable();
    }

    private void OnNewDialogueHandle(string[] textLine,DialogueManager.OnCompleted onCompleted)
    {
        Display();

        mySequence = DOTween.Sequence();

        var tweens = LineLabels.Select((v, i) => DOTween.To(() => v.text, x => v.text = x, textLine[i], textLine[i].Length > 0 ? txtAnimSpeed * textLine[i].Length : 0).SetEase(Ease.Linear));

        mySequence.AppendInterval(IsDisplay ? 0 : 1);

        foreach (var tween in tweens)
            mySequence.Append(tween);

        mySequence.OnPlay(() =>
        {
            SpeakerNameLabel.text = "SomeOnae";

            LineLabels[0].text = string.Empty;
            LineLabels[1].text = string.Empty;
            LineLabels[2].text = string.Empty;
        });

        OnDisplayCompletedEvent += onCompleted;
    }
}
