namespace WonderActorEditor.actor;

public class Actor
{
    private string _name;

    public string Name
    {
        get => _name;
    }
    
    public List<IComponent> Components = new List<IComponent>();

    public Actor(string name)
    {
        this._name = name;
    }
}