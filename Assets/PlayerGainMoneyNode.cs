using UnityEngine;

namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;
    
    [CreateNodeMenu(menuName = "Player/GainMoney")]
    public class PlayerGainMoneyNode : BaseNode, IExecutableNode
    {
        [Input]
        public ExecutionFlow Input { get; }

        [Output]
        public ExecutionFlow Output { get; }

        public int moneyToGain;
        [Input(nameof(moneyToGain))]
        public int MoneyToGain => GetInputValue(nameof(MoneyToGain), moneyToGain);

        public void Action(IBaseAction action) { }

        public void OnEnter()
        {
            DialogueDatabase.Instance.Player.GainMoney(MoneyToGain);
            GraphTreeContorller.Instance.ToNextExecutableNode(GetPortData(nameof(Output)), DialogueTree);
        }

        public void OnExit() { }
    }
}