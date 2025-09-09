using UnityEngine;
using UnityEditor;

public class DeveloperTools
{
    [MenuItem("Summit Sprint/Quick Setup/Create Player")]
    public static void CreatePlayerObject()
    {
        var playerGO = new GameObject("Player");
        playerGO.AddComponent<Rigidbody>();
        playerGO.AddComponent<SphereCollider>();
        // Will add RockController later
        
        Selection.activeGameObject = playerGO;
        SceneView.FrameLastActiveSceneView();
    }
    
    [MenuItem("Summit Sprint/Quick Setup/Create Track Spline")]
    public static void CreateTrackSpline()
    {
        var trackGO = new GameObject("Track");
        // Will add spline component later
        
        Selection.activeGameObject = trackGO;
        SceneView.FrameLastActiveSceneView();
    }
    
    [MenuItem("Summit Sprint/Debug/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player preferences cleared");
    }
    
    [MenuItem("Summit Sprint/Debug/Show Build Info")]
    public static void ShowBuildInfo()
    {
        Debug.Log($"Unity Version: {Application.unityVersion}");
        Debug.Log($"Platform: {Application.platform}");
        Debug.Log($"Build GUID: {Application.buildGUID}");
        Debug.Log($"Version: {Application.version}");
    }
    
    [MenuItem("Summit Sprint/Networking/Test UGS Connection")]
    public static void TestUGSConnection()
    {
        if (UGSManager.Instance != null)
        {
            Debug.Log($"UGS Initialized: {UGSManager.Instance.IsInitialized}");
            Debug.Log($"UGS Authenticated: {UGSManager.Instance.IsAuthenticated}");
        }
        else
        {
            Debug.LogError("UGSManager not found in scene");
        }
    }
}
