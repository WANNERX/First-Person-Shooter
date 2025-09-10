using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelResetTarget : Actor
{
    protected override void Death()
    {
        WeaponController weapon = FindObjectOfType<WeaponController>();
        if (weapon != null)
        {
            PlayerPrefs.SetInt("SavedAmmo", weapon.ammoCount);
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}