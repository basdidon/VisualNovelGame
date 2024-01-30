using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Graphview.NodeView
{
    using NodeData;

    public class DialogueGraphView : GraphView
    {
        public DialogueTree Tree { get; private set; }
        public GraphViewNode StartNode { get; private set; }

        public DialogueGraphView(string assetPath)
        {
            AddManipulator();
            AddBackground();
            AddStyle();

            LoadAsset(assetPath);

            graphViewChanged += OnGraphViewChange;
        }

        void LoadAsset(string assetPath)
        {
            Tree = AssetDatabase.LoadAssetAtPath<DialogueTree>(assetPath);

            Debug.Log($"Load {assetPath}");
            foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)) // load all sub assets
            {
                if(obj == null)
                    continue;
                
                if (obj is GVNodeData nodeData)
                {
                    var node = NodeFactory.GetNodeView(nodeData, this);
                    AddElement(node);
                    //Debug.Log($"-> {}");
                    continue;
                }

                throw new Exception($"Unexpected asset type. {obj.GetType()}");
            }

            //Debug.Log($"-> {GetPortByGuid("aad956ad-4dad-46ec-9069-b5226819ff81").direction}");
            // create edges

            foreach (var edgeData in Tree.Edges)
            {
                Port outputPort = GetPortByGuid(edgeData.OutputPortGuid);
                Port inputPort = GetPortByGuid(edgeData.InputPortGuid);

                if (outputPort == null || inputPort == null)
                    continue;

                Debug.Log($"{outputPort.viewDataKey} {outputPort.direction} -> {edgeData.InputPortGuid} {inputPort.direction}");
                /*
                if (outputPort.direction == inputPort.direction)
                    continue;
                               Edge edge = outputPort.ConnectTo(inputPort);
                               edge.viewDataKey = edgeData.EdgeGuid;
                */
                Edge edge = new();
                edge.viewDataKey = edgeData.EdgeGuid;
                edge.input = inputPort;
                edge.output = outputPort;
                edge.UpdateEdgeControl();
                AddElement(edge);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compartiblePorts = new();

            ports.ForEach(port =>
            {
                if (startPort == port)                              // can't connect to self
                    return;
                if (startPort.node == port.node)                    // can't connect to self node
                    return;
                if (startPort.direction == port.direction)          // can't connect to same dir
                    return;
                if (startPort.portType != port.portType)
                    return;

                compartiblePorts.Add(port);
            });

            return compartiblePorts;
        }

        void AddManipulator()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());        // need to add before RectangleSelector()
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(SaveContextualMenu());
        }

        void CreateActionEvent<T>(DropdownMenuAction actionEvent) where T:GVNodeData
            =>  AddElement(NodeFactory.GetNodeView(NodeFactory.CreateNode<T>(actionEvent.eventInfo.localMousePosition, Tree),this));
        

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction($"Create DialogueNode", CreateActionEvent<DialogueNode>);
            evt.menu.AppendAction($"Create ConditionNode", CreateActionEvent<ChoicesNode>);
            evt.menu.AppendAction($"Create QuestionNode", CreateActionEvent<QuestionNode>);
            evt.menu.AppendAction("Create LogicNode/Boolean", CreateActionEvent<BooleanNode>);
        }

        IManipulator SaveContextualMenu()
        {
            return new ContextualMenuManipulator(ev =>
            {
                ev.menu.AppendAction("Save Graph", actionEvent => SaveGraph());
            });
        }

        GraphViewChange OnGraphViewChange(GraphViewChange changes)
        {
            Debug.Log("OnGraphViewChange");
            if (changes.edgesToCreate != null)
            {
                foreach (var edge in changes.edgesToCreate)
                {
                    Node InputNode = edge.input.node;
                    Node OutputNode = edge.output.node;

                    Debug.Log($"EdgesToCreate {OutputNode.title} -> {InputNode.title}");

                    if (OutputNode.userData is not GVNodeData outputNode)
                        throw new Exception("OutputNode.userData is not GVNodeData outputNode");

                    if (InputNode.userData is not GVNodeData inputNode)
                        throw new Exception("InputNode.userData is not GVNodeData inputNode");

                    if (inputNode == null)
                        throw new Exception("inputNode == null");

                    EdgeData edgeData = new(Tree, edge.output.viewDataKey, edge.input.viewDataKey);
                    edge.viewDataKey = edgeData.EdgeGuid;
                    Tree.AddEdge(edgeData);
                }
            }

            if (changes.elementsToRemove != null)
            {
                foreach (var element in changes.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        EdgeData toRemoveEdgeData = Tree.Edges.FirstOrDefault(_edge => _edge.EdgeGuid == edge.viewDataKey);
                        if (toRemoveEdgeData != null)
                        {
                            Tree.RemoveEdge(toRemoveEdgeData);
                            Debug.Log("Edge was removed.");
                        }
                        else
                        {
                            Debug.Log($"not found Edge {edge.viewDataKey}");
                        }

                    }
                    else if (element is Node node)
                    {
                        Debug.Log("Node was removed.");
                        GVNodeData toRemoveNode = Tree.Nodes.FirstOrDefault(_node => _node.Id == node.viewDataKey);
                        Tree.Nodes.Remove(toRemoveNode);
                        AssetDatabase.RemoveObjectFromAsset(toRemoveNode);
                        //AssetDatabase.SaveAssets();
                    }
                }
            }

            if (changes.movedElements != null)
            {
                foreach (var element in changes.movedElements)
                {
                    if (element.userData is GVNodeData nodeData)
                    {
                        nodeData.GraphPosition = element.GetPosition().position;
                    }
                }
            }

            return changes;
        }

        void SaveGraph()
        {
            Debug.Log("Save");

            EditorUtility.SetDirty(Tree);
            AssetDatabase.SaveAssets();
        }

        void AddBackground()
        {
            GridBackground gridBackground = new();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        void AddStyle()
        {
            StyleSheet GrahpviewStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueGVStyle.uss");
            StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("NodeStyle.uss");
            styleSheets.Add(GrahpviewStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }
    }
}