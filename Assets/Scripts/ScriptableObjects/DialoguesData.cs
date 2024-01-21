using System.Collections.Generic;
using UnityEngine;
using System;
 
public enum Characters
{
    Sabastian,
    Emma,
    Johnny,
}

//[CreateAssetMenu(menuName = "Dialogue")]
public class DialoguesData : ScriptableObject
{
    public Vector2 GraphPosition { get; set; }             // position on graphview
    public List<DialoguesData> NextDialogue { get; set; }

    // old props
    [field: SerializeField] public List<Sentence> Dialogues { get; set; }

    void OnValidate()
    {
        foreach(var sentance in Dialogues)
        {
            sentance.OnValidate();
        }
    }
}

[Serializable]
public class Sentence
{
    public static int StringPerLine => 25;
    public static int MaxLine => 3;

    [field: SerializeField] public Characters Speaker { get; set; }
    [SerializeField] string[] textLine = new string[MaxLine];
    public string[] TextLine => textLine;
    [field: SerializeField] public Sprite Background { get; set; }

    public void OnValidate()
    {
        for (int i = 0; i < TextLine.Length; i++)
        {
            if (TextLine[i].Length > StringPerLine)
            {
                TextLine[i] = TextLine[i].Substring(0, StringPerLine);
            }
        }

        if (textLine.Length != MaxLine)
        {
            Array.Resize(ref textLine, MaxLine);
            return;
        }
    }
}
