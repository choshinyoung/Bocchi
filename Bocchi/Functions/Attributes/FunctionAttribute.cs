namespace Bocchi.Functions.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionAttribute : Attribute
{
    public string Description;
    public string Name;

    public FunctionAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}