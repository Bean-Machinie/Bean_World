namespace BeanWorld.Assets;

/// <summary>
/// String constants for all texture asset paths.
/// Always reference textures through these constants — never use raw strings.
/// Add a new constant here whenever a new texture is added to the content pipeline.
/// </summary>
public static class TextureAssets
{
    public const string Tileset          = "Textures/tileset";
    public const string UIAtlas          = "Textures/ui_atlas";

    // Player animation strips — one PNG per state (Sword, without shadow).
    // Place the actual PNGs in Content/Textures/Player/ then build content.
    public const string PlayerIdle       = "Textures/Player/Sword_Idle";
    public const string PlayerWalk       = "Textures/Player/Sword_Walk";
    public const string PlayerAttack     = "Textures/Player/Sword_attack";
    public const string PlayerWalkAttack = "Textures/Player/Sword_Walk_Attack";
    public const string PlayerHurt       = "Textures/Player/Sword_Hurt";
    public const string PlayerDeath      = "Textures/Player/Sword_Death";
}
