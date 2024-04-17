using System;

namespace H8.GraphView.UiElements
{
    public class PortFactoryUtils
    {
        // find match Factory class for specific type
        public static IPortFactory GetPortFactory(Type type)
        {
            if (type == typeof(ExecutionFlow))
            {
                return new ExecutionFlowPortFactory();
            }
            else if (PrimativeTypePortFactory.TryCreateFactory(type, out PrimativeTypePortFactory factory))
            {
                return factory;
            }
            else if (type.IsDefined(typeof(CustomTypeAttribute), true))
            {
                CustomTypeAttribute customTypeAttr = Attribute.GetCustomAttribute(type, typeof(CustomTypeAttribute)) as CustomTypeAttribute;
                return Activator.CreateInstance(customTypeAttr.PortFactoryType.GetType()) as IPortFactory;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}