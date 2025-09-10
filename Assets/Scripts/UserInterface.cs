using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Singleton;

    [Header("Ammo")]
    public TextMeshProUGUI bulletCount_Text;

    [Header("Level")]
    public TextMeshProUGUI levelCount_Text;

    void Awake()
    { UserInterface.Singleton = this; }

    public void UpdateBulletCounter(int ammoCount, int maxAmmo)
    { bulletCount_Text.text = ammoCount + " / " + maxAmmo; }

    public void UpdateLevelCounter(int level)
    { levelCount_Text.text = "Level " + level; }
}