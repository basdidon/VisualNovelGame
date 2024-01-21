using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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
                DialogueManager.Instance.StartDialogue();
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
            };

            // Dialogue Event
            DialogueManager.Instance.OnNewDialogue += (textline) =>
            {
                TapAction.Enable();

                DialogueUiController.SetDialogue("[SpeakerName]",textline);
                ChoicesPickerUiController.Hide();
            };

            DialogueManager.Instance.OnSelectChoices += (choiceOutput) => {
                TapAction.Disable();

                DialogueUiController.SetDialogue(choiceOutput.SpeakerName, new string[] { choiceOutput.QuestionText });

                ChoicesPickerUiController.SetChoices(choiceOutput.ChoicesText);
                ChoicesPickerUiController.Display();
            };

            DialogueManager.Instance.OnFinish += () =>
            {
                TapAction.Disable();

                DialogueUiController.Hide();
                ChoicesPickerUiController.Hide();
            };
        }
    }
}
