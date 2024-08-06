using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsEnabled { get; set; } = true;
    
    public bool IsOverlayMovable { get; set; } = true;
    
    public bool IsFlashingEffectEnabled { get; set; } = true;
    
    public Vector4 PrimaryTextColor { get; set; } = new(0, 255, 0, 1f);
    
    public Vector4 SecondaryTextColor { get; set; } = new(255, 0, 0, 1f);
        
    public Vector4 BackgroundColor { get; set; } = new(0,0,0, 0.5f);
        
    public bool HideInCombat { get; set; } = true;
    
    public int RemainingTimeInSeconds { get; set; } = 600;
    
    public bool ShowIfLevelSynced { get; set; } = true;
    
    public bool ShowInExtreme { get; set; } = true;
    
    public bool ShowInSavage { get; set; } = true;
    
    public bool ShowInUltimate { get; set; } = true;


    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
