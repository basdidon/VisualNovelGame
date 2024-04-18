namespace H8.FlowGraph
{
    public interface IExecutableNode
    {   
        public void OnEnter(GraphTreeController controller);
        public void OnExit(GraphTreeController controller);

        public void Action(GraphTreeController controller, IBaseAction action);
    }

    public interface IBaseAction { }
}
