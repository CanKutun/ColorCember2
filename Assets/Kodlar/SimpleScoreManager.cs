using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class SimpleScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private int currentScore = 0;

    void Start()
    {
        // Baþlangýçta yüksek skoru yükle ve göster
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI(savedHighScore);
    }

    public void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = "Skor: " + currentScore;

        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > savedHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            highScoreText.text = "En Yüksek: " + currentScore;

            // PlayFab'a skor gönder
            SendHighScoreToPlayFab(currentScore);
        }
    }

    private void UpdateUI(int highScore)
    {
        scoreText.text = "Skor: " + currentScore;
        highScoreText.text = "En Yüksek: " + highScore;
    }

    void SendHighScoreToPlayFab(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new System.Collections.Generic.List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log("Skor PlayFab'a gönderildi: " + score),
            error => Debug.LogError("PlayFab Skor Gönderme Hatasý: " + error.GenerateErrorReport()));
    }
}