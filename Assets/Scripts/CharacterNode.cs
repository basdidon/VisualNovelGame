using BasDidon.Dialogue.VisualGraphView;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    [Selector]
    public Characters Character;

    Character CharacterData => DialogueDatabase.Instance.GetCharacter(Character);

    [Output]
    public string Name => CharacterData.Name;

    [Output]
    public int Money => CharacterData.Money;
}