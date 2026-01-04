namespace WonderActorEditor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;

public sealed class ImGuiFileBrowser
{
	public string RootPath { get; private set; } = "";
	public string CurrentPath { get; private set; } = "";

	public string? SelectedPath { get; private set; }
	public bool SelectedIsDirectory { get; private set; }

	// Grid settings
	public float LeftTreeWidth = 300f;
	public float IconSize = 64f;
	public float CellPadding = 10f;
	public float TextHeight = 34f; // room for 1-2 lines

	// Cache
	private readonly Dictionary<string, string[]> _dirChildrenCache = new();
	private readonly Dictionary<string, DirectoryListing> _listingCache = new();

	private sealed class DirectoryListing
	{
		public string[] Directories = Array.Empty<string>();
		public string[] Files = Array.Empty<string>();
		public DateTime CachedAtUtc;
	}

	public ImGuiFileBrowser(string? rootPath = null)
	{
		if (!string.IsNullOrWhiteSpace(rootPath))
			SetRoot(rootPath);
	}

	public void SetRoot(string rootPath)
	{
		if (!Directory.Exists(rootPath))
			throw new DirectoryNotFoundException(rootPath);

		RootPath = Path.GetFullPath(rootPath);
		CurrentPath = RootPath;
		SelectedPath = null;
		SelectedIsDirectory = false;

		_dirChildrenCache.Clear();
		_listingCache.Clear();
	}

	public void Draw()
	{
		if (string.IsNullOrWhiteSpace(RootPath) || !Directory.Exists(RootPath))
		{
			ImGui.TextDisabled("No root folder selected.");
			return;
		}

		// Two columns layout: left tree fixed, right fill
		ImGui.Columns(2, "##filebrowser_cols", true);
		ImGui.SetColumnWidth(0, LeftTreeWidth);

		DrawLeftTree();
		ImGui.NextColumn();
		DrawRightGrid();

		ImGui.Columns(1);
	}

	private void DrawLeftTree()
	{
		ImGui.BeginChild("##fb_tree", new Vector2(0, 0), true);

		ImGui.TextUnformatted("Folders");
		ImGui.Separator();

		DrawDirectoryNode(RootPath);

		ImGui.EndChild();
	}

	private void DrawRightGrid()
	{
		ImGui.BeginChild("##fb_grid", new Vector2(0, 0), true);

		// Toolbar
		ImGui.TextUnformatted("Files");
		ImGui.SameLine();
		ImGui.TextDisabled(CurrentPath);

		ImGui.Separator();

		// Back / Up
		if (ImGui.Button("Up"))
		{
			var parent = Directory.GetParent(CurrentPath);
			if (parent != null && parent.Exists && IsUnderRoot(parent.FullName))
				CurrentPath = parent.FullName;
		}
		ImGui.SameLine();
		if (ImGui.Button("Refresh"))
		{
			Invalidate(CurrentPath);
		}

		ImGui.Separator();

		// Listing
		var listing = GetListing(CurrentPath);

		// Grid layout
		float avail = ImGui.GetContentRegionAvail().X;
		float cellW = IconSize + CellPadding * 2;
		int columns = Math.Max(1, (int)(avail / cellW));

		// Render directories first
		DrawGridItems(listing.Directories, isDir: true, columns, cellW);

		// Then files
		DrawGridItems(listing.Files, isDir: false, columns, cellW);

		ImGui.EndChild();
	}

private void DrawGridItems(string[] paths, bool isDir, int columns, float cellW)
{
	if (paths.Length == 0) return;

	ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(CellPadding, CellPadding));

	int col = 0;
	for (int i = 0; i < paths.Length; i++)
	{
		if (col > 0) ImGui.SameLine();

		DrawGridCell(paths[i], isDir, cellW);

		col++;
		if (col >= columns)
			col = 0;
	}

	ImGui.PopStyleVar();
}

private void DrawGridCell(string fullPath, bool isDir, float cellW)
{
	string name = Path.GetFileName(fullPath);
	if (string.IsNullOrEmpty(name)) name = fullPath;

	ImGui.PushID(fullPath);

	// Layout sizes (LOCAL space)
	Vector2 cellSize = new Vector2(cellW, IconSize + TextHeight + CellPadding * 2);

	// Reserve space for this cell and make it clickable
	Vector2 cellMin = ImGui.GetCursorScreenPos(); // for drawing only
	ImGui.BeginGroup();

	// Clickable area = full cell
	ImGui.InvisibleButton("##cell", cellSize);

	bool hovered = ImGui.IsItemHovered();
	bool clicked = ImGui.IsItemClicked(ImGuiMouseButton.Left);
	bool doubleClicked = hovered && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);

	if (clicked)
	{
		SelectedPath = fullPath;
		SelectedIsDirectory = isDir;
	}

	// Draw background (selection/hover)
	bool selected = string.Equals(SelectedPath, fullPath, StringComparison.OrdinalIgnoreCase);

	var draw = ImGui.GetWindowDrawList();
	uint bg = 0;
	if (selected) bg = ImGui.GetColorU32(ImGuiCol.HeaderActive);
	else if (hovered) bg = ImGui.GetColorU32(ImGuiCol.HeaderHovered);

	if (bg != 0)
		draw.AddRectFilled(cellMin, cellMin + cellSize, bg, 6f);

	// Draw icon (placeholder)
	Vector2 iconPos = cellMin + new Vector2(CellPadding, CellPadding);
	string icon = isDir ? "📁" : "📄";
	draw.AddText(iconPos, ImGui.GetColorU32(ImGuiCol.Text), icon);

	// Draw filename under icon - use ImGui text with wrapping, but DON'T mess with layout cursor
	// We temporarily move the cursor for text, then restore it.
	Vector2 savedCursor = ImGui.GetCursorPos();

	// Set cursor to text start (LOCAL cursor pos relative to window content)
	// We can compute local cursor by using GetCursorPos() BEFORE InvisibleButton:
	// But we already called InvisibleButton which advanced the cursor.
	// So instead: set screen pos for text, but then restore cursor.
	Vector2 textPos = cellMin + new Vector2(CellPadding, CellPadding + IconSize);
	ImGui.SetCursorScreenPos(textPos);

	float textMaxW = cellW - CellPadding * 2;
	ImGui.PushTextWrapPos(ImGui.GetCursorPosX() + textMaxW);
	ImGui.TextUnformatted(name);
	ImGui.PopTextWrapPos();

	// Restore cursor to end of cell group (after InvisibleButton)
	ImGui.SetCursorPos(savedCursor);

	// Double click: open folder
	if (doubleClicked && isDir)
	{
		CurrentPath = fullPath;
		Invalidate(CurrentPath); // optional: refresh right pane
	}

	ImGui.EndGroup();

	ImGui.PopID();
}


	private void DrawDirectoryNode(string path)
	{
		string label = Path.GetFileName(path);
		if (string.IsNullOrEmpty(label)) label = path;

		bool hasChildren = DirectoryHasSubdirectories(path);

		ImGuiTreeNodeFlags flags =
			ImGuiTreeNodeFlags.OpenOnArrow |
			ImGuiTreeNodeFlags.SpanAvailWidth;

		// Leaf if no subdirs
		if (!hasChildren) flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

		// Highlight if current path
		if (string.Equals(CurrentPath, path, StringComparison.OrdinalIgnoreCase))
			flags |= ImGuiTreeNodeFlags.Selected;

		bool opened = ImGui.TreeNodeEx(label + "##" + path, flags);

		// Single click: set current dir
		if (ImGui.IsItemClicked())
		{
			CurrentPath = path;
		}

		if (!hasChildren) return;

		if (opened)
		{
			foreach (var child in GetSubdirectories(path))
				DrawDirectoryNode(child);

			ImGui.TreePop();
		}
	}

	private bool DirectoryHasSubdirectories(string path)
	{
		try
		{
			using var e = Directory.EnumerateDirectories(path).GetEnumerator();
			return e.MoveNext();
		}
		catch { return false; }
	}

	private string[] GetSubdirectories(string path)
	{
		if (_dirChildrenCache.TryGetValue(path, out var cached))
			return cached;

		try
		{
			var dirs = Directory.GetDirectories(path);
			Array.Sort(dirs, StringComparer.OrdinalIgnoreCase);
			_dirChildrenCache[path] = dirs;
			return dirs;
		}
		catch
		{
			_dirChildrenCache[path] = Array.Empty<string>();
			return Array.Empty<string>();
		}
	}

	private DirectoryListing GetListing(string path)
	{
		if (_listingCache.TryGetValue(path, out var cached))
			return cached;

		var listing = new DirectoryListing();
		try
		{
			var dirs = Directory.GetDirectories(path);
			var files = Directory.GetFiles(path);

			Array.Sort(dirs, StringComparer.OrdinalIgnoreCase);
			Array.Sort(files, StringComparer.OrdinalIgnoreCase);

			listing.Directories = dirs;
			listing.Files = files;
			listing.CachedAtUtc = DateTime.UtcNow;
		}
		catch
		{
			listing.Directories = Array.Empty<string>();
			listing.Files = Array.Empty<string>();
			listing.CachedAtUtc = DateTime.UtcNow;
		}

		_listingCache[path] = listing;
		return listing;
	}

	public void Invalidate(string? pathPrefix = null)
	{
		if (pathPrefix == null)
		{
			_dirChildrenCache.Clear();
			_listingCache.Clear();
			return;
		}

		foreach (var key in new List<string>(_dirChildrenCache.Keys))
			if (key.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase))
				_dirChildrenCache.Remove(key);

		foreach (var key in new List<string>(_listingCache.Keys))
			if (key.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase))
				_listingCache.Remove(key);
	}

	private bool IsUnderRoot(string path)
	{
		string full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
		string root = RootPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
		return full.StartsWith(root, StringComparison.OrdinalIgnoreCase);
	}
}
