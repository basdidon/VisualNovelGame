using UnityEngine;
using System;

namespace BasDidon.Dialogue
{
    using System.Linq;
    using VisualGraphView;

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [field: SerializeField] public DialogueDatabase DialogueDatabase { get; set; }


        IExecutableNode currentNode;
        public IExecutableNode CurrentNode
        {
            get => currentNode;
            private set
            {
                CurrentNode?.OnExit();
                currentNode = value;
                if (CurrentNode != null)
                {
                    CurrentNode.OnEnter();
                }
                else
                {
                    Debug.Log("finish");
                    OnFinish?.Invoke();
                }
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
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
            }
        }

        public void SetNextNode(PortData outputPort, DialogueTree tree)
        {
            if (outputPort == null)
                throw new ArgumentNullException();
            var selectedNode = tree.GetConnectedNodes<IExecutableNode>(outputPort).FirstOrDefault();
            CurrentNode = selectedNode;
        }

        // event for ui
        public event Action<DialogueRecord> OnNewDialogue;
        public event Action<ChoicesRecord> OnSelectChoices;
        public event Action OnFinish;

        // forNodeinvoke
        internal void OnNewDialogueEventInvoke(DialogueRecord record)
        {
            Debug.Log($"OnNewDialogue : {record.SpeakerName ?? "Unknown"}.");
            OnNewDialogue?.Invoke(record);
        }

        internal void OnSelectChoicesEvent(ChoicesRecord choicesRecord)
        {
            OnSelectChoices?.Invoke(choicesRecord);
        }

        // user input to node
        public void NextDialogue()
        {
            if (CurrentNode is DialogueNode dialogueNode)
            {
                dialogueNode.Next();
            }
        }

        public void SelectChoice(int choiceIdx)
        {
            if (CurrentNode is ChoicesNode choicesNode)
            {
                choicesNode.SelectChoice(choiceIdx);
            }
        }

        public void SendEvent(string jsonString)
        {
            
        }
    }
}