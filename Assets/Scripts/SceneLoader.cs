using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadLeaderboardScene()
    {
        // Sahne yüklendikten sonra çalýþmasý için olayý baðla
        SceneManager.sceneLoaded += OnTabloSceneLoaded;

        // Sahneyi yükle
        SceneManager.LoadScene("Tablo");
    }

    private void OnTabloSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Tablo" && LeaderboardManager.Instance != null)
        {
            Debug.Log("Tablo sahnesi yüklendi, RefreshLeaderboard çaðrýlýyor");
            LeaderboardManager.Instance.RefreshLeaderboard();
        }

        // Baðlantýyý kaldýr, yoksa ileride ayný þey tekrar tekrar çaðrýlýr
        SceneManager.sceneLoaded -= OnTabloSceneLoaded;
    }
}