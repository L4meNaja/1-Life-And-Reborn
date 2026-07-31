using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupEnemyAndPlayerTag
{
    [MenuItem("Tools/Setup Enemy And Player Tag")]
    public static void Execute()
    {
        // เปิด PlayerTest scene
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/PlayerTest.unity");

        // === ตั้ง Tag "Player" ให้กับ Player ===
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.tag = "Player";
            Debug.Log("ตั้ง Tag 'Player' ให้กับ Player แล้ว!");
        }
        else
        {
            Debug.LogWarning("ไม่เจอ GameObject ชื่อ 'Player'");
        }

        // === สร้าง Enemy (Red Capsule) ===
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = "Enemy";
        enemy.transform.position = new Vector3(5f, 1f, 5f);

        // ใส่สีแดง
        Renderer renderer = enemy.GetComponent<Renderer>();
        Material redMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        redMat.SetColor("_BaseColor", Color.red);
        renderer.material = redMat;

        // ใส่ EnemyAI script
        EnemyAI enemyAI = enemy.AddComponent<EnemyAI>();
        enemyAI.maxHP = 50f;
        enemyAI.currentHP = 50f;
        enemyAI.attackDamage = 10f;
        enemyAI.moveSpeed = 4f;
        enemyAI.stoppingDistance = 2f;
        enemyAI.attackCooldown = 1.5f;
        enemyAI.attackRange = 2.5f;

        // เซฟ scene
        EditorSceneManager.SaveScene(scene);

        Debug.Log("สร้าง Enemy (Red Capsule) + ตั้ง Player Tag เรียบร้อยแล้ว!");
    }
}
