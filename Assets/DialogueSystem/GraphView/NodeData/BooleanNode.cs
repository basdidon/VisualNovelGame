using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class BooleanNode : NodeData
    {
        [SerializeField] bool value;
        [field:SerializeField] public bool Value { get; set; }

        [field: SerializeField] public PortData OutputFlowPortData { get; private set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            SaveChanges();
        }

        public override void OnInstantiatePortData()
        {
            OutputFlowPortData = InstantiatePortData(Direction.Output);
        }
    }
}