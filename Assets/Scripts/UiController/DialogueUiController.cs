using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueUiController : MonoBehaviour
{
    VisualElement root;
    VisualElement backgroundElement;
    Label speakerNameLabel;
    Label line1Label;
    Label line2Label;
    Label line3Label;

    [SerializeField] DialoguesData dialoguesData;
    Queue<Sentence> Sentences { get; set; }

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement;
            backgroundElement = root.Q("background");
            speakerNameLabel = root.Q<Label>("speaker-name-txt");
            line1Label = root.Q<Label>("line-1-txt");
            line2Label = root.Q<Label>("line-2-txt");
            line3Label = root.Q<Label>("line-3-txt");

            Sentences = new Queue<Sentence>();
            foreach(var sentance in dialoguesData.Dialogues)
            {
                Sentences.Enqueue(sentance);
            }

            DisplayNextDialogue();
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("space");
            DisplayNextDialogue();
        }
    }

    void DisplayNextDialogue()
    {
        if(Sentences.TryDequeue(out Sentence sentence))
        {
            speakerNameLabel.text = sentence.Speaker.ToString();
            line1Label.text = sentence.TextLine[0];
            line2Label.text = sentence.TextLine[1];
            line3Label.text = sentence.TextLine[2];
            backgroundElement.style.backgroundImage = new StyleBackground(sentence.Background);
        }
        else
        {
            // end of dialogue
        }
    }

}
