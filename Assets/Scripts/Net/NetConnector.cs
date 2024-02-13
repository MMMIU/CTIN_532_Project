using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Services.Core;
using Events;
using Managers;

public class NetConnector : MonoBehaviour
{
    public int connectedPlayers = 0;
    // network
    public bool useInternet = false;
    public bool acceptIncomingConnections = true;

    private static NetConnector instance;
    public static NetConnector Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        NetworkManager.Singleton.OnClientConnectedCallback += id =>
        {
            if (!acceptIncomingConnections)
            {
                Debug.LogWarning("Incoming connection refused, id = " + id);
                NetworkManager.Singleton.DisconnectClient(id);
                return;
            }
            Debug.Log("OnClientConnectedCallback connected, id: " + id);

            if (NetworkManager.Singleton.IsServer)
            {
                new SpawnPlayerEvent(id);
                connectedPlayers++;
                if(connectedPlayers == 2)
                {
                    GameManager.Instance.StarGame();
                }
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += id =>
        {
            Debug.Log("OnClientDisconnectCallback disconnected, id: " + id);
            if (NetworkManager.Singleton.IsServer)
            {
                connectedPlayers--;
            }
        };

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            Debug.Log("OnServerStarted started");
        };
    }

    public async Task<bool> StartClient(string joinCode)
    {
        if (!useInternet)
        {
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client started");
                return true;
            }
            else
            {
                Debug.Log("Client failed to start");
                return false;
            }
        }
        bool result = await StartClientWithRelay(joinCode);
        if (result)
        {
            Debug.Log("Client started");
            new JoinCodeAssignEvent(true, joinCode);
            return true;
        }
        else
        {
            Debug.Log("Client failed to start");
            new JoinCodeAssignEvent(false, "Client failed to start");
            return false;
        }
    }

    public async Task<bool> StartHost()
    {
        if (!useInternet)
        {

            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started");
                return true;
            }
            else
            {
                Debug.Log("Host failed to start");
                return false;
            }
        }
        string code = await StartHostWithRelay();
        if (code != null)
        {
            Debug.Log("Host started");
            new JoinCodeAssignEvent(true, code);
            return true;
        }
        else
        {
            Debug.Log("Host failed to start");
            new JoinCodeAssignEvent(false, "Host failed to start");
            return false;
        }
    }

    public void ShutdownNetwork()
    {
        NetworkManager.Singleton.Shutdown();
        Debug.Log("Network shutdown");
    }

    /// <summary>
    /// Starts a game host with a relay allocation: it initializes the Unity services, signs in anonymously and starts the host with a new relay allocation.
    /// </summary>
    /// <param name="maxConnections">Maximum number of connections to the created relay.</param>
    /// <returns>The join code that a client can use.</returns>
    /// <exception cref="ServicesInitializationException"> Exception when there's an error during services initialization </exception>
    /// <exception cref="UnityProjectNotLinkedException"> Exception when the project is not linked to a cloud project id </exception>
    /// <exception cref="CircularDependencyException"> Exception when two registered <see cref="IInitializablePackage"/> depend on the other </exception>
    /// <exception cref="AuthenticationException"> The task fails with the exception when the task cannot complete successfully due to Authentication specific errors. </exception>
    /// <exception cref="RequestFailedException"> See <see cref="IAuthenticationService.SignInAnonymouslyAsync"/></exception>
    /// <exception cref="ArgumentException">Thrown when the maxConnections argument fails validation in Relay Service SDK.</exception>
    /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation service but results in an error.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the UnityTransport component cannot be found.</exception>
    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await OnAnonymouslySignIn();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    /// <summary>
    /// Joins a game with relay: it will initialize the Unity services, sign in anonymously, join the relay with the given join code and start the client.
    /// </summary>
    /// <param name="joinCode">The join code of the allocation</param>
    /// <returns>True if starting the client was successful</returns>
    /// <exception cref="ServicesInitializationException"> Exception when there's an error during services initialization </exception>
    /// <exception cref="UnityProjectNotLinkedException"> Exception when the project is not linked to a cloud project id </exception>
    /// <exception cref="CircularDependencyException"> Exception when two registered <see cref="IInitializablePackage"/> depend on the other </exception>
    /// <exception cref="AuthenticationException"> The task fails with the exception when the task cannot complete successfully due to Authentication specific errors. </exception>
    /// <exception cref="RequestFailedException">Thrown when the request does not reach the Relay Allocation service.</exception>
    /// <exception cref="ArgumentException">Thrown if the joinCode has the wrong format.</exception>
    /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation service but results in an error.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the UnityTransport component cannot be found.</exception>
    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await OnAnonymouslySignIn();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    public async Task<string> OnAnonymouslySignIn()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        return AuthenticationService.Instance.PlayerId;
    }
}
