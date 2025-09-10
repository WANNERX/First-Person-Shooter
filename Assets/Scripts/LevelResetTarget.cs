using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelResetTarget : Actor
{
    protected override void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}