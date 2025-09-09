using UnityEngine;

/// <summary>
/// This class demonstrates the coding standards used throughout the SummitSprint project.
/// Use this as a reference for maintaining consistent code style across all team members.
/// </summary>
public class CodingStandards : MonoBehaviour
{
    #region Public Properties

    // Use PascalCase for public members
    public int PlayerCount { get; set; }
    public string SessionId { get; set; }
    public bool IsGameActive { get; set; }

    #endregion

    #region Private Fields

    // Use camelCase for private fields with descriptive names
    [Header("Network Settings")]
    public float connectionTimeout = 5f;
    public int maxPlayersPerLobby = 4;
    public bool enableDebugMode = false;

    [Header("Performance Settings")]
    public float networkTickRate = 60f;
    public int maxConcurrentLobbies = 100;

    // Use descriptive names - Good examples
    private bool isPlayerAuthenticated;
    private float lastNetworkPingTime;
    private int activeConnectionCount;

    // Avoid abbreviations - Bad examples (commented out)
    // private bool auth; // Bad - unclear meaning
    // private float ping; // Bad - too generic
    // private int connCount; // Bad - abbreviated

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeComponent();
    }

    private void Start()
    {
        SetupNetworking();
    }

    private void Update()
    {
        if (enableDebugMode)
        {
            UpdateDebugInfo();
        }
    }

    private void OnDestroy()
    {
        CleanupResources();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts a new game session with the specified parameters.
    /// </summary>
    /// <param name="maxPlayers">Maximum number of players allowed</param>
    /// <param name="gameMode">The game mode to use</param>
    /// <returns>True if the game started successfully</returns>
    public bool StartGame(int maxPlayers, string gameMode)
    {
        if (maxPlayers <= 0 || string.IsNullOrEmpty(gameMode))
        {
            Debug.LogError("Invalid game parameters provided");
            return false;
        }

        return InitializeGameSession(maxPlayers, gameMode);
    }

    /// <summary>
    /// Stops the current game session and cleans up resources.
    /// </summary>
    public void StopGame()
    {
        if (!IsGameActive)
        {
            Debug.LogWarning("No active game session to stop");
            return;
        }

        CleanupGameSession();
        IsGameActive = false;

        Debug.Log("Game session stopped successfully");
    }

    #endregion

    #region Private Methods

    private void InitializeComponent()
    {
        // Initialize component-specific settings
        PlayerCount = 0;
        isPlayerAuthenticated = false;
        lastNetworkPingTime = Time.time;

        Debug.Log($"CodingStandards component initialized with timeout: {connectionTimeout}s");
    }

    private void SetupNetworking()
    {
        // Configure network settings based on current values
        if (networkTickRate < 10f)
        {
            Debug.LogWarning("Network tick rate is very low, performance may be affected");
            networkTickRate = 10f;
        }

        // Use descriptive method names
        EstablishNetworkConnection();
        ConfigureNetworkParameters();
    }

    private bool InitializeGameSession(int maxPlayers, string gameMode)
    {
        try
        {
            PlayerCount = 0;
            IsGameActive = true;

            // Simulate game initialization
            Debug.Log($"Starting {gameMode} game mode for up to {maxPlayers} players");

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to initialize game session: {ex.Message}");
            return false;
        }
    }

    private void CleanupGameSession()
    {
        // Perform cleanup operations
        PlayerCount = 0;
        isPlayerAuthenticated = false;

        // Reset network state
        lastNetworkPingTime = 0f;
        activeConnectionCount = 0;
    }

    private void EstablishNetworkConnection()
    {
        // Network connection logic would go here
        activeConnectionCount = 1;
        Debug.Log("Network connection established");
    }

    private void ConfigureNetworkParameters()
    {
        // Configure based on current settings
        Debug.Log($"Network configured: Tick Rate = {networkTickRate}Hz, Timeout = {connectionTimeout}s");
    }

    private void UpdateDebugInfo()
    {
        // Update debug information periodically
        if (Time.time - lastNetworkPingTime > 1f)
        {
            lastNetworkPingTime = Time.time;
            // Debug info update logic
        }
    }

    private void CleanupResources()
    {
        // Cleanup any resources used by this component
        Debug.Log("CodingStandards component cleaned up");
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles player connection events.
    /// Use this pattern for event handlers.
    /// </summary>
    /// <param name="playerId">The ID of the connecting player</param>
    private void OnPlayerConnected(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Invalid player ID received");
            return;
        }

        PlayerCount++;
        Debug.Log($"Player {playerId} connected. Total players: {PlayerCount}");
    }

    /// <summary>
    /// Handles player disconnection events.
    /// </summary>
    /// <param name="playerId">The ID of the disconnecting player</param>
    private void OnPlayerDisconnected(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Invalid player ID received");
            return;
        }

        PlayerCount = Mathf.Max(0, PlayerCount - 1);
        Debug.Log($"Player {playerId} disconnected. Total players: {PlayerCount}");
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Validates if the current game state is valid.
    /// Use this pattern for validation methods.
    /// </summary>
    /// <returns>True if the game state is valid</returns>
    private bool ValidateGameState()
    {
        if (PlayerCount < 0)
        {
            Debug.LogError("Invalid player count detected");
            return false;
        }

        if (networkTickRate <= 0)
        {
            Debug.LogError("Invalid network tick rate");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a formatted status string for debugging.
    /// </summary>
    /// <returns>Formatted status string</returns>
    public string GetStatusString()
    {
        return $"Players: {PlayerCount}, Active: {IsGameActive}, Authenticated: {isPlayerAuthenticated}";
    }

    #endregion
}
