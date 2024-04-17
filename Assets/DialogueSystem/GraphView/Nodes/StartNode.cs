namespace H8.GraphView
{
    public class StartNode : BaseNode,IExecutableNode
    {
        [Output]
        public ExecutionFlow Output { get; }

        public void OnEnter(GraphTreeController controller)
        {
            controller.ToNextExecutableNode(GetPortData(nameof(Output)), GraphTree);
        }

        public void OnExit(GraphTreeController controller) { }

        public void Action(GraphTreeController controller, IBaseAction action) { }
    }
}