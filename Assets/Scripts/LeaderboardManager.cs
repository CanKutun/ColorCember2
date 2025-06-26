using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    public int maxResultsCount = 10;
    public string statisticName = "HighScore";

    [Header("UI References")]
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContent;
    public Button nextPageButton;
    public Button prevPageButton;
    public TextMeshProUGUI pageNumberText;

    private Coroutine leaderboardRoutine;
    private int currentPage = 0;
    private Dictionary<string, GameObject> entryObjects = new Dictionary<string, GameObject>();
    public static LeaderboardManager Instance { get; private set; }

    private bool isInitialized = false;
    private bool isLeaderboardLoading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (!isInitialized)
        {
            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveAllListeners();
                nextPageButton.onClick.AddListener(OnNextPage);
            }

            if (prevPageButton != null)
            {
                prevPageButton.onClick.RemoveAllListeners();
                prevPageButton.onClick.AddListener(OnPrevPage);
            }

            isInitialized = true;
        }

        currentPage = 0;
        UpdatePageNumberText();
        StartLeaderboardRoutineOnce();
    }

    public void RefreshLeaderboard()
    {
        StartLeaderboardRoutineOnce();
    }

    private void StartLeaderboardRoutineOnce()
    {
        if (isLeaderboardLoading) return;

        if (leaderboardRoutine != null)
            StopCoroutine(leaderboardRoutine);

        leaderboardRoutine = StartCoroutine(LoadLeaderboardRoutine());
    }

    private IEnumerator LoadLeaderboardRoutine()
    {
        isLeaderboardLoading = true;

        yield return StartCoroutine(ClearLeaderboardRoutine());
        yield return StartCoroutine(GetLeaderboard());
        yield return StartCoroutine(RequestUserDataForEntries());

        isLeaderboardLoading = false;
    }

    private IEnumerator ClearLeaderboardRoutine()
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        yield return null; // bir frame bekleyerek tam temizlensin
        entryObjects.Clear();
    }

    public IEnumerator GetLeaderboard()
    {
        bool done = false;

        int startPosition = currentPage * maxResultsCount;

        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = startPosition,
            MaxResultsCount = maxResultsCount
        };

        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            foreach (var item in result.Leaderboard)
            {
                GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);

                var rt = entryObj.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localScale = Vector3.one;
                    rt.localPosition = Vector3.zero;
                    rt.anchoredPosition = Vector2.zero;
                }

                var entryScript = entryObj.GetComponent<LeaderboardEntry>();
                if (entryScript != null)
                {
                    entryScript.SetEntry(item.Position + 1, item.DisplayName ?? item.PlayFabId, item.StatValue, "");
                }

                entryObjects[item.PlayFabId] = entryObj;
            }

            prevPageButton.interactable = currentPage > 0;
            nextPageButton.interactable = result.Leaderboard.Count == maxResultsCount;

            done = true;
        },
        error =>
        {
            Debug.LogError("Liderlik tablosu alınamadı: " + error.GenerateErrorReport());
            done = true;
        });

        while (!done)
            yield return null;
    }

    private IEnumerator RequestUserDataForEntries()
    {
        foreach (var kvp in entryObjects)
        {
            string playFabId = kvp.Key;
            GameObject entryObj = kvp.Value;

            bool done = false;

            var request = new GetUserDataRequest
            {
                PlayFabId = playFabId,
                Keys = new List<string> { "Country" }
            };

            PlayFabClientAPI.GetUserData(request, result =>
            {
                string countryCode = "";
                if (result.Data != null && result.Data.ContainsKey("Country"))
                {
                    string countryName = result.Data["Country"].Value.ToLower();
                    countryCode = GetCountryCode(countryName);
                }

                var entryScript = entryObj.GetComponent<LeaderboardEntry>();
                if (entryScript != null)
                {
                    entryScript.SetFlag(countryCode);
                }

                done = true;
            },
            error =>
            {
                Debug.LogError($"UserData alınamadı: {playFabId} -> {error.GenerateErrorReport()}");
                done = true;
            });

            while (!done)
                yield return null;
        }
    }

    private void OnNextPage()
    {
        currentPage++;
        UpdatePageNumberText();
        StartLeaderboardRoutineOnce();
    }

    private void OnPrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageNumberText();
            StartLeaderboardRoutineOnce();
        }
    }

    private void UpdatePageNumberText()
    {
        if (pageNumberText != null)
            pageNumberText.text = $"PAGE {currentPage + 1}";
    }

    string GetCountryCode(string countryName)
    {
        var map = new Dictionary<string, string>()
        {
            {"afghanistan", "af"}, {"albania", "al"}, {"algeria", "dz"}, {"andorra", "ad"},
            {"angola", "ao"}, {"argentina", "ar"}, {"armenia", "am"}, {"australia", "au"},
            {"austria", "at"}, {"azerbaijan", "az"}, {"bahamas", "bs"}, {"bahrain", "bh"},
            {"bangladesh", "bd"}, {"belarus", "by"}, {"belgium", "be"}, {"belize", "bz"},
            {"benin", "bj"}, {"bhutan", "bt"}, {"bolivia", "bo"}, {"bosnia and herzegovina", "ba"},
            {"botswana", "bw"}, {"brazil", "br"}, {"brunei", "bn"}, {"bulgaria", "bg"},
            {"burkina faso", "bf"}, {"burundi", "bi"}, {"cambodia", "kh"}, {"cameroon", "cm"},
            {"canada", "ca"}, {"chad", "td"}, {"chile", "cl"}, {"china", "cn"}, {"colombia", "co"},
            {"comoros", "km"}, {"congo", "cg"}, {"costa rica", "cr"}, {"croatia", "hr"},
            {"cuba", "cu"}, {"cyprus", "cy"}, {"czech republic", "cz"}, {"denmark", "dk"},
            {"djibouti", "dj"}, {"dominica", "dm"}, {"dominican republic", "do"}, {"ecuador", "ec"},
            {"egypt", "eg"}, {"el salvador", "sv"}, {"estonia", "ee"}, {"eswatini", "sz"},
            {"ethiopia", "et"}, {"fiji", "fj"}, {"finland", "fi"}, {"france", "fr"},
            {"gabon", "ga"}, {"gambia", "gm"}, {"georgia", "ge"}, {"germany", "de"},
            {"ghana", "gh"}, {"greece", "gr"}, {"greenland", "gl"}, {"grenada", "gd"},
            {"guatemala", "gt"}, {"guinea", "gn"}, {"guyana", "gy"}, {"haiti", "ht"},
            {"honduras", "hn"}, {"hungary", "hu"}, {"iceland", "is"}, {"india", "in"},
            {"indonesia", "id"}, {"iran", "ir"}, {"iraq", "iq"}, {"ireland", "ie"},
            {"israel", "il"}, {"italy", "it"}, {"ivory coast", "ci"}, {"jamaica", "jm"},
            {"japan", "jp"}, {"jordan", "jo"}, {"kazakhstan", "kz"}, {"kenya", "ke"},
            {"kiribati", "ki"}, {"kuwait", "kw"}, {"kyrgyzstan", "kg"}, {"laos", "la"},
            {"latvia", "lv"}, {"lebanon", "lb"}, {"lesotho", "ls"}, {"liberia", "lr"},
            {"libya", "ly"}, {"liechtenstein", "li"}, {"lithuania", "lt"}, {"luxembourg", "lu"},
            {"madagascar", "mg"}, {"malawi", "mw"}, {"malaysia", "my"}, {"maldives", "mv"},
            {"mali", "ml"}, {"malta", "mt"}, {"mauritania", "mr"}, {"mauritius", "mu"},
            {"mexico", "mx"}, {"moldova", "md"}, {"monaco", "mc"}, {"mongolia", "mn"},
            {"montenegro", "me"}, {"morocco", "ma"}, {"mozambique", "mz"}, {"myanmar", "mm"},
            {"namibia", "na"}, {"nepal", "np"}, {"netherlands", "nl"}, {"new zealand", "nz"},
            {"nicaragua", "ni"}, {"niger", "ne"}, {"nigeria", "ng"}, {"north korea", "kp"},
            {"north macedonia", "mk"}, {"norway", "no"}, {"oman", "om"}, {"pakistan", "pk"},
            {"palestine", "ps"}, {"panama", "pa"}, {"papua new guinea", "pg"}, {"paraguay", "py"},
            {"peru", "pe"}, {"philippines", "ph"}, {"poland", "pl"}, {"portugal", "pt"},
            {"qatar", "qa"}, {"romania", "ro"}, {"russia", "ru"}, {"rwanda", "rw"},
            {"saint lucia", "lc"}, {"samoa", "ws"}, {"san marino", "sm"}, {"sao tome and principe", "st"},
            {"saudi arabia", "sa"}, {"senegal", "sn"}, {"serbia", "rs"}, {"seychelles", "sc"},
            {"sierra leone", "sl"}, {"singapore", "sg"}, {"slovakia", "sk"}, {"slovenia", "si"},
            {"somalia", "so"}, {"south africa", "za"}, {"south korea", "kr"}, {"south sudan", "ss"},
            {"spain", "es"}, {"sri lanka", "lk"}, {"sudan", "sd"}, {"suriname", "sr"},
            {"sweden", "se"}, {"switzerland", "ch"}, {"syria", "sy"}, {"taiwan", "tw"},
            {"tajikistan", "tj"}, {"tanzania", "tz"}, {"thailand", "th"}, {"togo", "tg"},
            {"tonga", "to"}, {"trinidad and tobago", "tt"}, {"tunisia", "tn"}, {"turkey", "tr"},
            {"turkmenistan", "tm"}, {"uganda", "ug"}, {"ukraine", "ua"}, {"united arab emirates", "ae"},
            {"united kingdom", "gb"}, {"united states", "us"}, {"uruguay", "uy"}, {"uzbekistan", "uz"},
            {"vanuatu", "vu"}, {"venezuela", "ve"}, {"vietnam", "vn"}, {"yemen", "ye"},
            {"zambia", "zm"}, {"zimbabwe", "zw"}
        };

        if (map.TryGetValue(countryName, out string code))
            return code;
        return "";
    }
}
