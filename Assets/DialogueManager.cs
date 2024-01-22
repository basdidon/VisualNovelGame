using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Graphview.NodeData;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [field: SerializeField] public DialogueTree DialogueTree { get; set; }

    public GVNodeData CurrentNode{ get; private set; }

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

    public void StartDialogue()
    {
        CurrentNode = DialogueTree.StartNode;
        ExecuteNextNode();
    }

    public event Action<string,string[]> OnNewDialogue;
    public event Action<ChoicesNodeOutput> OnSelectChoices;
    public event Action OnFinish;

    public void ExecuteNextNode(int idx = 0)
    {
        CurrentNode = CurrentNode.GetChildren().ElementAtOrDefault(idx);

        if (CurrentNode == null)
        {
            Debug.Log("finish");
            OnFinish?.Invoke();
            return;
        }

        if (CurrentNode is DialogueNode dialogueNode)
        {
            OnNewDialogue?.Invoke(
                dialogueNode.CharacterData != null ? dialogueNode.CharacterData.Name : null ?? "[Unknown]",
                dialogueNode.TextLine
            );
        }
        else if (CurrentNode is ChoicesNode choicesNode)
        {
            ChoicesNodeOutput output = new()
            {
                SpeakerName = choicesNode.SpeakerName,
                QuestionText = choicesNode.QuestionText,
                ChoicesText = choicesNode.Choices
            };
            OnSelectChoices?.Invoke(output);
        }
    }
}