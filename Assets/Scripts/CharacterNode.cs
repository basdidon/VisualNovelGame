using UnityEngine;
using BasDidon.Dialogue.VisualGraphView;
using BasDidon.Dialogue;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    [Selector]
    public Characters Character;

    Character CharacterData => DialogueManager.Instance.DialogueDatabase.GetCharacter(Character);

    [Output]
    public string Name => CharacterData.Name;

    [Output]
    public int Money => CharacterData.Money;
}