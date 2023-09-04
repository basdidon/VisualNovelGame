using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class DialogueUiController
{
    VisualElement Root { get; set; }

    Label SpeakerNameLabel { get; set; }
    Label Line1Label { get; set; }
    Label Line2Label { get; set; }
    Label Line3Label { get; set; }

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

    // Input
    InputAction TapAction { get; set; }

    public DialogueUiController(VisualElement panel, InputAction tapAction)
    {
        Root = panel;

        SpeakerNameLabel = Root.Q<Label>("speaker-name-txt");
        Line1Label = Root.Q<Label>("line-1-txt");
        Line2Label = Root.Q<Label>("line-2-txt");
        Line3Label = Root.Q<Label>("line-3-txt");

        Sentences = new Queue<Sentence>();

        TapAction = tapAction;
        TapAction.performed += delegate
        {
            DisplayNextDialogue();
        };
    }

    public void Display()
    {
        Debug.Log("Display");
        DisplayNextDialogue();
        Root.AddToClassList("bottomsheet--up");
        TapAction.Enable();
    }

    public void Hide()
    {
        Root.RemoveFromClassList("bottomsheet--up");
        TapAction.Disable();
    }

    void DisplayNextDialogue()
    {
        if(Sentences.TryDequeue(out Sentence sentence))
        {
            SpeakerNameLabel.text = sentence.Speaker.ToString();
            Line1Label.text = sentence.TextLine[0];
            Line2Label.text = sentence.TextLine[1];
            Line3Label.text = sentence.TextLine[2];
            Root.style.backgroundImage = new StyleBackground(sentence.Background);
        }
        else
        {
            Hide();
            // end of dialogue
        }
    }

}
