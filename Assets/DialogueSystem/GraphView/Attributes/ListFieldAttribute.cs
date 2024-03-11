using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Dialogue
{
    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class ListFieldAttribute : Attribute
    {
        public ListFieldAttribute(Type type)
        {

        }
    }
}
