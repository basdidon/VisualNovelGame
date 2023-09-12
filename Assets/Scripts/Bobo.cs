using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobo : Charecter
{
    DialoguesData HelloDialogue { get; set; }
    DialoguesData SpecialDialogue { get; set; }

    public int TalkCount { get; private set; }
    
    public Bobo()
    {
        HelloDialogue = Resources.Load<DialoguesData>("DialoguesDataSet/Hello");
        SpecialDialogue = Resources.Load<DialoguesData>("DialoguesDataSet/Special");
    }

    public override void Talk()
    {
        TalkCount++;
        DialogueUC.DialoguesData = HelloDialogue;
        DialogueUC.Display();
        DialogueUC.OnDisplayCompletedEvent += () =>
        {
            var choices = new List<Choice> {
                new Choice("กระโดด ตีลังกา 1 สเต็ป !!!", () => {
                    ChoicesPickerUC.Hide();
                    DialogueUC.Hide();
                }),
                new Choice("คุยอีกครั้ง", () => {
                    ChoicesPickerUC.Hide();
                    Talk();
                })
            };

            if (TalkCount > 1)
            {
                Debug.Log(TalkCount);
                choices.Add(new Choice("ตัวเลือกพิเศษ สำหรับคนพิเศษ", () =>
                {
                    ChoicesPickerUC.Hide();
                    DialogueUC.DialoguesData = SpecialDialogue;
                    DialogueUC.Display();
                    DialogueUC.OnDisplayCompletedEvent += () =>
                     {
                         DialogueUC.Hide();
                     };
                }));
            }

            ChoicesPickerUC.SetChoices(choices.ToArray());
            ChoicesPickerUC.Display();
        };
    }
}
