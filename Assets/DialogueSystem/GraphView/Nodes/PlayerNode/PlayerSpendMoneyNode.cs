using System.Linq;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
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
            DialogueManager.Instance.DialogueDatabase.Player.SpendMoney(Cost);

            DialogueManager.Instance.SetNextNode(GetPortData(nameof(Output)), DialogueTree);
        }

        public void OnExit() { }
    }

}

