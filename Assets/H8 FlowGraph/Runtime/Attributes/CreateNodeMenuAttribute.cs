using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace H8.FlowGraph
{
    using UiElements;

    [AttributeUsage(AttributeTargets.Class)]
    public class CreateNodeMenuAttribute : Attribute
    {
        public string MenuName { get; }

        public CreateNodeMenuAttribute(string menuName)
        {
            MenuName = menuName;
        }
    }
}
