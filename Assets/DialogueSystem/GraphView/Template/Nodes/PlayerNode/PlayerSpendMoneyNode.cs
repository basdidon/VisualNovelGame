using UnityEngine;

namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;

    [CreateNodeMenu(menuName = "Player/SpendMoney")]
    public class PlayerSpendMoneyNode : BaseNode, IExecutableNode
    {
        [Input]
        public ExecutionFlow Input { get; }
        [Output]
        public ExecutionFlow Output { get; }

        [Input(nameof(cost))]
        public int Cost => GetInputValue(nameof(Cost), cost);
        public int cost;

        public void OnEnter()
        {
            Debug.Log($"spend money : {Cost}");
            DialogueDatabase.Instance.Player.SpendMoney(Cost);

            GraphTreeContorller.Instance.ToNextExecutableNode(GetPortData(nameof(Output)), DialogueTree);
        }

        public void OnExit() { }

        public void Action(IBaseAction action){}
    }

}

