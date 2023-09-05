using System.Collections;
using System.Collections.Generic;
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

    // Input
    [field:SerializeField] public InputActionReference TapInputAction { get; set; }

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

            DialogueBox.userData = new DialogueUiController(DialogueBox,TapInputAction.action);

            ChatButton.clicked += delegate {
                Debug.Log("chat-btn was clicked");
                DialogueUiController controller = (DialogueUiController) DialogueBox.userData;
                controller.DialoguesData = DialoguesData;
                controller.Display();
            };
        }
    }
}
