using ImGuiNET;
using WonderActorEditor.actor;
using YamlDotNet.RepresentationModel;

namespace WonderActorEditor.components;

[ComponentData("Damage Reaction Table", "Specify the Reactions the Actor has on Different Damage Types")]
public class DamageReactionComponent : IComponent
{
    private static string[] DamageTypes =
    {
        "BlockBody",
        "Body",
        "BodyHuge",
        "BodyLarge",
        "BodySmall",
        "Bomb",
        "BombMedium",
        "Bubble",
        "CarriedObject",
        "Eat",
        "Fire",
        "HipDropDrill"
    };
    private Dictionary<string, string> valueMap = new Dictionary<string, string>();
    public void Render(int id, Actor parent)
    {
        ImGui.Text("Enter Values:");
        float half = ImGui.GetContentRegionAvail().X * 0.5f;
        foreach (string damageType in DamageTypes)
        {
            ImGui.SetNextItemWidth(half);
            ImGui.Text(damageType);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(half);
            string val = valueMap.GetValueOrDefault(damageType,"");
            ImGui.InputText("##damageReaction$" + damageType + "$" + id, ref val, 64);
            valueMap[damageType] = val;
        }
    }

    public string GetActorParamId()
    {
        return "DamageReactionTable";
    }

    public YamlNode GetYAML()
    {
        return new YamlMappingNode();
    }
}