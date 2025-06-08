using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDevam : MonoBehaviour
{
    void Start()
    {
        // Baþlangýçta kaldýðý leveli al
        currentLevel = PlayerPrefs.GetInt("SavedLevel", 1);
        Debug.Log("Bu sahne için kayýtlý level: " + currentLevel);
    }

    public int currentLevel = 1;

    public void CompleteLevel()
    {
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt("SavedLevel", nextLevel);
        PlayerPrefs.Save();
        Debug.Log("Level kaydedildi: " + nextLevel);
    }
}