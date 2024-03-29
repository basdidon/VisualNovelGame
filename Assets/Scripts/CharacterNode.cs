using BasDidon.Dialogue.NodeTemplate;
using BasDidon.Dialogue.VisualGraphView;
using UnityEditor;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    /*
    [Selector]
    public Characters Character;
    */
    [NodeField]
    public CharacterData characterData;
    public CharacterData CharacterData
    {
        get
        {
            if (characterData == null)
                throw new System.NullReferenceException();
            return characterData;
        }
    }

    Character GetCharacter()
    {
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier<CharacterData>(CharacterData, out string guid, out _))
        {
            return DialogueDatabase.Instance.GetCharacter(guid);
        }
        else
        {
            return default;
        }
    }

    [Output]
    public string Name => GetCharacter()?.Name;

    [Output]
    public int Money => GetCharacter()?.Money ?? 0;
}