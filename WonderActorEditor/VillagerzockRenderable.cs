using System.Numerics;
using ImGuiNET;

namespace WonderActorEditor;

public class VillagerzockRenderable : ImGuiRenderable
{
    public void Render()
    {
        if (Program.openFolder != null && ImGui.Begin("FileViewer"))
        {
            Program.browser.Draw();
            ImGui.End();
        }

        if (ImGui.Begin("Edit Actor"))
        {
            if (ImGui.BeginTabBar("openFiles"))
            {
                for (int i = 0; i < Program.openFiles.Length; i++)
                {
                    if (ImGui.BeginTabItem(Program.openFiles[i]))
                    {
                        float spacing = ImGui.GetStyle().ItemSpacing.X;
                        float firstPart = (ImGui.GetContentRegionAvail().X - spacing) * 0.15f;
                        float secondPart = (ImGui.GetContentRegionAvail().X - spacing) - firstPart;
                        float height = 0; // 0 = nimmt restliche Höhe

                        ImGui.BeginChild("##ActorRenderer", new Vector2(firstPart, height), false);

                        ImGui.BeginChild("##ActorModelRenderer", new Vector2(firstPart, firstPart), true);
					        
                        ImGui.Text("Here Comes A Model");
                        ImGui.EndChild();
                        ImGui.PushFont(Program.titleFont);
                        ImGui.Text(Program.openFiles[i]);
                        ImGui.PushFont(Program.defaultFont);
					        
                        ImGui.EndChild();

                        ImGui.SameLine();

                        ImGui.BeginChild("##ActorComponents", new Vector2(secondPart, height), true);
                        ImGui.Button("+ Add Component", new Vector2(-1, 40));
                        ImGui.EndChild();
                    }
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }

            ImGui.End();
        }
    }
}