using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUIPanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI networkStatusText;
    public TextMeshProUGUI playerCountText;
    public Button toggleButton;
    public GameObject debugPanel;
    
    [Header("Settings")]
    public bool showOnStart = true;
    public float updateInterval = 0.5f;
    
    private float frameCount = 0;
    private float deltaTime = 0;
    private float lastUpdate = 0;
    
    private void Start()
    {
        if (debugPanel != null)
            debugPanel.SetActive(showOnStart);
        
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleDebugPanel);
        
        // Only show in development builds
        #if !DEVELOPMENT_BUILD
            gameObject.SetActive(false);
        #endif
    }
    
    private void Update()
    {
        // Calculate FPS
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;
        
        if (Time.time - lastUpdate > updateInterval)
        {
            UpdateDebugInfo();
            lastUpdate = Time.time;
        }
    }
    
    private void UpdateDebugInfo()
    {
        // FPS calculation
        float fps = frameCount / deltaTime;
        if (fpsText != null)
            fpsText.text = $"FPS: {fps:F1}";
        
        // Network status
        if (networkStatusText != null)
        {
            bool isConnected = UGSManager.Instance?.IsAuthenticated ?? false;
            networkStatusText.text = $"Network: {(isConnected ? "Connected" : "Disconnected")}";
        }
        
        // Player count (will be updated when multiplayer is implemented)
        if (playerCountText != null)
            playerCountText.text = "Players: 1/16";
        
        // Reset counters
        frameCount = 0;
        deltaTime = 0;
    }
    
    public void ToggleDebugPanel()
    {
        if (debugPanel != null)
            debugPanel.SetActive(!debugPanel.activeSelf);
    }
}