using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.maxValue = PlayerStats.playerStats.maxHP;
        healthBar.value = PlayerStats.playerStats.currentHP;
    }
}
