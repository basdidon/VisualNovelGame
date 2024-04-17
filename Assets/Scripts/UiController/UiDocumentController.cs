using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using H8.GraphView;
using H8.GraphView.NodeTemplate;

public class UiDocumentController : MonoBehaviour
{
    public static UiDocumentController Instance { get; private set; }

    GraphTreeController GraphTreeController { get; set; }

    VisualElement root;

    // VisualElements
    Button ChatButton { get; set; }
    VisualElement DialogueBox { get; set; }
    VisualElement ChoicesPicker { get; set; }

    // UiController
    public DialogueUiController DialogueUiController { get; private set; }
    public ChoicesPickerUiController ChoicesPickerUiController { get; private set; }

    // debug dialogue tree
    [field: SerializeField] public GraphTree DialogueTree { get; set; }

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

        GraphTreeController = new();

        if (TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement;

            ChatButton = root.Q<Button>("chat-btn");
            DialogueBox = root.Q("DialogueBox");

            DialogueUiController = new DialogueUiController(GraphTreeController,DialogueBox);
            DialogueBox.userData = DialogueUiController;

            ChatButton.clicked += delegate {
                Debug.Log("chat-btn was clicked");
                GraphTreeController.StartGraphTree(DialogueTree);
            };

            // ChoicesPicker
            ChoicesPicker = root.Q("ChoicesPicker");
            ChoicesPickerUiController = new ChoicesPickerUiController(GraphTreeController,ChoicesPicker);
            ChoicesPicker.userData = ChoicesPickerUiController;
            ChoicesPickerUiController.Hide();

            // input
            TapAction.performed += (_) =>
            {
                DialogueUiController.Next();
                Debug.Log("tap");
            };

            // Dialogue Event
            GraphTreeController.OnCustomEvent += (ctx) =>
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
