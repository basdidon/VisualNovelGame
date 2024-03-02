using UnityEngine;
using System;
using System.Linq;

namespace BasDidon.Dialogue
{
    using VisualGraphView;

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

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
                    Debug.Log("DialogueTree ended.");
                    OnCustomEvent?.Invoke(new DialogueEndEvent());
                }
            }
        }

        public event Action<ICustomEvent> OnCustomEvent;

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

        public void ToNextExecutableNode(PortData outputPort, DialogueTree tree)
        {
            if (outputPort == null)
                throw new ArgumentNullException();
            var selectedNode = tree.GetConnectedNodes<IExecutableNode>(outputPort).FirstOrDefault();
            CurrentNode = selectedNode;
        }

        // for Node invoke
        internal void FireEvent(ICustomEvent customEvent)
        {
            OnCustomEvent?.Invoke(customEvent);
        }

        // user input to node
        public void ExecuteAction(IBaseAction action)
        {
            currentNode.Action(action);
        }
    }
}