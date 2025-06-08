using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.ReloadAttribute;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    int currentLevel;

    void Start()
    {
        // Sahne adýndan Level numarasýný çýkar
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level"))
        {
            string[] parts = sceneName.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1], out int levelNum))
            {
                currentLevel = levelNum;
            }
        }

        Debug.Log("Bu sahne: Level " + currentLevel);
    }

    public void CompleteLevel()
    {
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt("SavedLevel", nextLevel);
        PlayerPrefs.Save();
        Debug.Log("Level kaydedildi: " + nextLevel);
        SceneManager.LoadScene("Level " + nextLevel);
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne geçince kaybolmasýn istiyorsan
        }
        else
        {
            Destroy(gameObject); // Ýkinci bir instance oluþmasýný engeller
        }
    }

    internal void NextLevel()
    {
        throw new NotImplementedException();
    }
}