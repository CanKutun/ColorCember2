using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public int skor = 1234; // oyun sonunda gelen skor

    void Start()
    {
        // Skoru ve �lkeyi otomatik g�nder
        PlayFabManager.Instance.SendScore(skor);
    }
}