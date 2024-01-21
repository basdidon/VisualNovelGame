using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Character")]
public class CharacterData : ScriptableObject
{
    public string Name => name;
    [field: SerializeField] public Sprite Sprite { get; private set; }
}