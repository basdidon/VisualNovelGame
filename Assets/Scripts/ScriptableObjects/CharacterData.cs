using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Character")]
public class CharacterData:ScriptableObject
{
    public string Name => name;
    [SerializeField] Sprite sprite;
    public Sprite Sprite => sprite;
}
