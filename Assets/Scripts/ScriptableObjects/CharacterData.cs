using UnityEngine;
using Basdidon.Dialogue;

[CreateAssetMenu(menuName = "Dialogues/Character")]
public class CharacterData : DialogueCharacterBase
{
    [SerializeField] Sprite sprite;
    public override Sprite Sprite => sprite;

    private void OnDestroy()
    {
        Debug.Log("boom");
    }
}
