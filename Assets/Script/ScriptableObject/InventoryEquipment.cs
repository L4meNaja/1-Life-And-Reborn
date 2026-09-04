using UnityEngine;

[CreateAssetMenu(fileName = "InventoryEquipment", menuName = "Scriptable Objects/InventoryEquipment")]
public class InventoryEquipment : ScriptableObject
{
    public ItemSlot itemSlot;

    public float damageValue;
    public float attackSpeed;
    public int maxAmmo;
    public int currentAmmo;
    public int invAmmo;
    public Texture2D itemTexture;
    public Vector3 textureSize;

    public float reloadTime;
    public float healValue;
    public float shieldValue;

    // เปลี่ยนจาก GameObject เป็น Mesh (หรือ Transform) แทน จะลากใส่ SO ได้ทันที!
    public Mesh itemMesh; 
    public Material itemMaterial; // เก็บ Material เผื่อไว้ลงสีด้วย

    public Vector3 itemPos;
    public Vector3 itemRot;
    public Vector3 itemSize;

    public Vector3 ammoSpawnPos;

    public int bulletCount;
    public float bulletSpread;
    public float bulletSize;
    public float camRecoil;
}

public enum ItemSlot { Primary , Secondary , HealthPotion , ShieldPotion }