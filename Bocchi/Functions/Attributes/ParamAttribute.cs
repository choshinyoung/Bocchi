namespace Bocchi.Functions.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ParamAttribute : Attribute
{
    public string? Description;
    public string? Type;

    public ParamAttribute(string? description = null, string? type = null)
    {
        Description = description;
        Type = type;
    }
}