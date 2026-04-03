using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class NameManager : MonoBehaviour
{
    public GameObject namePanel;
    public TMP_InputField nameInput;

    void Start()
    {
        if (PlayerPrefs.HasKey("displayNameSet"))
        {
            namePanel.SetActive(false); // Ýsim zaten ayarlandýysa paneli kapat
        }
        else
        {
            namePanel.SetActive(true);  // Ýlk kez giriyorsa paneli aç
        }
    }

    public void OnConfirmButtonClicked()
    {
        string playerName = nameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = playerName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdated, OnDisplayNameError);
        }
    }

    void OnDisplayNameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated: " + result.DisplayName);
        PlayerPrefs.SetInt("displayNameSet", 1); // Kaydetti, bir daha sorma
        namePanel.SetActive(false); // Paneli gizle
    }

    void OnDisplayNameError(PlayFabError error)
    {
        Debug.LogError("Display name update failed: " + error.GenerateErrorReport());
    }
}