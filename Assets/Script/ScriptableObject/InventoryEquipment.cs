using UnityEngine;

[CreateAssetMenu(fileName = "InventoryEquipment", menuName = "Scriptable Objects/InventoryEquipment")]
public class InventoryEquipment : ScriptableObject
{
    public ItemSlot itemSlot;

    public float damageValue;
    public int reach;
    public float attackSpeed;
    public int maxAmmo;
    public int currentAmmo;
    public int invAmmo;
    public int count;
    public Texture2D itemTexture;
    public Vector3 textureSize;

    // เปลี่ยนจาก GameObject เป็น Mesh (หรือ Transform) แทน จะลากใส่ SO ได้ทันที!
    public Mesh itemMesh; 
    public Material itemMaterial; // เก็บ Material เผื่อไว้ลงสีด้วย
}

public enum ItemSlot { Primary , Secondary , Melee , HealthPotion , ShieldPotion }