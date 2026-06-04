public static class GameLayers
{
    public const string Player = "Player";
    public const string Weapon = "Weapon";
    public const string WeaponWorld = "WeaponWorld";
    public const string WeaponProjectile = "WeaponProjectile";
    public const string Enemy = "Enemy";
    public const string Enterance = "Enterance";
    public const string Interactive = "Interactive";
    public const string NPC = "NPC";
    public const string Ground = "Ground";

    public static int PlayerIndex => UnityEngine.LayerMask.NameToLayer(Player);
    public static int WeaponIndex => UnityEngine.LayerMask.NameToLayer(Weapon);
    public static int WeaponWorldIndex => UnityEngine.LayerMask.NameToLayer(WeaponWorld);
    public static int WeaponProjectileIndex => UnityEngine.LayerMask.NameToLayer(WeaponProjectile);
    public static int EnemyIndex => UnityEngine.LayerMask.NameToLayer(Enemy);
}
