using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;
using H8.GraphView;
using H8.GraphView.NodeTemplate;

public class DialogueUiController: IUiController
{
    GraphTreeController GraphTreeController { get; }

    VisualElement Root { get; set; }
    
    Label SpeakerNameLabel { get; set; }
    Label TextDisplay { get; set; }

    string ShowBottomSheetClassName => "bottomsheet--up";

    public bool IsDisplay => Root.ClassListContains(ShowBottomSheetClassName);

    // text anim
    Sequence mySequence;
    float TextInputSpeed => .1f;

    public DialogueUiController(GraphTreeController graphTreeController, VisualElement panel)
    {
        GraphTreeController = graphTreeController;

        Root = panel;

        SpeakerNameLabel = Root.Q<Label>("speaker-name-txt");
        TextDisplay = Root.Q<Label>("text-display");
    }

    public void Display()
    {
        Root.AddToClassList(ShowBottomSheetClassName);
    }

    public void Hide()
    {
        Root.RemoveFromClassList(ShowBottomSheetClassName);
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
            GraphTreeController.ExecuteAction(new NextDialogueAction());
        }
    }

    public void SetDialogue(string speakerName, string dialogue)
    {
        if (string.IsNullOrEmpty(speakerName))
            speakerName = "Unknown";

        Debug.Log($"{speakerName} : {dialogue}");
        SpeakerNameLabel.text = speakerName;
        TextDisplay.text = dialogue;

        Display();
    }

    public void SetDialogueTyping(string speakerName, string dialogue)
    {
        if (speakerName == null)
            speakerName = "Unknown";
        // Create a new DOTween sequence
        mySequence = DOTween.Sequence();

        // Append an interval to the sequence based on whether the dialogue box is already displayed
        mySequence.AppendInterval(IsDisplay ? 0 : 1);

        // Append a DOTween animation to gradually update the TextDisplay.text with the dialogue
        mySequence.Append(DOTween.To(
            () => TextDisplay.text,            // Getter function
            x => TextDisplay.text = x,         // Setter function
            dialogue,                          // End value (dialogue to be displayed)
            TextInputSpeed * dialogue.Length   // Duration of the animation
        ).SetEase(Ease.Linear));

        // Set up a callback to be executed when the sequence starts playing
        mySequence.OnPlay(() =>
        {
            // Set the SpeakerNameLabel.text to the specified speakerName
            SpeakerNameLabel.text = speakerName;

            // Clear the TextDisplay.text before starting the animation
            TextDisplay.text = string.Empty;
        });

        // Start playing the DOTween sequence
        mySequence.Play();

        // Display the dialogue box
        Display();
    }
}
