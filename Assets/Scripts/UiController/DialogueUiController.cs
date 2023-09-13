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
    readonly float txtAnimSpeed = 2f;
    Sequence mySequence;

    // Event
    public delegate void OnCompleted();
    public event OnCompleted OnDisplayCompletedEvent;

    public DialogueUiController(VisualElement panel, InputAction tapAction)
    {
        Root = panel;

        SpeakerNameLabel = Root.Q<Label>("speaker-name-txt");
        LineLabels = new Label[]{
            Root.Q<Label>("line-1-txt"),
            Root.Q<Label>("line-2-txt"),
            Root.Q<Label>("line-3-txt")
        };

        //Sentences = new Queue<Sentence>();
        mySequence = DOTween.Sequence();

        TapAction = tapAction;
        TapAction.performed += delegate
        {
            if (mySequence.IsActive() && mySequence.IsPlaying())
            {
                mySequence.Complete();
            }
            else
            {
                OnDisplayCompletedEvent?.Invoke();
            }
        };
    }

    public void DisplayDialogue(DialogueNode dialogueNode,OnCompleted onCompleted)
    {
        DialogueNode = dialogueNode;
        OnDisplayCompletedEvent += onCompleted;
        Display();
    }

    public void Display()
    {
        Root.AddToClassList("bottomsheet--up");
        if (IsDisplay)
            DisplayNextDialogue();
        else
            DisplayNextDialogue(1);
        TapAction.Enable();
    }

    public void Hide()
    {
        Root.RemoveFromClassList("bottomsheet--up");
        TapAction.Disable();
    }

    void DisplayNextDialogue(float delay = 0)
    {
        mySequence = DOTween.Sequence();
        ﻿﻿﻿﻿﻿mySequence.PrependInterval(delay);
        var tweens = LineLabels.Select((v, i) => DOTween.To(() => v.text, x => v.text = x, DialogueNode.TextLine[i], txtAnimSpeed).SetEase(Ease.Linear));
        
        foreach(var tween in tweens)
            mySequence.Append(tween);


        mySequence.OnPlay(()=>
        {
            SpeakerNameLabel.text = "SomeOne";

            LineLabels[0].text = string.Empty;
            LineLabels[1].text = string.Empty;
            LineLabels[2].text = string.Empty;
            //Root.style.backgroundImage = new StyleBackground(Sentence.Background);
        });
    }
}
