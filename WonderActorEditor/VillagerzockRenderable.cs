using System.Numerics;
using System.Reflection;
using ImGuiNET;
using WonderActorEditor.actor;

namespace WonderActorEditor;

public class VillagerzockRenderable : ImGuiRenderable
{
    private Actor? _addComponentTo = null;
    private Actor? _closeAfterSaving = null;
    
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
                for (int i = 0; i < Program.openFiles.Count; i++)
                {
                    bool shouldClose = true;
                    Actor currentActor = Program.openFiles[i];
                    if (ImGui.BeginTabItem(currentActor.Name,ref shouldClose, currentActor.HasUnsavedChanges ? ImGuiTabItemFlags.UnsavedDocument : ImGuiTabItemFlags.None))
                    {
                        if (!shouldClose && !currentActor.HasUnsavedChanges)
                        {
                            Program.openFiles.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (!shouldClose && currentActor.HasUnsavedChanges)
                        {
                            _closeAfterSaving = currentActor;
                        }
                        float spacing = ImGui.GetStyle().ItemSpacing.X;
                        float firstPart = (ImGui.GetContentRegionAvail().X - spacing) * 0.15f;
                        float secondPart = (ImGui.GetContentRegionAvail().X - spacing) - firstPart;
                        float height = 0; // 0 = nimmt restliche Höhe

                        ImGui.BeginChild("##ActorRenderer", new Vector2(firstPart, height), false);

                        ImGui.BeginChild("##ActorModelRenderer", new Vector2(firstPart, firstPart), true);
					        
                        ImGui.Text("Here Comes A Model");
                        ImGui.EndChild();
                        ImGui.PushFont(Program.titleFont);
                        ImGui.Text(currentActor.Name);
                        ImGui.PushFont(Program.defaultFont);
					        
                        ImGui.EndChild();

                        ImGui.SameLine();

                        ImGui.BeginChild("##ActorComponents", new Vector2(secondPart, height), true);
                        for (int j = 0; j < currentActor.Components.Count; j++)
                        {
                            ImGui.BeginGroup();
                           
                            IComponent component = currentActor.Components[j];
                            
                            ImGui.PushFont(Program.titleFont);
                            ImGui.Text(component.GetName());
                            ImGui.PushFont(Program.defaultFont);
                            
                            ImGui.SameLine();
                            
                            ImGui.BeginGroup();
                            component.Render(j, currentActor);
                            ImGui.EndGroup();
                            
                            ImGui.EndGroup();
                        }
                        if (ImGui.Button("+ Add Component", new Vector2(-1, 40)))
                        {
                            _addComponentTo = currentActor;
                        }
                        ImGui.EndChild();
                    }
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
        ImGui.End();
            
        if (_addComponentTo != null)
        {
            ImGui.SetNextWindowSize(new Vector2(300,400));
            bool open = true;
            if (ImGui.Begin("Add Component", ref open, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking))
            {
                if (!open)
                {
                    _addComponentTo = null;
                    return;
                }
                foreach (Type componentType in Program.allComponentTypes)
                {
                    ComponentData? data = componentType.GetCustomAttribute<ComponentData>();
                    if (data != null)
                    {
                        if (ImGui.Selectable(data.name))
                        {
                            IComponent? component = Activator.CreateInstance(componentType) as IComponent;
                            if (component != null)
                            {
                                _addComponentTo?.Components.Add(component);
                                _addComponentTo?.MarkUnsavedChanges();
                            }
                            _addComponentTo = null;
                        }
                    }
                }
            }
            ImGui.End();
        }
    }
}