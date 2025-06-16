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
    /// Oyuncu giriþ yaptýktan sonra local ve PlayFab skorlarýný bir kere senkronize eder.
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
                    // Yereldeki skor daha yüksekse PlayFab'a gönder
                    SendHighScore(localScore);
                }
                else if (serverScore > localScore)
                {
                    // PlayFab skoru daha yüksekse yereli güncelle
                    PlayerPrefs.SetInt("HighScore", serverScore);
                    Debug.Log("Yerel skor güncellendi: " + serverScore);
                }
                else
                {
                    Debug.Log("Skorlar zaten eþit.");
                }
            },
            error =>
            {
                Debug.LogError("Skor çekme hatasý: " + error.GenerateErrorReport());
            });
    }

    /// <summary>
    /// Yeni skor PlayFab'a gönderilir. Oyun içinde sürekli çaðrýlmamalý.
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
            result => Debug.Log("Yeni yüksek skor PlayFab'a gönderildi: " + score),
            error => Debug.LogError("Skor gönderme hatasý: " + error.GenerateErrorReport()));
    }
}