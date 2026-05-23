using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "9toHell/Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float baseFireRate = 1.5f;
    public float attackRange = 7f;
    public float baseDamage = 10f;
    public float projectileSpeed = 10f;
    public float lifetime = 3f;
    public GameObject projectilePrefab;
    public string poolTag;
}