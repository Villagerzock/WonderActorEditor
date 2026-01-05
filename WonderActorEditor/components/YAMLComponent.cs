using System.Numerics;
using ImGuiNET;
using Syroot.NintenTools.Byaml;

namespace WonderActorEditor.components;

[ComponentData("YAML Component", "Write Plain YML and add the ActorParam name, used if a specific ActorParam is not Implemented as a Component")]
public class YAMLComponent : IComponent
{
    private string value = "";
    private string name = "";
    public void Render(int id)
    {
        ImGui.InputText("name##$"+id, ref name, 512);
        ImGui.InputTextMultiline("yamlValue##$" + id, ref this.value, 80000, new Vector2(-200,200));
        
    }

    public string GetName()
    {
        if (name != "")
        {
            return name;
        }
        return "YAML Component";
    }

    public string GetActorParamID()
    {
        return GetName();
    }
}