using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [field: SerializeField] public DialogueTree DialogueTree { get; set; }

    GVNodeData currentNode;
    public GVNodeData CurrentNode
    {
        get => currentNode;
        set
        {
            currentNode = value;

            if (currentNode is DialogueNode dialogueNode)
            {
                OnNewDialogue?.Invoke(dialogueNode.TextLine,dialogueNode.Next);
            }
            else if(CurrentNode == null)
            {
                OnFinish?.Invoke();
            }
        }
    }

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

        currentNode = DialogueTree.StartNode;
    }


    void Next()
    {
        if (CurrentNode == null)
            return;
        CurrentNode.Next();
    }

    public void StartDialogue()
    {
        CurrentNode = DialogueTree.StartNode;
        Next();
    }

    public event Action OnNodeCompleted;
    public delegate void OnCompleted();


    public event Action<string[], OnCompleted> OnNewDialogue;
    public event Action<string[]> OnSelectChoices;
    public event Action OnFinish;

}
