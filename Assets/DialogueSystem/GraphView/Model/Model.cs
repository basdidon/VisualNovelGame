using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Player")]
    public class Player:BaseNode
    {
        [Output]
        public string Name => "Bas";

        [Output]
        public int Money => 1000;
    }
}
