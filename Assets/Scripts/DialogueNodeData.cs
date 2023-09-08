using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNodeData : GVNodeData
{
    public static int StringPerLine => 25;
    public static int MaxLine => 3;

    [field: SerializeField] public Charecters Speaker { get; set; }
    [SerializeField] string[] textLine = new string[MaxLine];
    public string[] TextLine => textLine;
    [field: SerializeField] public Sprite Background { get; set; }

    void OnValidate()
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
