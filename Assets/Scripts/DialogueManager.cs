using UnityEngine;
using System;
using System.Linq;
using Graphview.NodeData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GVNodeData CurrentNode { get; set; }

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
        if (dialogueTree == null)
        {
            Debug.LogWarning("can't start when dialogueTree is null");
        }
        else
        {
            CurrentNode = dialogueTree.StartNode;
            ExecuteNextNode();
        }
    }

    // event for ui
    public event Action<DialogueRecord> OnNewDialogue;
    public event Action<ChoicesRecord> OnSelectChoices;
    public event Action OnFinish;

    // forNodeinvoke
    internal void OnNewDialogueEventInvoke(DialogueRecord record)
    {
        OnNewDialogue?.Invoke(record);
    }
    
    internal void OnSelectChoicesEvent(ChoicesRecord choicesRecord)
    {
        OnSelectChoices?.Invoke(choicesRecord);
    }
    
    public void ExecuteNextNode(int idx = 0)
    {
        if (CurrentNode == null)
        {
            Debug.Log("finish");
            OnFinish?.Invoke();
            return;
        }
        /*
        CurrentNode.Execute(idx);
        */

    }
}