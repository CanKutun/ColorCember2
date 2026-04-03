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
    result => {
        Debug.Log("Yeni yüksek skor PlayFab'a gönderildi: " + score);
        // Liderlik tablosunu yenile (örnek olarak)
        LeaderboardManager.Instance?.RefreshLeaderboard();
    },
    error => Debug.LogError("Skor gönderme hatasý: " + error.GenerateErrorReport()));
    }
}