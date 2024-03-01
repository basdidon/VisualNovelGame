using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;
using BasDidon.Dialogue;
using BasDidon.Dialogue.VisualGraphView;

public class UiDocumentController : MonoBehaviour
{
    public static UiDocumentController Instance { get; private set; }

    VisualElement root;

    // VisualElements
    Button ChatButton { get; set; }
    VisualElement DialogueBox { get; set; }
    VisualElement ChoicesPicker { get; set; }

    // UiController
    public DialogueUiController DialogueUiController { get; private set; }
    public ChoicesPickerUiController ChoicesPickerUiController { get; private set; }

    // debug dialogue tree
    [field: SerializeField] public DialogueTree DialogueTree { get; set; }

    // Input
    [field:SerializeField] InputActionReference TapInputAction { get; set; }
    public InputAction TapAction => TapInputAction.action;

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

        if (TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement;

            ChatButton = root.Q<Button>("chat-btn");
            DialogueBox = root.Q("DialogueBox");

            DialogueUiController = new DialogueUiController(DialogueBox);
            DialogueBox.userData = DialogueUiController;

            ChatButton.clicked += delegate {
                Debug.Log("chat-btn was clicked");
                DialogueManager.Instance.StartDialogue(DialogueTree);
            };

            // ChoicesPicker
            ChoicesPicker = root.Q("ChoicesPicker");
            ChoicesPickerUiController = new ChoicesPickerUiController(ChoicesPicker);
            ChoicesPicker.userData = ChoicesPickerUiController;
            ChoicesPickerUiController.Hide();

            // input
            TapAction.performed += (_) =>
            {
                DialogueUiController.Next();
                Debug.Log("tap");
            };

            // Dialogue Event
            DialogueManager.Instance.OnCustomEvent += (ctx) =>
            {
                if (ctx is DialogueRecord dialogue)
                {
                    TapAction.Enable();

                    DialogueUiController.SetDialogue(dialogue.SpeakerName, dialogue.DialogueText);
                    ChoicesPickerUiController.Hide();
                }
                else if(ctx is ChoicesRecord choice)
                {
                    TapAction.Disable();

                    DialogueUiController.SetDialogue(choice.DialogueRecord.SpeakerName, choice.DialogueRecord.DialogueText);

                    ChoicesPickerUiController.SetChoices(choice.ChoiceRecords);
                    ChoicesPickerUiController.Display();
                }
                else if(ctx is DialogueEndEvent)
                {
                    TapAction.Disable();

                    DialogueUiController.Hide();
                    ChoicesPickerUiController.Hide();
                }
            };
        }
    }
}
