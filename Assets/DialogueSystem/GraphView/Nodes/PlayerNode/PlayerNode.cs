namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Player/")]
    public class PlayerNode:BaseNode
    {
        Player Player => DialogueManager.Instance.DialogueDatabase.Player;

        [Output]
        public string Name => Player.Name;

        [Output]
        public int Money => Player.Money;
    }
}
