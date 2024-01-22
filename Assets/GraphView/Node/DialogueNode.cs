using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.NodeData
{
    public class DialogueNode : GVNodeData
    {
        public int MaxLine => 3;
        public int MaxTextLength => 25;

        [SerializeField] string outputPortGuid;
        public override string[] OutputPortGuids => new string[] { outputPortGuid };

        public Characters Speaker { get; set; }

        [field: SerializeField] public CharacterData CharacterData { get; set; }
        [field: SerializeField] public string[] TextLine { get; set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            TextLine = new string[MaxLine];
            for (int i = 0; i < MaxLine; i++)
            {
                TextLine[i] = string.Empty;
            }

            outputPortGuid = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void ValidateTextLine(InputEvent ev, int lineIdx)
        {
            TextField txtField = ev.currentTarget as TextField;
            if (ev.newData.Length > MaxTextLength)
                txtField.value = ev.newData.Substring(0, MaxTextLength);
            TextLine[lineIdx] = txtField.value;
        }

        [field: SerializeField] GVNodeData Child { get; set; }

        public override void Draw(Node node)
        {
            DrawInputPort(node);

            // output port
            Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.viewDataKey = outputPortGuid;
            outputPort.portName = "Output";
            node.outputContainer.Add(outputPort);

            // Custom extension
            VisualElement customVisualElement = new();

            // CharacterData ObjectField
            ObjectField characterDataObjectField = new() { 
                objectType = typeof(CharacterData), 
                value = CharacterData 
            };
            characterDataObjectField.RegisterValueChangedCallback(e =>CharacterData =(CharacterData) e.newValue);
            node.mainContainer.Insert(1,characterDataObjectField);

            // Foldout Textline
            Foldout txtFoldout = new() { text = "text" };

            foreach (var lineIdx in Enumerable.Range(0, MaxLine))
            {
                VisualElement container = new();
                TextField lineTxt = new() { 
                    label = $"[{lineIdx + 1}]",
                    value = TextLine.ElementAtOrDefault(lineIdx) ?? string.Empty 
                };
                lineTxt.RegisterCallback<InputEvent>((ev) => ValidateTextLine(ev, lineIdx));
                Label lineLabel = lineTxt.labelElement;
                lineLabel.style.color = new StyleColor(Color.black);
                lineLabel.style.minWidth = new StyleLength(StyleKeyword.Auto);

                container.Add(lineTxt);
                txtFoldout.Add(container);
            }

            customVisualElement.Add(txtFoldout);
            node.extensionContainer.Add(customVisualElement);

            // start with expanded state
            node.RefreshExpandedState();
        }

        public override void AddChild(GVNodeData child)
        {
            Child = child;
        }

        public override void RemoveChild(GVNodeData child)
        {
            if (Child == child)
                Child = null;
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return new GVNodeData[] { Child };
        }

        void OnValidate()
        {
            for (int i = 0; i < TextLine.Length; i++)
            {
                if (TextLine[i].Length > MaxTextLength)
                {
                    TextLine[i] = TextLine[i].Substring(0, MaxTextLength);
                }
            }
        }
    }
}