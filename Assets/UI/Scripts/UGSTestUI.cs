using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UGSTestUI : MonoBehaviour
{
    [Header("UI References")]
    public Button createLobbyButton;
    public Button refreshLobbiesButton;
    public TMP_InputField lobbyNameInput;
    public TMP_InputField lobbyCodeInput;
    public Button joinByCodeButton;
    public TextMeshProUGUI statusText;
    public Transform lobbyListParent;
    public GameObject lobbyItemPrefab;
    
    [Header("Button State Management")]
    private bool isCreatingLobby = false;
    private bool isRefreshingLobbies = false;
    private bool isJoiningLobby = false;
    
    private static UGSTestUI instance;
    
    private void Awake()
    {
        // Prevent duplicate components
        if (instance != null)
        {
            Debug.LogWarning("Multiple UGSTestUI instances found - destroying duplicate");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    private void Start()
    {
        // Ensure buttons are only subscribed once
        if (createLobbyButton != null)
        {
            createLobbyButton.onClick.RemoveAllListeners();
            createLobbyButton.onClick.AddListener(CreateLobby);
        }
        
        if (refreshLobbiesButton != null)
        {
            refreshLobbiesButton.onClick.RemoveAllListeners();
            refreshLobbiesButton.onClick.AddListener(RefreshLobbies);
        }
        
        if (joinByCodeButton != null)
        {
            joinByCodeButton.onClick.RemoveAllListeners();
            joinByCodeButton.onClick.AddListener(JoinByCode);
        }
        
        // Subscribe to lobby updates
        if (LobbyBrowser.Instance != null)
        {
            LobbyBrowser.Instance.OnLobbiesUpdated -= UpdateLobbyList; // Remove first to prevent duplicates
            LobbyBrowser.Instance.OnLobbiesUpdated += UpdateLobbyList;
        }
        
        UpdateStatus("Ready to test UGS services");
        UpdateButtonStates();
    }
    
    private void OnDestroy()
    {
        // Clean up subscriptions
        if (LobbyBrowser.Instance != null)
        {
            LobbyBrowser.Instance.OnLobbiesUpdated -= UpdateLobbyList;
        }
        
        if (instance == this)
        {
            instance = null;
        }
    }
    
    private async void CreateLobby()
    {
        if (isCreatingLobby)
        {
            Debug.LogWarning("Lobby creation already in progress - ignoring duplicate call");
            return;
        }
        
        isCreatingLobby = true;
        UpdateButtonStates();
        
        string lobbyName = string.IsNullOrEmpty(lobbyNameInput.text) ? "Test Lobby" : lobbyNameInput.text;
        UpdateStatus("Creating lobby...");
        
        try
        {
            bool success = await SessionManager.Instance.CreateLobby(lobbyName);
            UpdateStatus(success ? "Lobby created successfully!" : "Failed to create lobby");
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Error creating lobby: {e.Message}");
            Debug.LogError($"Lobby creation error: {e}");
        }
        finally
        {
            isCreatingLobby = false;
            UpdateButtonStates();
        }
    }
    
    private async void RefreshLobbies()
    {
        if (isRefreshingLobbies)
        {
            Debug.LogWarning("Lobby refresh already in progress - ignoring duplicate call");
            return;
        }
        
        if (LobbyBrowser.Instance == null)
        {
            UpdateStatus("LobbyBrowser not available");
            return;
        }
        
        isRefreshingLobbies = true;
        UpdateButtonStates();
        UpdateStatus("Refreshing lobbies...");
        
        try
        {
            await LobbyBrowser.Instance.RefreshLobbiesAsync();
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Error refreshing lobbies: {e.Message}");
            Debug.LogError($"Lobby refresh error: {e}");
        }
        finally
        {
            isRefreshingLobbies = false;
            UpdateButtonStates();
        }
    }
    
    private async void JoinByCode()
    {
        if (isJoiningLobby)
        {
            Debug.LogWarning("Join lobby already in progress - ignoring duplicate call");
            return;
        }
        
        string code = lobbyCodeInput.text;
        if (string.IsNullOrEmpty(code))
        {
            UpdateStatus("Please enter a lobby code");
            return;
        }
        
        isJoiningLobby = true;
        UpdateButtonStates();
        UpdateStatus("Joining lobby...");
        
        try
        {
            bool success = await SessionManager.Instance.JoinLobbyByCode(code);
            UpdateStatus(success ? "Joined lobby successfully!" : "Failed to join lobby");
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Error joining lobby: {e.Message}");
            Debug.LogError($"Join lobby error: {e}");
        }
        finally
        {
            isJoiningLobby = false;
            UpdateButtonStates();
        }
    }
    
    private void UpdateButtonStates()
    {
        if (createLobbyButton != null)
        {
            createLobbyButton.interactable = !isCreatingLobby && UGSManager.Instance != null && UGSManager.Instance.IsAuthenticated;
        }
        
        if (refreshLobbiesButton != null)
        {
            refreshLobbiesButton.interactable = !isRefreshingLobbies && UGSManager.Instance != null && UGSManager.Instance.IsAuthenticated;
        }
        
        if (joinByCodeButton != null)
        {
            joinByCodeButton.interactable = !isJoiningLobby && UGSManager.Instance != null && UGSManager.Instance.IsAuthenticated;
        }
    }
    
    private void UpdateLobbyList(System.Collections.Generic.List<Unity.Services.Lobbies.Models.Lobby> lobbies)
    {
        if (lobbyListParent == null) return;
        
        // Clear existing lobby items
        foreach (Transform child in lobbyListParent)
        {
            Destroy(child.gameObject);
        }
        
        // Create new lobby items
        foreach (var lobby in lobbies)
        {
            if (lobbyItemPrefab == null) continue;
            
            var lobbyItem = Instantiate(lobbyItemPrefab, lobbyListParent);
            var lobbyText = lobbyItem.GetComponentInChildren<TextMeshProUGUI>();
            if (lobbyText != null)
            {
                lobbyText.text = $"{lobby.Name} ({lobby.Players.Count}/{lobby.MaxPlayers})";
            }
            
            var joinButton = lobbyItem.GetComponentInChildren<Button>();
            if (joinButton != null)
            {
                string lobbyId = lobby.Id; // Capture for closure
                joinButton.onClick.RemoveAllListeners();
                joinButton.onClick.AddListener(() => JoinLobby(lobbyId));
            }
        }
        
        UpdateStatus($"Found {lobbies.Count} lobbies");
    }
    
    private async void JoinLobby(string lobbyId)
    {
        if (isJoiningLobby) return;
        
        isJoiningLobby = true;
        UpdateButtonStates();
        UpdateStatus("Joining lobby...");
        
        try
        {
            bool success = await SessionManager.Instance.JoinLobbyById(lobbyId);
            UpdateStatus(success ? "Joined lobby!" : "Failed to join lobby");
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Error joining lobby: {e.Message}");
            Debug.LogError($"Join lobby error: {e}");
        }
        finally
        {
            isJoiningLobby = false;
            UpdateButtonStates();
        }
    }
    
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
        }
        Debug.Log($"UI Status: {message}");
    }
}