using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    public interface IExecutableNode
    {
        public void OnEnter();
        public void OnExit();

        public void Action(IBaseAction action);
    }

    public interface IBaseAction { }
}
