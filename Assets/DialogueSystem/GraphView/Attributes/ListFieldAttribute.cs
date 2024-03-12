using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Dialogue
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ListFieldAttribute : Attribute
    {
        public Type CreatorType { get; }
        public ListFieldAttribute(Type type)
        {
            CreatorType = type;
        }
    }
}
