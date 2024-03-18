using UnityEngine;

namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;

    public record DialogueRecord : ICustomEvent
    {
        public string SpeakerName { get; }
        public string DialogueText { get; }

        public DialogueRecord(string speakerName,string dialogueText)
        {
            SpeakerName = speakerName;
            DialogueText = dialogueText ?? string.Empty;
        }
    }

    [CreateNodeMenu(menuName = nameof(DialogueNode))]
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

        public void OnEnter()
        {
            Debug.Log("dialogue node executing");
            GraphTreeContorller.Instance.FireEvent(new DialogueRecord(speaker.Name, StringHelper.GetValueFromSyntax(DialogueText)));
        }

        public void OnExit(){}

        public void Action(IBaseAction action)
        {
            if(action is NextDialogueAction)
            {
                GraphTreeContorller.Instance.ToNextExecutableNode(GetPortData(nameof(Output)), GraphTree);
            }
        }
    }


    public class NextDialogueAction : IBaseAction{}

}