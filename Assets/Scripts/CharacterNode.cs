using UnityEngine;
using BasDidon.Dialogue.VisualGraphView;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    [Selector]
    public Characters Character;

    [Port(PortDirection.Output, PortFieldStyle.Hide)]
    public string m_name;
    [Port(PortDirection.Output, PortFieldStyle.Hide)]
    public int money;

    public override object GetValue(string outputPortGuid)
    {
        Character character = DialogueDatabase.GetCharacter(Character);

        if(GetPortData("m_name").PortGuid == outputPortGuid)
        {
            return character.Name;
        }
        else if(GetPortData("money").PortGuid == outputPortGuid)
        {
            return character.Money;
        }
        else
        {
            throw new System.InvalidOperationException();
        }
    }
}