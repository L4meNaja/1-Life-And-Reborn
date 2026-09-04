using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldBar : MonoBehaviour
{
    public Slider shieldBar;
    public TextMeshProUGUI shieldText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shieldBar.maxValue = PlayerStats.playerStats.maxShield;
        shieldBar.value = PlayerStats.playerStats.currentShield;
        shieldText.text = PlayerStats.playerStats.currentShield.ToString("F1");
    }
}
