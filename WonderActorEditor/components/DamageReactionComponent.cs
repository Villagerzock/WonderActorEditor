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
    private Dictionary<string, string> _valueMap = new();
    public void Render(int id, Actor parent)
    {
        ImGui.Text("Enter Values:");
        foreach (string damageType in DamageTypes)
        {
            string val = _valueMap.GetValueOrDefault(damageType,"");
            ImGui.InputText($"{damageType}##damageReaction${id}", ref val, 64);
            _valueMap[damageType] = val;
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