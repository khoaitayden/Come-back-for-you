using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerBehavior : MonoBehaviour
{
    public static event Action OnPlayerDied;

    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        OnPlayerDied?.Invoke();
        Debug.Log("Player died!");
        RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}