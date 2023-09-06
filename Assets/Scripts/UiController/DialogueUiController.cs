using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Linq;

public class DialogueUiController
{
    VisualElement Root { get; set; }

    Label SpeakerNameLabel { get; set; }
    Label[] LineLabels { get; set; }

    DialoguesData dialoguesData;
    public DialoguesData DialoguesData
    {
        get => dialoguesData;
        set
        {
            dialoguesData = value;

            Sentences.Clear();
            foreach (var sentance in dialoguesData.Dialogues)
            {
                Sentences.Enqueue(sentance);
            }
        }
    }

    Queue<Sentence> Sentences { get; set; }
    Sentence Sentence { get; set; }

    // Input
    InputAction TapAction { get; set; }

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

        Sentences = new Queue<Sentence>();
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
                DisplayNextDialogue();
            }
        };


    }

    public void Display()
    {
        Root.AddToClassList("bottomsheet--up");
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
        if(!Sentences.TryDequeue(out Sentence sentence))
        {
            OnDisplayCompletedEvent?.Invoke();
            return;
        }
            
        Sentence = sentence;

        mySequence = DOTween.Sequence();
        ﻿﻿﻿﻿﻿mySequence.PrependInterval(delay);
        var tweens = LineLabels.Select((v, i) => DOTween.To(() => v.text, x => v.text = x, Sentence.TextLine[i], txtAnimSpeed).SetEase(Ease.Linear));
        
        foreach(var tween in tweens)
            mySequence.Append(tween);

        mySequence.OnPlay(()=> 
        {
            SpeakerNameLabel.text = Sentence.Speaker.ToString();

            LineLabels[0].text = string.Empty;
            LineLabels[1].text = string.Empty;
            LineLabels[2].text = string.Empty;
            Root.style.backgroundImage = new StyleBackground(Sentence.Background);
        });
    }
}
