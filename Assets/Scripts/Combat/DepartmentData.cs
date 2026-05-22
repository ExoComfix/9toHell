using UnityEngine;

[CreateAssetMenu(fileName = "NewDepartmentData", menuName = "9toHell/Data/Department Data")]
public class DepartmentData : ScriptableObject
{
    public string departmentName;
    public PlayerController.DepartmentType departmentType;

    public float moveSpeed = 5f;
    public float maxBurnout = 100f;
    public float attackCooldownModifier = 1f;
    public float damageMultiplier = 1f;
    public float rangeBonus = 0f;
}