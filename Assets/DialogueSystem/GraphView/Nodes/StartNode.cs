namespace H8.GraphView
{
    public class StartNode : BaseNode,IExecutableNode
    {
        [Output]
        public ExecutionFlow Output { get; }

        public void OnEnter()
        {
            GraphTreeContorller.Instance.ToNextExecutableNode(GetPortData(nameof(Output)), GraphTree);
        }

        public void OnExit(){}

        public void Action(IBaseAction action){}
    }
}