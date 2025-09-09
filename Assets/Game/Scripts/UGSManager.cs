using System;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using UnityEngine;

public class UGSManager : MonoBehaviour
{
    public static UGSManager Instance { get; private set; }
    
    [Header("UGS Configuration")]
    public bool autoInitialize = true;
    public bool enableDebugLogging = true;
    
    public bool IsInitialized { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public string PlayerId => AuthenticationService.Instance.PlayerId;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (autoInitialize)
            {
                InitializeUGS();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public async void InitializeUGS()
    {
        try
        {
            if (enableDebugLogging)
                Debug.Log("Initializing Unity Gaming Services...");
            
            // Initialize UGS with default configuration
            await UnityServices.InitializeAsync();
            IsInitialized = true;
            
            if (enableDebugLogging)
                Debug.Log("UGS Initialized successfully");
            
            await AuthenticateAnonymously();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize UGS: {e.Message}");
            await InitializeWithRetry();
        }
    }
    
    private async System.Threading.Tasks.Task AuthenticateAnonymously()
    {
        try
        {
            if (enableDebugLogging)
                Debug.Log("Authenticating anonymously...");
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            IsAuthenticated = true;
            
            if (enableDebugLogging)
                Debug.Log($"Authenticated with Player ID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Authentication failed: {e.Message}");
            throw;
        }
    }
    
    private async System.Threading.Tasks.Task InitializeWithRetry(int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await UnityServices.InitializeAsync();
                IsInitialized = true;
                await AuthenticateAnonymously();
                return;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"UGS initialization attempt {attempt} failed: {e.Message}");
                
                if (attempt == maxRetries)
                {
                    Debug.LogError("Failed to initialize UGS after maximum retries");
                    throw;
                }
                
                // Exponential backoff delay
                await System.Threading.Tasks.Task.Delay(1000 * attempt);
            }
        }
    }

    [Header("Testing")]
    public bool testServicesOnStart = false;

    private async void Start()
    {
        if (testServicesOnStart && IsInitialized && IsAuthenticated)
        {
            await TestAllServices();
        }
    }

    public async System.Threading.Tasks.Task TestAllServices()
    {
        Debug.Log("Testing UGS services...");
        
        // Test Relay Service
        await TestRelayService();
        
        // Test Lobby Service
        await TestLobbyService();
    }

    private async System.Threading.Tasks.Task TestRelayService()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(1);
            Debug.Log($"✅ Relay test successful - Allocation ID: {allocation.AllocationId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Relay test failed: {e.Message}");
        }
    }

    private async System.Threading.Tasks.Task TestLobbyService()
    {
        try
        {
            var queryOptions = new QueryLobbiesOptions
            {
                Count = 5
            };
            var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            Debug.Log($"✅ Lobby test successful - Found {response.Results.Count} lobbies");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Lobby test failed: {e.Message}");
        }
    }
}

