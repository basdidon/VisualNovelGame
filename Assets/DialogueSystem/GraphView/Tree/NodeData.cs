using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;

namespace BasDidon.Dialogue.VisualGraphView
{
    public abstract class NodeData : ScriptableObject
    {
        DialogueTree dialogueTree;
        public DialogueTree DialogueTree
        {
            get
            {
                if (dialogueTree == null)
                    throw new NullReferenceException();
                return dialogueTree;
            }
            private set
            {
                dialogueTree = value;
            }
        }

        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

        // Port
        [SerializeField] List<PortData> PortsData;
        public IEnumerable<string> InputPortGuids => PortsData.Where(p => p.Direction == Direction.Input).Select(p => p.PortGuid);
        public IEnumerable<string> OutputPortGuids => PortsData.Where(p => p.Direction == Direction.Output).Select(p => p.PortGuid);
        
        protected PortData InstantiatePortData(Direction direction)
        {
            PortData newPortData = new(DialogueTree, direction);
            PortsData.Add(newPortData);
            return newPortData;
        }

        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            DialogueTree = dialogueTree;
            name = GetType().Name;

            PortsData = new();

            OnInstantiatePortData();

            dialogueTree.Nodes.Add(this);
            AssetDatabase.AddObjectToAsset(this, dialogueTree);

            SaveChanges();
        }

        public abstract void OnInstantiatePortData();

        public virtual object ReadValueFromPort(string outputPortGuid) => null;

        public void SaveChanges()
        {
            // save node asset
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }

    public interface IExecutableNode
    {
        public void Start();
        public void Exit();
    }

    [Serializable]
    public record OutputValue
    {
        [field: SerializeField] public string Guid { get; set; }
        [field: SerializeField] public object Value { get; set; }
        [field: SerializeField] public string Text { get; set; }
    }
}