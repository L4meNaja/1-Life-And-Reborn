using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.maxValue = PlayerStats.playerStats.maxHP;
        healthBar.value = PlayerStats.playerStats.currentHP;
        healthText.text = PlayerStats.playerStats.currentHP.ToString("F1");
    }
}
