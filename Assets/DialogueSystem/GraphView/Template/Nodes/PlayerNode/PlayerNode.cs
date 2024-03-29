namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;

    [CreateNodeMenu(menuName = "Player/Player")]
    public class PlayerNode:BaseNode
    {
        Player Player => DialogueDatabase.Instance.Player;

        [Output]
        public string Name => Player.Name;

        [Output]
        public int Money => Player.Money;
    }
}
