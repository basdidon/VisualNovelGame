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

    public class GraphListView : BaseListView
    {
        protected override CollectionViewController CreateViewController()
        {
            return new GraphListViewController();
        }
    }

    public class GraphListViewController : BaseListViewController
    {
        protected override void BindItem(VisualElement element, int index)
        {
            throw new NotImplementedException();
        }

        protected override void DestroyItem(VisualElement element)
        {
            throw new NotImplementedException();
        }

        protected override VisualElement MakeItem()
        {
            throw new NotImplementedException();
        }

        protected override void UnbindItem(VisualElement element, int index)
        {
            throw new NotImplementedException();
        }
    }

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
            // list elements
            VisualElement listElementsContainer = new();
            Add(listElementsContainer);

            for(int i = 0; i < ItemsSource.Count; i++)
            {
                var itemView = MakeItem();
                BindItem?.Invoke(itemView, i);

                listElementsContainer.Add(itemView);
            }

            // Add Button
            Button addCondition = new() { text = "Add Choice" };
            Add(addCondition);
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
                List<Choice> choices = new(choicesNode.Choices);
                ListGraphView listView = new (choices,MakeItem,BindItem);
                extensionContainer.Add(listView);
                
                
                /*
                var serializedChoices = SerializedObject.FindProperty("choices");
                if (serializedChoices.isArray)
                {
                    
                    for (int i = 0; i < serializedChoices.arraySize; i++)
                    {
                        /*
                        extensionContainer.Add(ChoiceContainer.GetChoiceContainer(this, serializedChoices, i));
                    }

                }
                        */
                /*
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
                */
                extensionContainer.style.paddingTop = new StyleLength(4);
                extensionContainer.style.paddingBottom = new StyleLength(4);
                extensionContainer.style.paddingLeft = new StyleLength(4);
                extensionContainer.style.paddingRight = new StyleLength(4);

                // start with expanded state
                RefreshExpandedState();

                SerializedObject.ApplyModifiedProperties();
            }
        }

        public VisualElement MakeItem()
        {
            VisualElement listElement = new();
            listElement.style.flexDirection = FlexDirection.Column;

            VisualElement PortsContainer = new();
            listElement.Add(PortsContainer);

            // IsEnablePort
            var IsEnablePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            IsEnablePort.name = "is_enable_port";
            IsEnablePort.portName = string.Empty;
            PortsContainer.Add(IsEnablePort);

            // OutputFlowPort
            var outputFlowPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            outputFlowPort.name = "output_flow_port";
            outputFlowPort.portName = string.Empty;
            outputFlowPort.portColor = Color.yellow;
            PortsContainer.Add(outputFlowPort);

            // ChoiceText
            var choiceText = new TextField() {
                name = "choice_text"
            };
            listElement.Add(choiceText);

            // DeleteChoiceButton
            Button deleteChoiceBtn = new() { text = $"X" };
            listElement.Add(deleteChoiceBtn);

            deleteChoiceBtn.clicked += () =>
            {
                /*
                SerializedChoices.DeleteArrayElementAtIndex(ChoiceIndex);
                SerializedChoices.serializedObject.ApplyModifiedProperties();
                ChoicesNodeView.GraphView.RemoveElement(this);
                */
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
            style.borderTopLeftRadius = new(8);
            style.borderTopRightRadius = new(8);

            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            
            return listElement;
        }

        public void BindItem(VisualElement visualElement,int index)
        {
            var serializedChoices = SerializedObject.FindProperty("choices");
            var SerializedChoice = serializedChoices.GetArrayElementAtIndex(index);

            var isEnableInputPortSP = SerializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField");
            string isEnablePortGuid = isEnableInputPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
            var isEnablePort = visualElement.Q<Port>("is_enable_port");
            isEnablePort.viewDataKey = isEnablePortGuid;

            var outputFlowPortSP = SerializedChoice.FindPropertyRelative("<OutputFlowPortData>k__BackingField");
            string outputFlowPortGuid = outputFlowPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
            var outputFlowPort = visualElement.Q<Port>("output_flow_port");
            outputFlowPort.viewDataKey = outputFlowPortGuid;

            // ChoiceText
            var choiceTextSP = SerializedChoice.FindPropertyRelative("<Name>k__BackingField");
            var choiceText = visualElement.Q<TextField>("choice_text");
            choiceText.BindProperty(choiceTextSP);
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