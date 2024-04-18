using UnityEngine;

namespace H8.FlowGraph.NodeTemplate
{
    [CreateNodeMenu("Player/SpendMoney")]
    public class PlayerSpendMoneyNode : BaseNode, IExecutableNode
    {
        [Input]
        public ExecutionFlow Input { get; }
        [Output]
        public ExecutionFlow Output { get; }

        [Input(nameof(cost))]
        public int Cost => GetInputValue(nameof(Cost), cost);
        public int cost;

        public void OnEnter(GraphTreeController controller)
        {
            Debug.Log($"spend money : {Cost}");
            DialogueDatabase.Instance.Player.SpendMoney(Cost);

            controller.ToNextExecutableNode(GetPortData(nameof(Output)), GraphTree);
        }

        public void OnExit(GraphTreeController controller) { }

        public void Action(GraphTreeController controller, IBaseAction action) { }
    }

}

