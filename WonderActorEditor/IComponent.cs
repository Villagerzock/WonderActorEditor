using System.Reflection;
using WonderActorEditor.actor;
using WonderActorEditor.components;
using YamlDotNet.RepresentationModel;

namespace WonderActorEditor;

public interface IComponent
{
    
    public static Type[] allComponentTypes =
    {
        typeof(YAMLComponent),
        typeof(ModelBindParamComponent),
        typeof(DamageReactionComponent),
        typeof(ReceiveNumToDieComponent)
    };
    void Render(int id, Actor parent);
    string GetName()
    {
        ComponentData? data = this.GetType().GetCustomAttribute<ComponentData>();
        if (data != null)
        {
            return data.Name;
        }
        return this.GetType().Name;
    }

    string GetActorParamId();
    YamlNode GetYAML();

    // Here Come Some More methods for Serialization Later
}