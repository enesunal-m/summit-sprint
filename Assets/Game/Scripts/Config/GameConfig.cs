using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Summit Sprint/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Environment")]
    public bool isDevelopment = true;
    public string environment = "Development";
    
    [Header("UGS Configuration")]
    public string projectId = "";
    public string developmentEnvironmentId = "";
    public string productionEnvironmentId = "";
    
    [Header("Networking")]
    public int maxPlayers = 16;
    public float networkTickRate = 60f;
    public int snapshotRate = 20;
    
    [Header("Debug Settings")]
    public bool enableNetworkLogging = true;
    public bool enableAnalyticsLogging = false;
    public bool showDebugUI = true;
    
    [Header("Build Settings")]
    public string buildVersion = "0.1.0";
    public int buildNumber = 1;
}