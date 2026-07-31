using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    public Slider shieldBar;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shieldBar.maxValue = PlayerStats.playerStats.maxShield;
        shieldBar.value = PlayerStats.playerStats.currentShield;
    }
}
