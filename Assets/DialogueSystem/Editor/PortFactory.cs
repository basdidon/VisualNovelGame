using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace H8.GraphView.UiElements
{
    public static class PortFactory
    {
        // View
        public static Port CreateUnbindPort(PortAttribute portAttr, PropertyInfo propertyInfo, NodeView nodeView)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException();

            Type type = propertyInfo.PropertyType;
            IPortFactory portFactory = PortFactoryUtils.GetPortFactory(type);

            return portFactory.CreateUnbindPort(portAttr.Direction, nodeView, propertyInfo.Name, portAttr.HasBackingFieldName);
        }

        public static Port CreatePort(PortAttribute portAttr, PropertyInfo propertyInfo, NodeView nodeView, PortData portData, SerializedObject serializedObject = null)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException();

            Type type = propertyInfo.PropertyType;
            IPortFactory portFactory = PortFactoryUtils.GetPortFactory(type);

            if (serializedObject != null && portAttr.HasBackingFieldName)
            {
                var serializeProperty = serializedObject.FindProperty(portAttr.BackingFieldName);

                if (serializeProperty == null)
                    throw new NullReferenceException();
                else
                    return portFactory.CreatePortWithField(serializeProperty, portData.PortGuid, portData.Direction, nodeView, propertyInfo.Name);
            }
            else
            {
                return portFactory.CreatePort(portData.PortGuid, portData.Direction, nodeView, propertyInfo.Name);
            }
        }
    }
}