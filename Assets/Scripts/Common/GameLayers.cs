public static class GameLayers
{
    public const string Player = "Player";
    public const string Weapon = "Weapon";
    public const string Enemy = "Enemy";
    public const string Enterance = "Enterance";
    public const string Interactive = "Interactive";
    public const string NPC = "NPC";
    public const string Ground = "Ground";
    public static int PlayerIndex => UnityEngine.LayerMask.NameToLayer(Player);
    public static int WeaponIndex => UnityEngine.LayerMask.NameToLayer(Weapon);
}
