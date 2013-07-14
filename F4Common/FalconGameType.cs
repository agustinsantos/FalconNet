
namespace FalconNet.F4Common
{
    // ==========================================
    // Game types
    // ==========================================
    public enum FalconGameType
    {
        game_PlayerPool = 0,
        game_InstantAction,
        game_Dogfight,
        game_TacticalEngagement,
        game_Campaign,
        game_MaxGameTypes, // This MUST be the last type (I use it as an array size) Please don't assign values individually
        // (Except for playerpool = 0)
    }
}
