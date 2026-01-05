using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using NativeFileDialogNET;

namespace WonderActorEditor
{
    public static class Program
    {
	    public static string? openFolder
	    {
		    get;
		    set
		    {
			    if (field == value || value == null) return;
			    field = value;
			    browser.SetRoot(value);
		    }
	    }

	    public static string[] openFiles = {"test"};

	    public static ImGuiFileBrowser browser = new ImGuiFileBrowser(openFolder); // oder dein Projektpfad
	    private static ImGuiRenderable[] renderables =
		    {
			    new VillagerzockRenderable(),
			    new CitraRenderable()
		    };

	    public static ImFontPtr defaultFont;
	    public static ImFontPtr titleFont;

	    
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
// Window + GraphicsDevice erstellen
		VeldridStartup.CreateWindowAndGraphicsDevice(
			new WindowCreateInfo(
				x: 100,
				y: 100,
				windowWidth: 1280,
				windowHeight: 720,
				windowInitialState: WindowState.Normal,
				windowTitle: "Actor Editor"
			),
			new GraphicsDeviceOptions(
				debug: true,
				swapchainDepthFormat: null,
				syncToVerticalBlank: true,
				resourceBindingModel: ResourceBindingModel.Improved,
				preferStandardClipSpaceYDirection: true,
				preferDepthRangeZeroToOne: true
			),
			out Sdl2Window window,
			out GraphicsDevice gd
		);

		// CommandList für Rendering
		CommandList cl = gd.ResourceFactory.CreateCommandList();

		// ImGui Renderer
		ImGuiRenderer imgui = new ImGuiRenderer(
			gd,
			gd.MainSwapchain.Framebuffer.OutputDescription,
			window.Width,
			window.Height
		);
		var io = ImGui.GetIO();
		io.Fonts.Clear();

		defaultFont = io.Fonts.AddFontFromFileTTF("C:\\Windows\\Fonts\\segoeui.ttf", 18f);
		titleFont = io.Fonts.AddFontFromFileTTF("C:\\Windows\\Fonts\\segoeui.ttf", 36f);
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.ViewportsEnable;

		imgui.RecreateFontDeviceTexture(); // <- wichtig
		
		ImGui.PushFont(defaultFont);

		// Resize handling
		window.Resized += () =>
		{
			gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
			imgui.WindowResized(window.Width, window.Height);
		};

		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		double last = stopwatch.Elapsed.TotalSeconds;

		while (window.Exists)
		{
			// 1) Events/Input zuerst
			InputSnapshot snapshot = window.PumpEvents();
			if (!window.Exists) break;

			// 2) DeltaTime
			double now = stopwatch.Elapsed.TotalSeconds;
			float dt = (float)(now - last);
			last = now;
			if (dt <= 0) dt = 1f / 60f;

			// 3) ImGui Frame starten (macht intern NewFrame + Input)
			imgui.Update(dt, snapshot);

			// 4) Dockspace ROOT (Fullscreen)
			DrawDockspaceRoot(window);

			DrawWindows();

			// 6) Render
			cl.Begin();
			cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
			cl.ClearColorTarget(0, RgbaFloat.Black);

			imgui.Render(gd, cl);

			cl.End();
			gd.SubmitCommands(cl);
			gd.SwapBuffers(gd.MainSwapchain);
			// gd.WaitForIdle(); // am besten raus, sonst ultra langsam
		}


		// Cleanup
		gd.WaitForIdle();
		imgui.Dispose();
		cl.Dispose();
		gd.Dispose();
        }

        private static void DrawWindows()
        {
	        for (int i = 0; i < renderables.Length; i++)
	        {
		        renderables[i].Render();
	        }
        }

        private static void DrawDockspaceRoot(Sdl2Window window)
        {
	        var viewport = ImGui.GetMainViewport();

	        ImGui.SetNextWindowPos(viewport.Pos);
	        ImGui.SetNextWindowSize(viewport.Size);
	        ImGui.SetNextWindowViewport(viewport.ID);

	        ImGuiWindowFlags flags =
		        ImGuiWindowFlags.NoDocking |
		        ImGuiWindowFlags.NoTitleBar |
		        ImGuiWindowFlags.NoCollapse |
		        ImGuiWindowFlags.NoResize |
		        ImGuiWindowFlags.NoMove |
		        ImGuiWindowFlags.NoBringToFrontOnFocus |
		        ImGuiWindowFlags.NoNavFocus |
		        ImGuiWindowFlags.MenuBar;

	        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
	        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

	        ImGui.Begin("DockSpaceRoot", flags);
	        ImGui.PopStyleVar(2);

	        uint dockspaceId = ImGui.GetID("MyDockSpace");
	        ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.None);

	        if (ImGui.BeginMenuBar())
	        {
		        if (ImGui.BeginMenu("File"))
		        {
			        if (ImGui.MenuItem("Exit")) window.Close();
			        if (ImGui.MenuItem("Open Folder")) OpenFolderDialog();
			        ImGui.EndMenu();
		        }
		        ImGui.EndMenuBar();
	        }

	        ImGui.End();
        }

        private static void OpenFolderDialog()
        {
	        var dialog = new NativeFileDialog()
		        .SelectFolder();

	        if (dialog.Open(out string? result) == DialogResult.Okay)
	        {
		        Console.WriteLine("Opening Folder: " + result);
		        openFolder = result;
	        }

        }
    }
}