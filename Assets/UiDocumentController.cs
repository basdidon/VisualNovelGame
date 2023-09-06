using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UiDocumentController : MonoBehaviour
{
    public static UiDocumentController Instance { get; private set; }

    VisualElement root;

    [field: SerializeField] public DialoguesData DialoguesData { get; set; }

    // VisualElements
    Button ChatButton { get; set; }
    VisualElement DialogueBox { get; set; }
    VisualElement ChoicesPicker { get; set; }

    // UiController
    public DialogueUiController DialogueUiController { get; private set; }
    public ChoicesPickerUiController ChoicesPickerUiController { get; private set; }

    // Input
    [field:SerializeField] public InputActionReference TapInputAction { get; set; }

    // Charecters
    Charecter Charecter_1 { get; set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Charecter_1 = new Bobo();

        if (TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement;

            ChatButton = root.Q<Button>("chat-btn");
            DialogueBox = root.Q("DialogueBox");

            DialogueUiController = new DialogueUiController(DialogueBox, TapInputAction.action);
            DialogueBox.userData = DialogueUiController;

            ChatButton.clicked += delegate {
                Debug.Log("chat-btn was clicked");
                Charecter_1.Talk();
               
            };

            // ChoicesPicker
            ChoicesPicker = root.Q("ChoicesPicker");
            ChoicesPickerUiController = new ChoicesPickerUiController(ChoicesPicker);
            ChoicesPicker.userData = ChoicesPickerUiController;
            ChoicesPickerUiController.Hide();
        }
    }

    /*
    public void DisplayDialogueBox(DialoguesData data,DialogueUiController.OnCompleted OnCompleted = null)
    {
        DialogueUiController controller = (DialogueUiController) DialogueBox.userData;
        controller.DialoguesData = data;
        controller.Display();
        controller.OnDisplayCompletedEvent += OnCompleted;
    }

    public void DisplayChoicesPicker(Choice[] choices)
    {
        ChoicesPickerUiController controller = (ChoicesPickerUiController) ChoicesPicker.userData;
        controller.SetChoices(choices);
        controller.Display();
    }*/
}
