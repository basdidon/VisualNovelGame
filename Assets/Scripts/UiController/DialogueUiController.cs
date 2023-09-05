using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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
    bool IsTextAnimationPlaying { get; set; }
    Coroutine TextAnimationRountine { get; set; }

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

        TapAction = tapAction;
        TapAction.performed += delegate
        {
            if (IsTextAnimationPlaying)
            {
                // stop animation
                UiDocumentController.Instance.StopCoroutine(TextAnimationRountine);
                // fill lebel with text
                LineLabels[0].text = Sentence.TextLine[0];
                LineLabels[1].text = Sentence.TextLine[1];
                LineLabels[2].text = Sentence.TextLine[2];
                IsTextAnimationPlaying = false;
                return;
            }

            DisplayNextDialogue();
        };
    }

    public void Display()
    {
        Debug.Log("Display");
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
            Hide();
            return;
        }

        SpeakerNameLabel.text = sentence.Speaker.ToString();

        LineLabels[0].text = "";
        LineLabels[1].text = "";
        LineLabels[2].text = "";
        Root.style.backgroundImage = new StyleBackground(sentence.Background);
            
        Sentence = sentence;
        TextAnimationRountine = UiDocumentController.Instance.StartCoroutine(DisplayTextRoutine(delay));

    }

    IEnumerator DisplayTextRoutine(float delay)
    {
        IsTextAnimationPlaying = true;
        yield return new WaitForSeconds(delay);
        yield return DisplayTextLine(0);
        yield return DisplayTextLine(1);
        yield return DisplayTextLine(2);
        IsTextAnimationPlaying = false;
    }

    IEnumerator DisplayTextLine(int lineIdx,int charIdx = 0)
    {
        yield return new WaitForSeconds(0.1f);
        LineLabels[lineIdx].text = charIdx < Sentence.TextLine[lineIdx].Length ?  Sentence.TextLine[lineIdx][0..(charIdx+1)] : "";

        if (charIdx+1 >= Sentence.TextLine[lineIdx].Length)
        {
            yield return null;
        }
        else
        {
            yield return DisplayTextLine(lineIdx, charIdx+1);
        }
    }
}
