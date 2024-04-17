using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace H8.GraphView.UiElements
{
    public class DialogueGraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        
        public GraphTree Tree { get; private set; }

        public readonly string[] nodesPath = { "Assets/" };

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
            Tree = AssetDatabase.LoadAssetAtPath<GraphTree>(assetPath);

            Debug.Log($"Load {assetPath}");
            foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)) // load all sub assets
            {
                if (obj == null)
                    continue;

                if (obj is BaseNode nodeData)
                {
                    var node = NodeViewFactory.GetNodeView(nodeData, this);
                    AddElement(node);
                    continue;
                }

                throw new Exception($"Unexpected asset type. {obj.GetType()}");
            }

            Debug.Log($"StartDrawingEdge ({Tree.Edges.Count})");
            foreach (var edgeData in Tree.Edges)
            {
                Port outputPort = GetPortByGuid(edgeData.OutputPortGuid);
                Port inputPort = GetPortByGuid(edgeData.InputPortGuid);

                if (outputPort == null || inputPort == null)
                    continue;

                if (outputPort.direction == inputPort.direction)
                    continue;

                Edge edge = outputPort.ConnectTo(inputPort);
                edge.viewDataKey = edgeData.EdgeGuid;

                AddElement(edge);

                OnPortConnect?.Invoke(inputPort);
                OnPortConnect?.Invoke(outputPort);
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

           PopulateCreateContextualMenu(this);
        }

        public static void PopulateCreateContextualMenu(DialogueGraphView graphView)
        {
            var baseNodesType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseNode)) && type.IsDefined(typeof(CreateNodeMenuAttribute)));

            foreach (var type in baseNodesType)
            {
                var attr = type.GetCustomAttribute<CreateNodeMenuAttribute>();

                graphView.AddManipulator(new ContextualMenuManipulator(ev =>
                {
                    ev.menu.AppendAction(attr.MenuName, actionEvent =>
                    {
                        VisualElement contentViewContainer = graphView.ElementAt(1);
                        Vector3 screenMousePosition = actionEvent.eventInfo.localMousePosition;
                        Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
                        worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
                        // create baseNode
                        BaseNode baseNode = NodeFactory.CreateNode(type, worldMousePosition, graphView.Tree);
                        // get nodeView by baseNode
                        NodeView nodeView = NodeViewFactory.GetNodeView(baseNode, graphView);

                        // add nodeView to graph
                        graphView.AddElement(nodeView);
                    });
                }));

            }
        }

        void CreateActionEvent<T>(DropdownMenuAction actionEvent) where T : BaseNode
        {
            // create baseNode
            BaseNode baseNode = NodeFactory.CreateNode<T>(actionEvent.eventInfo.localMousePosition, Tree);
            // get nodeView by baseNode
            NodeView nodeView = NodeViewFactory.GetNodeView(baseNode, this);
            // add nodeView to graph
            AddElement(nodeView);
        }
        

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            //evt.menu.AppendAction($"Create DialogueNode", CreateActionEvent<DialogueNode>);
            //evt.menu.AppendAction($"Create ConditionNode", CreateActionEvent<ChoicesNode>);
        }

        IManipulator SaveContextualMenu()
        {
            return new ContextualMenuManipulator(ev =>
            {
                ev.menu.AppendAction("Save Graph", actionEvent => SaveGraph());
            });
        }

        public event Action<Port> OnPortConnect;
        public event Action<Port> OnPortDisconnect;

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

                    if (OutputNode.userData is not BaseNode outputNode)
                        throw new Exception("OutputNode.userData is not GVNodeData outputNode");

                    if (InputNode.userData is not BaseNode inputNode)
                        throw new Exception("InputNode.userData is not GVNodeData inputNode");

                    if (inputNode == null)
                        throw new Exception("inputNode == null");

                    EdgeData edgeData = new(edge.output.viewDataKey, edge.input.viewDataKey);
                    edge.viewDataKey = edgeData.EdgeGuid;
                    Tree.AddEdge(edgeData);
                    
                    OnPortConnect?.Invoke(edge.input);
                    OnPortConnect?.Invoke(edge.output);;
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

                            OnPortDisconnect?.Invoke(edge.input);
                            OnPortDisconnect?.Invoke(edge.output);
                        }
                        else
                        {
                            Debug.Log($"not found Edge {edge.viewDataKey}");
                        }

                    }
                    else if (element is Node node)
                    {
                        Debug.Log("Node was removed.");
                        BaseNode toRemoveNode = Tree.Nodes.FirstOrDefault(_node => _node.Id == node.viewDataKey);
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
                    if (element.userData is BaseNode nodeData)
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
            var nodeStylesPath = AssetDatabase.FindAssets(
                "t:StyleSheet", 
                new[] { 
                    "Assets/DialogueSystem/GraphView/Styles",
                });

            foreach(var stylesPath in nodeStylesPath)
            {
                var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(stylesPath));
                styleSheets.Add(sheet);
            }
        }
    }
}