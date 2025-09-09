using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameConfig gameConfig;
    
    public static EnvironmentManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load appropriate config based on build
            LoadEnvironmentConfig();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadEnvironmentConfig()
    {
#if DEVELOPMENT_BUILD
            gameConfig = Resources.Load<GameConfig>("GameConfig_Development");
#else
        gameConfig = Resources.Load<GameConfig>("GameConfig_Production");
#endif
        
        if (gameConfig == null)
        {
            Debug.LogError("No game configuration found! Creating default config.");
            gameConfig = ScriptableObject.CreateInstance<GameConfig>();
        }
        
        ApplyConfiguration();
    }
    
    private void ApplyConfiguration()
    {
        // Apply debug settings
        Debug.unityLogger.logEnabled = gameConfig.enableNetworkLogging;
        
        // Set application target frame rate based on environment
        Application.targetFrameRate = gameConfig.isDevelopment ? -1 : 60;
        
        Debug.Log($"Environment loaded: {gameConfig.environment}");
    }
}
