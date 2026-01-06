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
                            ImGui.EndTabItem();
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
                            if (ImGui.Button("Remove##$"+j+"$"+i))
                            {
                                currentActor.Components.RemoveAt(j);
                                j--;
                            }
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
            ImGui.SetNextWindowSize(new Vector2(700,400));
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
                    if (data == null) continue;

                    if (!data.HasMultiple && ActorAlreadyHasComponent(_addComponentTo, componentType)) continue;

                    float paddingY = ImGui.GetStyle().FramePadding.Y;
                    float spacingY = ImGui.GetStyle().ItemInnerSpacing.Y;

// Textgrößen berechnen
                    Vector2 nameSize = ImGui.CalcTextSize(data.Name);
                    Vector2 descSize = ImGui.CalcTextSize(data.Description ?? "");

// Höhe für 2 Zeilen + Padding
                    float height =
                        nameSize.Y +
                        (data.Description != null ? (spacingY + descSize.Y) : 0f) +
                        paddingY * 2f;

// Volle Breite nutzen
                    Vector2 size = new Vector2(0f, height);

// Unsichtbares Label, damit ID eindeutig ist (wichtig bei gleichen Namen)
                    string id = $"##addcomp_{componentType.FullName}";

                    Vector2 startPos = ImGui.GetCursorScreenPos();

                    bool clicked = ImGui.Selectable(id, false, ImGuiSelectableFlags.None, size);

                    Vector2 textPos = startPos + new Vector2(ImGui.GetStyle().FramePadding.X, ImGui.GetStyle().FramePadding.Y);

// Name
                    ImGui.GetWindowDrawList().AddText(textPos, ImGui.GetColorU32(ImGuiCol.Text), data.Name);

// Description darunter (etwas “disabled”-mäßig)
                    if (!string.IsNullOrEmpty(data.Description))
                    {
                        Vector2 descPos = textPos + new Vector2(0f, nameSize.Y + spacingY);
                        ImGui.GetWindowDrawList().AddText(descPos, ImGui.GetColorU32(ImGuiCol.TextDisabled), data.Description);
                    }

                    if (clicked)
                    {
                        if (Activator.CreateInstance(componentType) is IComponent component)
                        {
                            _addComponentTo?.Components.Add(component);
                            _addComponentTo?.MarkUnsavedChanges();
                        }

                        _addComponentTo = null;
                        ImGui.CloseCurrentPopup(); // falls du im Popup bist
                    }

                }
            }
            ImGui.End();
        }
    }

    public bool ActorAlreadyHasComponent(Actor? actor, Type componentType)
    {
        if (actor == null) return false;
        foreach (IComponent component in  actor.Components)
        {
            if (component.GetType() == componentType)
            {
                return true;
            }
        }

        return false;
    }
}