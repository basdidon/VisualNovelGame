using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.Dialogue.VisualGraphView;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    [Output] public string m_name = string.Empty;

    [field: SerializeField] public Character Character { get; set; }
}
