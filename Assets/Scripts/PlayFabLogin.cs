using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLogin : MonoBehaviour
{
    public static PlayFabLogin Instance { get; private set; }
    public string PlayFabId { get; private set; }

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

    void Start()
    {
        LoginWithCustomID();
    }

    void LoginWithCustomID()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Giri� ba�ar�l�");
        PlayFabId = result.PlayFabId;

        // Giri� ba�ar�l� olunca leaderboard'u �ek
        LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
        if (leaderboardManager != null)
        {
            leaderboardManager.GetLeaderboard();
        }

        // �ste�e ba�l�: giri�te PlayFab'den skor �ekilip yerel olarak kaydedilebilir
        GetPlayerHighScoreFromPlayFab();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Giri� ba�ar�s�z: " + error.GenerateErrorReport());
    }

    /// <summary>
    /// Yeni skor eski y�ksek skordan b�y�kse PlayFab'e g�nderir.
    /// </summary>
    public void SendHighScoreIfNew(int newScore)
    {
        int previousScore = PlayerPrefs.GetInt("HighScore", 0);

        if (newScore > previousScore)
        {
            PlayerPrefs.SetInt("HighScore", newScore);

            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Skor",
                        Value = newScore
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request,
                result => Debug.Log("Yeni y�ksek skor PlayFab'e g�nderildi: " + newScore),
                error => Debug.LogError("Skor g�nderme hatas�: " + error.GenerateErrorReport()));
        }
        else
        {
            Debug.Log("Skor daha �nce g�nderilmi� y�ksek skordan d���k, g�nderilmedi.");
        }
    }

    /// <summary>
    /// Giri�te PlayFab'deki skoru �ekip yerel PlayerPrefs'e kaydeder (cihaz de�i�imi i�in faydal�).
    /// </summary>
    private void GetPlayerHighScoreFromPlayFab()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result =>
            {
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == "Skor")
                    {
                        int serverScore = stat.Value;
                        int localScore = PlayerPrefs.GetInt("HighScore", 0);

                        if (serverScore > localScore)
                        {
                            PlayerPrefs.SetInt("HighScore", serverScore);
                            Debug.Log("Sunucudaki y�ksek skor yerel belle�e kaydedildi: " + serverScore);
                        }
                    }
                }
            },
            error =>
            {
                Debug.LogWarning("Skor �ekme ba�ar�s�z: " + error.GenerateErrorReport());
            });
    }
}
