using UnityEngine;
using H8.FlowGraph;
using H8.FlowGraph.NodeTemplate;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] GraphTree graphTree;
    GraphTreeController GraphTreeController { get; set; }

    private void Awake()
    {
        GraphTreeController = new();
    }

    public void OnEnable()
    {
        GraphTreeController.OnCustomEvent += OnCustomEventHandler;
    }

    public void OnDisable()
    {
        GraphTreeController.OnCustomEvent -= OnCustomEventHandler;
    }

    void OnCustomEventHandler(ICustomEvent @event)
    {
        if(@event is DialogueEvent dialogue)
        {
            Debug.Log($"{dialogue.SpeakerName ?? "Unknow"} : {dialogue.DialogueText}");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!GraphTreeController.IsPlaying)
            {
                GraphTreeController.StartGraphTree(graphTree);
            }
            else
            {
                GraphTreeController.ExecuteAction(new NextDialogueAction());
            }
        }
    }
}
