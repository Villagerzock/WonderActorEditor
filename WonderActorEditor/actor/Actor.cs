using Vulkan;

namespace WonderActorEditor.actor;

public class Actor
{
    private string _name;

    public string Name
    {
        get => _name;
    }
    
    public bool HasUnsavedChanges = false;

    public void MarkUnsavedChanges()
    {
        HasUnsavedChanges = true;
    }

    public void Save()
    {
        HasUnsavedChanges = false;
    }

    public List<IComponent> Components = new List<IComponent>();

    public Actor(string name)
    {
        this._name = name;
    }
}