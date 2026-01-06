using WonderActorEditor.actor;
using YamlDotNet.RepresentationModel;

namespace WonderActorEditor.components;

[ComponentData("Model Bind", "Specifies if the Actor can be Bound to any Actors")]
public class ModelBindParamComponent : IComponent
{
    public void Render(int id, Actor parent)
    {
        
    }

    public string GetActorParamId()
    {
        return "ModelBindParam";
    }

    public YamlNode GetYAML()
    {
        return new YamlMappingNode() { };
    }
}