namespace Bocchi.Functions.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ParamAttribute : Attribute
{
    public string? Description;

    public ParamAttribute(string? description = null)
    {
        Description = description;
    }
}