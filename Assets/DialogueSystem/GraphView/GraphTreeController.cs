using UnityEngine;
using System;
using System.Linq;

namespace H8.GraphView
{
    public class GraphTreeController
    {
        IExecutableNode currentNode;
        public IExecutableNode CurrentNode
        {
            get => currentNode;
            private set
            {
                CurrentNode?.OnExit(this);
                currentNode = value;
                if (CurrentNode != null)
                {
                    Debug.Log($"<color=yellow>{CurrentNode.GetType().Name}</color> is Executing.");
                    CurrentNode.OnEnter(this);
                }
                else
                {
                    Debug.Log("GraphTree ended.");
                    OnCustomEvent?.Invoke(new DialogueEndEvent());
                }
            }
        }

        public event Action<ICustomEvent> OnCustomEvent;
 
        public void StartGraphTree(GraphTree dialogueTree)
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

        public void ToNextExecutableNode(PortData outputPort, GraphTree tree)
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
            currentNode.Action(this, action);
        }
    }
}