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
                    OnCustomEvent?.Invoke(new DialogueEndEvent());
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

        public void ToNextExecutableNode(PortData outputPort, DialogueTree tree)
        {
            if (outputPort == null)
                throw new ArgumentNullException();
            var selectedNode = tree.GetConnectedNodes<IExecutableNode>(outputPort).FirstOrDefault();
            CurrentNode = selectedNode;
        }
    
        public event Action<ICustomEvent> OnCustomEvent;

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

    public interface ICustomEvent{}

    public class DialogueEndEvent : ICustomEvent{}
}