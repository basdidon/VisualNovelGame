using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.NodeTemplate
{
    using System.Collections;
    using System.Collections.Generic;
    using VisualGraphView;

    public class ListGraphView:VisualElement
    {
        public IList ItemsSource { get; set; }
        public Func<VisualElement> MakeItem { get; set; }
        public Action<VisualElement, int> BindItem { get; set; }

        public ListGraphView(IList itemsSource , Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
        {
            ItemsSource = itemsSource;
            MakeItem = makeItem;
            BindItem = bindItem;

            RefreshItems();
        }

        public void RefreshItems()
        {
            // Add Button
            Button addCondition = new() { text = "Add Choice" };
            Add(addCondition);

            // list elements
            VisualElement listElementsContainer = new();
            Add(listElementsContainer);
            foreach (var item in ItemsSource)
            {
                listElementsContainer.Add(MakeItem());
            }
        }
    }

    public class ListEntryGraphView : GraphElement
    {

    }

    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class ChoicesGraphViewNode : NodeView
    {
        readonly List<string> list = new() { "a", "b", "c", "d" };

        public override void OnDrawNodeView(BaseNode nodeData)
        {
            base.OnDrawNodeView(nodeData);
            if (nodeData is ChoicesNode choicesNode)
            {
                VisualElement makeItem() => new Label();
                void bindItem(VisualElement e, int i) => (e as Label).text = list[i];

                ListGraphView listView = new (list,makeItem,bindItem);
                extensionContainer.Add(listView);

                var nodeBorder = mainContainer.Q<VisualElement>("node-border");
                var selectionBorder = mainContainer.Q<VisualElement>("selection-border");

                //selectionBorder.style.height = nodeBorder.resolvedStyle.height;

                var serializedChoices = SerializedObject.FindProperty("choices");
                if (serializedChoices.isArray)
                {
                    for (int i = 0; i < serializedChoices.arraySize; i++)
                    {
                        extensionContainer.Add(ChoiceContainer.GetChoiceContainer(this, serializedChoices, i));
                    }

                }

                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () =>
                {
                    choicesNode.CreateChoice();
                    serializedChoices.serializedObject.Update();
                    //var choiceProperty = serializedChoices.GetArrayElementAtIndex(serializedChoices.arraySize - 1);

                    //DrawChoicePort(choiceProperty);
                    extensionContainer.Add(ChoiceContainer.GetChoiceContainer(this, serializedChoices, serializedChoices.arraySize - 1));
                    RefreshExpandedState();
                };
                mainContainer.Insert(2, addCondition);

                extensionContainer.style.paddingTop = new StyleLength(4);
                extensionContainer.style.paddingBottom = new StyleLength(4);
                extensionContainer.style.paddingLeft = new StyleLength(4);
                extensionContainer.style.paddingRight = new StyleLength(4);

                // start with expanded state
                RefreshExpandedState();

                SerializedObject.ApplyModifiedProperties();
            }
        }
    }

    public class ChoiceContainer : GraphElement
    {
        public ChoicesGraphViewNode ChoicesNodeView { get; }
        public SerializedProperty SerializedChoices { get; }
        public SerializedProperty SerializedChoice { get; }

        public int ChoiceIndex { get; }
     
        public ChoiceContainer(ChoicesGraphViewNode choicesNodeView,SerializedProperty serializedChoices,int choiceIndex)
        {
            ChoicesNodeView = choicesNodeView;
            SerializedChoices = serializedChoices;
            SerializedChoice = serializedChoices.GetArrayElementAtIndex(choiceIndex);
            ChoiceIndex = choiceIndex;
        }

        public static ChoiceContainer GetChoiceContainer(ChoicesGraphViewNode choicesNodeView,SerializedProperty serializedChoices, int choiceIndex)
        {
            var choiceContainer = new ChoiceContainer(choicesNodeView ,serializedChoices,choiceIndex);
            choiceContainer.Initialize();
            return choiceContainer;
        }

        public void Initialize()
        {
            VisualElement PortsContainer = new();
            Add(PortsContainer);

            SerializedChoice.isExpanded = true;

            var isEnableInputPortSP = SerializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField");
            string isEnablePortGuid = isEnableInputPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;

            var IsEnablePort = NodeElementFactory.CreatePortWithField(
                SerializedChoice.FindPropertyRelative("<IsEnable>k__BackingField"),
                typeof(bool),
                isEnablePortGuid,
                Direction.Input,
                ChoicesNodeView,
                ""
            );

            PortsContainer.Add(IsEnablePort);

            //
            var outputFlowPortSP = SerializedChoice.FindPropertyRelative("<OutputFlowPortData>k__BackingField");
            string outputFlowPortGuid = outputFlowPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;

            var outputFlowPort = NodeElementFactory.CreatePortWithField(
                SerializedChoice.FindPropertyRelative("<Output>k_BackingField"),
                typeof(ExecutionFlow),
                outputFlowPortGuid,
                Direction.Output,
                ChoicesNodeView,
                "Output"
            );

            PortsContainer.Add(outputFlowPort);

            var nameSP = SerializedChoice.FindPropertyRelative("<Name>k__BackingField");
            var choiceText = new PropertyField(nameSP, string.Empty);// TextField() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            choiceText.BindProperty(nameSP);

            Add(choiceText);

            Button deleteChoiceBtn = new() { text = $"X {ChoiceIndex}" };
            Add(deleteChoiceBtn);

            deleteChoiceBtn.clicked += () =>
            {
                SerializedChoices.DeleteArrayElementAtIndex(ChoiceIndex);
                SerializedChoices.serializedObject.ApplyModifiedProperties();
                ChoicesNodeView.GraphView.RemoveElement(this);
                //ChoicesNodeView.OnDrawNodeView();
                //SerializedChoices.serializedObject.Update();
            };

            // Style 
            style.marginTop = new(2f);
            style.marginBottom = new(0f);
            style.marginRight = new(0f);
            style.marginLeft = new(0f);
            style.backgroundColor = new Color(.08f, .08f, .08f, .5f);
            style.borderBottomLeftRadius = new(8);
            style.borderBottomRightRadius = new(8);
            style.borderTopLeftRadius =new(8);
            style.borderTopRightRadius = new(8);

            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
        }
    }
}