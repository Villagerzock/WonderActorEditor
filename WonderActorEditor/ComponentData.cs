namespace WonderActorEditor;
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ComponentData : Attribute
{
    public string Name;
    public string Description;
    public bool HasMultiple = false;
    
    public ComponentData(string name)
    {
        this.Name = name;
        this.Description = "";
        HasMultiple = false;
    }
    public ComponentData(string name, string description)
    {
        this.Name = name;
        this.Description = description;
        HasMultiple = false;
    }
    
    public ComponentData(string name, bool hasMultiple)
    {
        this.Name = name;
        this.Description = "";
        HasMultiple = hasMultiple;
    }
    public ComponentData(string name, string description, bool hasMultiple)
    {
        this.Name = name;
        this.Description = description;
        HasMultiple = hasMultiple;
    }
}