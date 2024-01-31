using UnityEngine;
using System;
using System.Linq;
using Graphview.NodeData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    GVNodeData currentNode;
    public GVNodeData CurrentNode{ 
        get => currentNode;
        private set 
        {
            currentNode = value;
            if(CurrentNode != null)
                CurrentNode.Execute();
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
    }

    public void StartDialogue(DialogueTree dialogueTree)
    {
        CurrentNode = dialogueTree.StartNode;
        ExecuteNextNode();
    }

    // event for ui
    public event Action<DialogueRecord> OnNewDialogue;
    //public event Action<ChoicesRecord> OnSelectChoices;
    public event Action OnFinish;

    // forNodeinvoke
    internal void OnNewDialogueEventInvoke(DialogueRecord record)
    {
        OnNewDialogue?.Invoke(record);
    }
    /*
    internal void OnSelectChoicesEvent(ChoicesRecord choicesRecord)
    {
        OnSelectChoices?.Invoke(choicesRecord);
    }
    */
    public void ExecuteNextNode(int idx = 0)
    {
        CurrentNode = CurrentNode.GetChildren().ElementAtOrDefault(idx);

        if (CurrentNode == null)
        {
            Debug.Log("finish");
            OnFinish?.Invoke();
            return;
        }
    }
}