using UnityEngine;
using H8.FlowGraph;
using H8.FlowGraph.NodeTemplate;
using H8.FlowGraph.UiElements;

[CreateNodeMenu("Player/GainMoney")]
public class PlayerGainMoneyNode : BaseNode, IExecutableNode
{
    [Input]
    public ExecutionFlow Input { get; }

    [Output]
    public ExecutionFlow Output { get; }

    public int moneyToGain;
    [Input(nameof(moneyToGain))]
    public int MoneyToGain => GetInputValue(nameof(MoneyToGain), moneyToGain);

    public void Action(GraphTreeController controller, IBaseAction action) { }

    public void OnEnter(GraphTreeController controller)
    {
        DialogueDatabase.Instance.Player.GainMoney(MoneyToGain);
        controller.ToNextExecutableNode(GetPortData(nameof(Output)), GraphTree);
    }

    public void OnExit(GraphTreeController controller) { }
}

[CustomNodeView(typeof(PlayerGainMoneyNode))]
public class PlayerGainMoneyNodeView : NodeView
{
    public override Color? TitleBackgroundColor => new Color32(64, 128, 64, 255);
    public override Color? TitleTextColor => new Color(.8f,.8f,.8f);
}