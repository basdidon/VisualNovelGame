using System;

public interface ITextNodeData
{
    public static int StringPerLine => 25;
    public static int MaxLine => 3;

    public Charecters Speaker { get; set; }
    public string[] TextLine { get; set; }

    void OnValidate()
    {
        for (int i = 0; i < TextLine.Length; i++)
        {
            if (TextLine[i].Length > StringPerLine)
            {
                TextLine[i] = TextLine[i].Substring(0, StringPerLine);
            }
        }

        string[] _textLine = TextLine;              // can't pass property into ref parmeter, so i changed it to field
        if (TextLine.Length != MaxLine)
        {
            Array.Resize(ref _textLine, MaxLine);
            return;
        }
    }
}
