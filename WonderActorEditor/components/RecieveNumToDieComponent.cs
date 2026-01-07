using ImGuiNET;
using WonderActorEditor.actor;
using YamlDotNet.RepresentationModel;

namespace WonderActorEditor
{
    [ComponentData(
        "Receive Num To Die",
        "Specifies the number of hits the actor can take before dying"
    )]
    public class ReceiveNumToDieComponent : IComponent
    {
        private int hitsToDie = 1;

        public void Render(int id, Actor parent)
        {
            int oldValue = hitsToDie;

            ImGui.InputInt($"Hits to Die##{id}", ref hitsToDie);

            if (oldValue != hitsToDie)
            {
                parent.MarkUnsavedChanges();
            }
        }

        public string GetName()
        {
            return "Receive Num To Die";
        }

        public string GetActorParamId()
        {
            return "ReceiveNumToDieParam";
        }

        public YamlNode GetYAML()
        {
            return new YamlMappingNode
            {
                { "hits_to_die", new YamlScalarNode(hitsToDie.ToString()) }
            };
        }
    }
}