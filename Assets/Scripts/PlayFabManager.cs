using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Login();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Login()
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

        // �lke bilgisi �ek
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, null);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Giri� hatas�: " + error.GenerateErrorReport());
    }

    void OnGetUserDataSuccess(GetUserDataResult result)
    {
        if (!result.Data.ContainsKey("Country"))
        {
            string country = Application.systemLanguage.ToString(); // basit yakla��m
            SetCountry(country);
        }
    }

    public void SetCountry(string country)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Country", country}
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result => {
            Debug.Log("�lke bilgisi g�nderildi: " + country);
        }, OnLoginFailure);
    }

    public void SendScore(int score)
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

        PlayFabClientAPI.UpdatePlayerStatistics(request, result => {
            Debug.Log("Skor g�nderildi: " + score);
        }, OnLoginFailure);
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, result => {
            foreach (var entry in result.Leaderboard)
            {
                Debug.Log($"{entry.Position + 1}. {entry.DisplayName ?? entry.PlayFabId} - {entry.StatValue}");
            }
        }, OnLoginFailure);
    }
}