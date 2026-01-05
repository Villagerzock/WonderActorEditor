using System.Reflection;

namespace WonderActorEditor;

public interface IComponent
{
    void Render(int id);
    string GetName()
    {
        ComponentData? data = this.GetType().GetCustomAttribute<ComponentData>();
        if (data != null)
        {
            return data.name;
        }
        return this.GetType().Name;
    }

    string GetActorParamID();

    // Here Come Some More methods for Serialization Later
}