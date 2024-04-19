using UnityEngine;

namespace H8.FlowGraph.NodeTemplate
{
    public class NextDialogueAction : IBaseAction { }
    public record DialogueEvent : ICustomEvent
    {
        public string SpeakerName { get; }
        public string DialogueText { get; }

        public DialogueEvent(string speakerName,string dialogueText)
        {
            SpeakerName = speakerName;
            DialogueText = dialogueText ?? string.Empty;
        }
    }

    [CreateNodeMenu(nameof(DialogueNode))]
    public class DialogueNode : BaseNode, IExecutableNode
    {
        [Input]
        public ExecutionFlow Input { get; }
        [Output]
        public ExecutionFlow Output { get; }

        [NodeField]
        public CharacterData speaker;
        
        [TextArea, NodeField]
        public string DialogueText  = string.Empty;

        public void OnEnter(GraphTreeController controller)
        {
            Debug.Log("dialogue node executing");
            controller.FireEvent(new DialogueEvent(speaker?.Name, DialogueText));
        }

        public void OnExit(GraphTreeController controller) { }

        public void Action(GraphTreeController controller, IBaseAction action)
        {
            if(action is NextDialogueAction)
            {
                controller.ToNextExecutableNode(GetPortData(nameof(Output)), GraphTree);
            }
        }
    }

    

}