using Solana.Unity.SDK;   // for Web3
using Solana.Unity.Wallet; // for Account
using UnityEngine;

public class WalletState : MonoBehaviour
{
    public static WalletState Instance { get; private set; }

    // Other scripts can read this
    public bool IsConnected { get; private set; }

    // we'll cache this once we find it
    private GameStarter _gameStarter;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // maybe the GameManager already exists in this scene
        TryFindGameStarter();
    }

    void OnEnable()
    {
        Web3.OnLogin  += HandleLogin;
        Web3.OnLogout += HandleLogout;

        // initialize from current state
        IsConnected = Web3.Wallet != null;
        UpdateGameStarterFlag();
    }

    void OnDisable()
    {
        Web3.OnLogin  -= HandleLogin;
        Web3.OnLogout -= HandleLogout;
    }

    private void HandleLogin(Account account)
    {
        IsConnected = true;
        Debug.Log("Wallet connected: " + account.PublicKey);
        UpdateGameStarterFlag();
    }

    private void HandleLogout()
    {
        IsConnected = false;
        Debug.Log("Wallet disconnected");
        UpdateGameStarterFlag();
    }

    private void UpdateGameStarterFlag()
    {
        // ensure we have a reference
        if (_gameStarter == null) TryFindGameStarter();

        if (_gameStarter != null)
            _gameStarter.allowPlay = IsConnected;
    }

    private void TryFindGameStarter()
    {
        var gm = GameObject.FindWithTag("GameManager");
        if (gm != null)
            _gameStarter = gm.GetComponent<GameStarter>();

        if (_gameStarter == null)
            Debug.LogWarning("WalletState: could not find GameStarter on tag 'GameManager'");
    }
}
