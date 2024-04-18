using UnityEngine;

namespace H8.FlowGraph.NodeTemplate
{
    [CreateAssetMenu(menuName = "Dialogues/Character")]
    public class CharacterData : ScriptableObject
    {
        public string Name => name;
        [SerializeField] Sprite sprite;
        public Sprite Sprite => sprite;

        public int Hp => 10;
    }
}