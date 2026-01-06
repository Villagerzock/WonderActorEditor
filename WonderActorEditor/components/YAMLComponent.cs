using System.Numerics;
using ImGuiNET;
using WonderActorEditor.actor;
using YamlDotNet.RepresentationModel;

namespace WonderActorEditor.components;

[ComponentData("YAML Component", "Write Plain YML and add the ActorParam name, used if a specific ActorParam is not Implemented as a Component")]
public class YAMLComponent : IComponent
{
    private string value = "";
    private string name = "";
    public void Render(int id, Actor parent)
    {
        string oldValue = value;
        string oldName = name;
        
        ImGui.InputText("name##$"+id, ref name, 512);
        ImGui.InputTextMultiline("yamlValue##$" + id, ref value, 80000, new Vector2(-200,200));

        if (oldValue != value || oldName != name)
        {
            parent.MarkUnsavedChanges();
        }
    }

    public string GetName()
    {
        if (name != "")
        {
            return name;
        }
        return "YAML Component";
    }

    public string GetActorParamId()
    {
        return GetName();
    }

    public YamlNode GetYAML()
    {
        return new YamlMappingNode
        {
            { "name", new YamlScalarNode("Bokoblin") },
            { "health", new YamlScalarNode("120") },
            {
                "drops",
                new YamlSequenceNode(
                    new YamlScalarNode("horn"),
                    new YamlScalarNode("fang")
                )
            }
        };
    }
}