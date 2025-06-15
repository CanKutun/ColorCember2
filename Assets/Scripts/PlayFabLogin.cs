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
        Debug.Log("Giriþ baþarýlý");
        PlayFabId = result.PlayFabId;

        // Giriþ baþarýlý olunca leaderboard'u çek
        LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
        if (leaderboardManager != null)
        {
            leaderboardManager.GetLeaderboard();
        }

        // Ýsteðe baðlý: giriþte PlayFab'den skor çekilip yerel olarak kaydedilebilir
        GetPlayerHighScoreFromPlayFab();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Giriþ baþarýsýz: " + error.GenerateErrorReport());
    }

    /// <summary>
    /// Yeni skor eski yüksek skordan büyükse PlayFab'e gönderir.
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
                result => Debug.Log("Yeni yüksek skor PlayFab'e gönderildi: " + newScore),
                error => Debug.LogError("Skor gönderme hatasý: " + error.GenerateErrorReport()));
        }
        else
        {
            Debug.Log("Skor daha önce gönderilmiþ yüksek skordan düþük, gönderilmedi.");
        }
    }

    /// <summary>
    /// Giriþte PlayFab'deki skoru çekip yerel PlayerPrefs'e kaydeder (cihaz deðiþimi için faydalý).
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
                            Debug.Log("Sunucudaki yüksek skor yerel belleðe kaydedildi: " + serverScore);
                        }
                    }
                }
            },
            error =>
            {
                Debug.LogWarning("Skor çekme baþarýsýz: " + error.GenerateErrorReport());
            });
    }
}
