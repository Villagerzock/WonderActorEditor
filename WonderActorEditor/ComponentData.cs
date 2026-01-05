namespace WonderActorEditor;
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ComponentData : Attribute
{
    public string name;
    public string description;
    
    public ComponentData(string name)
    {
        this.name = name;
        this.description = "";
    }
    public ComponentData(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
}