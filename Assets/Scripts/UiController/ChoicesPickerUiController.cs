using UnityEngine.UIElements;
using H8.GraphView.NodeTemplate;
using H8.GraphView;

public interface IUiController
{
    public void Display();
    public void Hide();
    public bool IsDisplay { get; }
}

public class ChoicesPickerUiController : IUiController
{
    GraphTreeController GraphTreeController { get; }

    VisualElement Root { get; set; }

    public bool IsDisplay => Root.style.display == DisplayStyle.Flex;

    public ChoicesPickerUiController(GraphTreeController graphTreeController, VisualElement root)
    {
        GraphTreeController = graphTreeController;
        Root = root;
    }

    public void Clear() 
    {
        // clear old choices
        while (Root[0].childCount > 0)
        {
            Root[0].RemoveAt(0);
        }
    }

    public void SetChoices(ChoiceRecord[] choiceRecords)
    {
        Clear();

        for(int i = 0; i < choiceRecords.Length; i++)
        {
            Button btn = new();
            btn.AddToClassList("choice-btn");       // style
            Root[0].Add(btn);                       // add to root
            btn.text = choiceRecords[i].ChoiceText;
            btn.clicked += () => 
            {
                GraphTreeController.ExecuteAction(new SelectChoiceAction(btn.parent.IndexOf(btn)));
            };

            if (choiceRecords[i].IsEnable == false)
                btn.SetEnabled(false);
        }
    }

    public void Display()
    {
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
    }
}
