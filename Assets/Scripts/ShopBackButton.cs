using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ShopBackButton : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "MainScene";
    [SerializeField] private string returnSpawnPointName = "SpawnFromShop";

    private bool isReturning;

    private void Update()
    {
        if (isReturning || Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GoBack();
        }
    }

    public void GoBack()
    {
        if (isReturning) return;
        isReturning = true;

        GameProgress.NextSpawnPointName = returnSpawnPointName;
        SceneManager.LoadScene(mainSceneName);
    }
}