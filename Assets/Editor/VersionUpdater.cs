
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class VersionUpdater : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string versionFilePath = "version.txt";

        if (!File.Exists(versionFilePath))
        {
            UnityEngine.Debug.LogWarning("[VersionUpdater] version.txt bulunamadı, sürüm güncellenmedi.");
            return;
        }

        string versionText = File.ReadAllText(versionFilePath).Trim();
        if (string.IsNullOrEmpty(versionText))
        {
            UnityEngine.Debug.LogWarning("[VersionUpdater] version.txt boş, sürüm güncellenmedi.");
            return;
        }

        PlayerSettings.bundleVersion = versionText;

        string[] parts = versionText.Split('.');
        int major = parts.Length > 0 ? int.Parse(parts[0]) : 0;
        int minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
        int patch = parts.Length > 2 ? int.Parse(parts[2]) : 0;

        int versionCode = major * 10000 + minor * 100 + patch;
        PlayerSettings.Android.bundleVersionCode = versionCode;

        UnityEngine.Debug.Log($"[VersionUpdater] Versiyon: {versionText} — VersionCode: {versionCode}");
    }
}
