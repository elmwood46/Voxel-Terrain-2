using System;
using System.Diagnostics;
using GDebugPanelGodot.Core;
using GDebugPanelGodot.DebugActions.Containers;
using GDebugPanelGodot.Examples.OptionsObjects;
using GDebugPanelGodot.Extensions;
using Godot;

public partial class DebugManager : Control
{
    readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    [Export] public RichTextLabel debugLogView;

    public static DebugManager Instance { get; private set; }

    public event Action DebugOverlayToggled;
    public event Action ToggleDebugLog;

    bool _toggle1;
    bool _toggle2;
    int _int1;
    float _float1;

    public static void Log(String message)
    {
        Instance.debugLog += message + "\n";
        Instance.debugLogView.AppendText(message + "\n");
        Instance.debugLogView.ScrollToLine(Instance.debugLogView.GetLineCount());
    }

    public string debugLog {get; set;}

    ExampleOptionsObject.ExampleEnum _enum1;
    
    public override void _Ready()
    {

        #if DEBUG
            debugLog += "Debug build\n";
        # else 
            debugLogView.Hide();
            Instance.Hide();
        #endif

        Instance = this;
        ProcessMode = ProcessModeEnum.Always;

        IDebugActionsSection section1 = GDebugPanel.AddNonCollapsableSection("Section 1");
        section1.AddInfoDynamic(() =>_stopwatch.Elapsed.ToString());
        section1.AddInfoDynamic(() => $"PlayerPosition: {Player.Instance.Position}");
        section1.AddInfoDynamic(() => $"PlayerChunkPosition: {Mathf.FloorToInt(Player.Instance.Position.X/Chunk.Dimensions.X)},{Mathf.FloorToInt(Player.Instance.Position.Z/Chunk.Dimensions.Z)}");
        
        IDebugActionsSection section4 = GDebugPanel.AddSection("Debug Log");
        section4.AddInfoDynamic(() => Instance.debugLog);

        this.Visible = false;

        // subscribe to the event
        DebugOverlayToggled += OnDebugOverlayToggled;
        ToggleDebugLog += OnToggleDebugLog;
    }

   public override void _UnhandledInput(InputEvent @event)
    {
        #if DEBUG

        // Check if the input action is pressed
        if (@event.IsActionPressed("show_debug_overlay"))
        {
            DebugOverlayToggled?.Invoke();
        }

        if (@event.IsActionPressed("toggle_debug_log"))
        {
            ToggleDebugLog?.Invoke();
        }

        #endif
    }

    private void OnDebugOverlayToggled()
    {
        if (Instance.Visible)
        {
            Instance.Hide();
            GetTree().Paused = false;
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else
        {
            Instance.Show();
            GDebugPanel.Show(Instance);
            GetTree().Paused = true;
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    private void OnToggleDebugLog()
    {
        Instance.debugLogView.Visible = !Instance.debugLogView.Visible;
    }
}