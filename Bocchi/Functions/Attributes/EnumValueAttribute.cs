namespace Bocchi.Functions.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class EnumValueAttribute : Attribute
{
    public List<string>? Enums;

    public EnumValueAttribute(params string[] enums)
    {
        Enums = enums.ToList();
    }
}