using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    [Header("Session Configuration")]
    public int maxPlayers = 16;
    public string gameMode = "RockRun";

    public Lobby CurrentLobby { get; private set; }
    public Allocation CurrentCreateAllocation { get; private set; }
    public JoinAllocation CurrentJoinAllocation { get; private set; }

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

    public async Task<bool> CreateLobby(string lobbyName)
    {
        try
        {
            CurrentCreateAllocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(CurrentCreateAllocation.AllocationId);

            var lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "gameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) },
                    { "relayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) },
                    { "version", new DataObject(DataObject.VisibilityOptions.Public, Application.version) }
                }
            };

            CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            Debug.Log($"Lobby created: {CurrentLobby.Name} (ID: {CurrentLobby.Id})");
            Debug.Log($"Relay join code: {joinCode}");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create lobby: {e.Message}");
            return false;
        }
    }

    public async Task<bool> JoinLobbyById(string lobbyId)
    {
        try
        {
            CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            if (CurrentLobby.Data.TryGetValue("relayJoinCode", out var relayCodeData))
            {
                var joinCode = relayCodeData.Value;
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                CurrentJoinAllocation = joinAllocation;

                Debug.Log($"Joined lobby: {CurrentLobby.Name}");
                Debug.Log($"Connected to relay");
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
            return false;
        }
    }

    public async Task<bool> JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            if (CurrentLobby.Data.TryGetValue("relayJoinCode", out var relayCodeData))
            {
                var joinCode = relayCodeData.Value;
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                CurrentJoinAllocation = joinAllocation;

                Debug.Log($"Joined lobby: {CurrentLobby.Name} via code");
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join lobby by code: {e.Message}");
            return false;
        }
    }

    public async void LeaveLobby()
    {
        if (CurrentLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, UGSManager.Instance.PlayerId);
                CurrentLobby = null;
                CurrentCreateAllocation = null;
                CurrentJoinAllocation = null;
                Debug.Log("Left lobby");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to leave lobby: {e.Message}");
            }
        }
    }
}