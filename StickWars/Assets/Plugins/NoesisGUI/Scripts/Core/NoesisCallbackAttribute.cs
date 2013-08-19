using System;

////////////////////////////////////////////////////////////////////////////////////////////////
[System.AttributeUsage(System.AttributeTargets.Method)]
public class MonoPInvokeCallbackAttribute : System.Attribute
{
    public MonoPInvokeCallbackAttribute(Type t)
    {
        type = t;
    }

    public Type Type
    {
        get
        {
            return type;
        }
    }

    private Type type;
}
