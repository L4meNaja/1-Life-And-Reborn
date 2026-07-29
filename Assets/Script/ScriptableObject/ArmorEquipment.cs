using UnityEngine;

[CreateAssetMenu(fileName = "ArmorEquipment", menuName = "Scriptable Objects/ArmorEquipment")]
public class ArmorEquipment : ScriptableObject
{
    public EquipmentSlot equipSlot;

    public float armorValue;
    public float shieldValue;
    public float weightValue;
}

public enum EquipmentSlot { Head , Body , Legs , Feet }