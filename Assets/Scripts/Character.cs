using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public UiDocumentController UDC => UiDocumentController.Instance;
    public DialogueUiController DialogueUC => UDC.DialogueUiController;
    public ChoicesPickerUiController ChoicesPickerUC => UDC.ChoicesPickerUiController;

    public abstract void Talk();
}
