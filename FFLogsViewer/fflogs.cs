using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.EzIpcManager;
using System;
using ECommons.ChatMethods;
using ECommons.SimpleGui;
using System.Diagnostics;

namespace FFLogsChat2Integration;
#pragma warning disable
public class Chat2IPC : IDisposable
{
    string? CurrentID;
    [EzIPC] Func<string> Register;
    [EzIPC] Action<string> Unregister;

    [EzIPCEvent]
    void Available()
    {
        CurrentID = Register();
        PluginLog.Debug($"Chat2 id={CurrentID}");
    }

    [EzIPCEvent]
    void Invoke(string id, PlayerPayload? snd, ulong contentId, Payload? payload, SeString? senderString, SeString? content)
    {
        if (id != CurrentID || snd == null) return;
        
        string playerName = snd.PlayerName.ToString();
        string worldName = snd.World.RowId.ToString();
        
        if (ImGui.Selectable("[FFLogs] Open Profile"))
        {
            string url = $"https://www.fflogs.com/character/{worldName}/{playerName}";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }

    public Chat2IPC()
    {
        EzIPC.Init(this, "ChatTwo", SafeWrapper.AnyException);
        Available();
    }

    public void Dispose()
    {
        if (CurrentID != null) Unregister(CurrentID);
    }
}
