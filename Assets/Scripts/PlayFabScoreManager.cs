using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabScoreManager : MonoBehaviour
{
    public static PlayFabScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Oyuncu giri� yapt�ktan sonra local ve PlayFab skorlar�n� bir kere senkronize eder.
    /// </summary>
    public void SyncScoreOnLogin()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result =>
            {
                int serverScore = 0;
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == "HighScore")
                    {
                        serverScore = stat.Value;
                        break;
                    }
                }

                int localScore = PlayerPrefs.GetInt("HighScore", 0);

                if (localScore > serverScore)
                {
                    // Yereldeki skor daha y�ksekse PlayFab'a g�nder
                    SendHighScore(localScore);
                }
                else if (serverScore > localScore)
                {
                    // PlayFab skoru daha y�ksekse yereli g�ncelle
                    PlayerPrefs.SetInt("HighScore", serverScore);
                    Debug.Log("Yerel skor g�ncellendi: " + serverScore);
                }
                else
                {
                    Debug.Log("Skorlar zaten e�it.");
                }
            },
            error =>
            {
                Debug.LogError("Skor �ekme hatas�: " + error.GenerateErrorReport());
            });
    }

    /// <summary>
    /// Yeni skor PlayFab'a g�nderilir. Oyun i�inde s�rekli �a�r�lmamal�.
    /// </summary>
    public void SendHighScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log("Yeni y�ksek skor PlayFab'a g�nderildi: " + score),
            error => Debug.LogError("Skor g�nderme hatas�: " + error.GenerateErrorReport()));
    }
}