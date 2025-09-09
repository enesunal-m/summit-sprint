using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LobbyBrowser : MonoBehaviour
{
    public static LobbyBrowser Instance { get; private set; }

    [Header("Browser Settings")]
    public int maxResults = 25;
    public float autoRefreshInterval = 15f; // Increased from 5f to 15f
    public bool autoRefresh = false; // Disabled by default to prevent rate limiting

    [Header("Rate Limiting")]
    public int maxRequestsPerMinute = 30; // UGS limit is typically 30-60 per minute
    public float rateLimitCooldown = 60f; // Cooldown after hitting rate limit

    private List<Lobby> availableLobbies = new List<Lobby>();
    private float lastRefresh;
    private Queue<float> requestTimes = new Queue<float>();
    private bool isRateLimited = false;
    private float rateLimitEndTime;

    public System.Action<List<Lobby>> OnLobbiesUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Check if rate limit cooldown has expired
        if (isRateLimited && Time.time > rateLimitEndTime)
        {
            isRateLimited = false;
            Debug.Log("Rate limit cooldown expired - API calls resumed");
        }

        // Auto refresh with rate limiting check
        if (autoRefresh && !isRateLimited && Time.time - lastRefresh > autoRefreshInterval)
        {
            if (CanMakeRequest())
            {
                RefreshLobbies();
            }
            else
            {
                Debug.LogWarning("Skipping lobby refresh - approaching rate limit");
            }
        }
    }

    private bool CanMakeRequest()
    {
        // Remove requests older than 1 minute
        float oneMinuteAgo = Time.time - 60f;
        while (requestTimes.Count > 0 && requestTimes.Peek() < oneMinuteAgo)
        {
            requestTimes.Dequeue();
        }

        // Check if we're under the rate limit
        return requestTimes.Count < maxRequestsPerMinute && !isRateLimited;
    }

    private void RecordRequest()
    {
        requestTimes.Enqueue(Time.time);
    }

    public async void RefreshLobbies()
    {
        if (isRateLimited)
        {
            Debug.LogWarning("Cannot refresh lobbies - rate limited");
            return;
        }

        if (!CanMakeRequest())
        {
            Debug.LogWarning("Cannot refresh lobbies - rate limit would be exceeded");
            return;
        }

        await RefreshLobbiesAsync();
    }

    public async Task RefreshLobbiesAsync()
    {
        try
        {
            RecordRequest();

            var queryOptions = new QueryLobbiesOptions
            {
                Count = maxResults,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            availableLobbies = response.Results;
            lastRefresh = Time.time;

            Debug.Log($"Found {availableLobbies.Count} available lobbies");
            OnLobbiesUpdated?.Invoke(availableLobbies);
        }
        catch (Unity.Services.Lobbies.LobbyServiceException e)
        {
            if (e.Message.Contains("Rate limit") || e.Message.Contains("rate limit"))
            {
                HandleRateLimit();
            }
            else
            {
                Debug.LogError($"Failed to refresh lobbies: {e.Message}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to refresh lobbies: {e.Message}");
        }
    }

    private void HandleRateLimit()
    {
        isRateLimited = true;
        rateLimitEndTime = Time.time + rateLimitCooldown;

        Debug.LogWarning($"Rate limit hit - API calls suspended for {rateLimitCooldown} seconds");
        Debug.LogWarning($"Consider increasing autoRefreshInterval or disabling autoRefresh");

        // Clear request history to start fresh after cooldown
        requestTimes.Clear();
    }

    public List<Lobby> GetAvailableLobbies()
    {
        return new List<Lobby>(availableLobbies);
    }

    public void EnableAutoRefresh(bool enable)
    {
        autoRefresh = enable;
        Debug.Log($"Auto refresh {(enable ? "enabled" : "disabled")}");
    }

    public int GetRequestsInLastMinute()
    {
        float oneMinuteAgo = Time.time - 60f;
        while (requestTimes.Count > 0 && requestTimes.Peek() < oneMinuteAgo)
        {
            requestTimes.Dequeue();
        }
        return requestTimes.Count;
    }
}