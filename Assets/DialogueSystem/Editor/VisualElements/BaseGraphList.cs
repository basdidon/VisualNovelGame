using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace H8.GraphView.UiElements
{
    public class BaseGraphList : VisualElement
    {
        public VisualElement PortsContainer { get; }
        public VisualElement InputContainer { get; }
        public VisualElement OutputContainer { get; }
        public VisualElement ContentContainer { get; }

        public BaseGraphList()
        {
            PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            Add(PortsContainer);

            InputContainer = new();
            InputContainer.style.flexGrow = 1;
            PortsContainer.Add(InputContainer);

            OutputContainer = new();
            OutputContainer.style.flexGrow = 1;
            PortsContainer.Add(OutputContainer);

            ContentContainer = new();
            Add(ContentContainer);  
        }

        public void AddPort(Port portToAdd)
        {
            if(portToAdd.direction == Direction.Input)
            {
                InputContainer.Add(portToAdd);
            }
            else
            {
                OutputContainer.Add(portToAdd);
            }
        }
    }
}