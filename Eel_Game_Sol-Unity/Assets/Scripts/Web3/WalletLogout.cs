using UnityEngine;
using UnityEngine.UI;
using Solana.Unity.SDK;

namespace Solana.Unity.SDK.Example
{
    /// <summary>
    /// Provides a simple logout button that calls Web3.Logout()
    /// </summary>
    public class WalletLogout : SimpleScreen
    {
        [SerializeField]
        private Button logoutBtn;

        private void Start()
        {
            if (logoutBtn == null)
            {
                Debug.LogError("WalletLogout: logoutBtn is not assigned in the Inspector.");
                return;
            }

            logoutBtn.onClick.AddListener(OnLogoutButtonClicked);
        }

        private void OnLogoutButtonClicked()
        {
            if (Web3.Instance == null)
            {
                Debug.LogWarning("WalletLogout: Web3 instance not found.");
                return;
            }

            // Perform logout
            Web3.Instance.Logout();
            Debug.Log("WalletLogout: Successfully logged out.");

            // Hide or disable this UI screen
            gameObject.SetActive(false);
        }
    }
}
