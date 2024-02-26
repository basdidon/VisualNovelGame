using UnityEngine;
using BasDidon.Dialogue.VisualGraphView;
using BasDidon.Dialogue;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    [Selector]
    public Characters Character;

    [OldPort(PortDirection.Output, PortFieldStyle.Hide)]
    public string m_name;
    [OldPort(PortDirection.Output, PortFieldStyle.Hide)]
    public int money;

    public override object GetValue(string outputPortGuid)
    {
        Character character = DialogueManager.Instance.DialogueDatabase.GetCharacter(Character);

        if(GetPortData(nameof(m_name)).PortGuid == outputPortGuid)
        {
            return character.Name;
        }
        else if(GetPortData(nameof(money)).PortGuid == outputPortGuid)
        {
            return character.Money;
        }
        else
        {
            throw new System.InvalidOperationException();
        }
    }
}