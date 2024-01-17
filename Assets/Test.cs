//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false)]
public class MyAttribute:Attribute
{

}

public class Test : MonoBehaviour
{
    [My]
    public int a;
    [My]
    public int B { get; set; }
    public UnityEvent UnityEvent;
    //public Func<bool> func;
    public UnityAction UnityAction;


    private void Start()
    {
        Debug.Log("start");
        // Get all types in the assembly
        var assembly = Assembly.GetExecutingAssembly();
        var allTypes = assembly.GetTypes();

        // Filter types that have methods with the MyAttribute
        var typesWithAttribute = allTypes.Where(type =>
            type.GetProperties().Any(method =>
                method.CanRead && Attribute.IsDefined(method, typeof(MyAttribute))));

        //var propertyInfos = allTypes.SelectMany(type => type.GetProperties().Any(prop => prop.CanRead || Attribute.IsDefined(prop, typeof(MyAttribute))));

        Debug.Log("Classes with methods using MyAttribute:");

        foreach (var type in typesWithAttribute)
        {
            Debug.Log($"   {type.Name} ");
        }
    }
}