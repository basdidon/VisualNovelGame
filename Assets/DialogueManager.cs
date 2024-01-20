using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Graphview.NodeData;

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

            if (CurrentNode is DialogueNode dialogueNode)
            {
                OnNewDialogue?.Invoke(dialogueNode.TextLine,dialogueNode.OnCompleted);
            }
            else if(CurrentNode is ChoicesNode choicesNode)
            {
                ChoicesNodeOutput output = new() { 
                    SpeakerName = choicesNode.SpeakerName,
                    QuestionText = choicesNode.QuestionText,
                    ChoicesText = choicesNode.Choices
                };
                OnSelectChoices?.Invoke(output,choicesNode.OnCompleted);
            }
            else if(CurrentNode == null)
            {
                Debug.Log("finist");
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

    public void StartDialogue()
    {
        DialogueTree.StartNode.Execute();
    }

    public delegate void OnCompleted();
    public delegate void OnCompleted<T>(T obj);

    public event Action<string[], OnCompleted> OnNewDialogue;
    public event Action<ChoicesNodeOutput, OnCompleted<int>> OnSelectChoices;
    public event Action OnFinish;
}
