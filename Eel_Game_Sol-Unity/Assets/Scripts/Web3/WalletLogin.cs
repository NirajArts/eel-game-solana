using UnityEngine;
using UnityEngine.UI;
using Solana.Unity.Wallet;

namespace Solana.Unity.SDK.Example
{
    public class WalletLogin : SimpleScreen
    {
        [SerializeField]
        private Button loginBtnWalletAdapter;

        private void Start()
        {
            loginBtnWalletAdapter.onClick.AddListener(LoginWithWalletAdapter);
        }

        private async void LoginWithWalletAdapter()
        {
            if (Web3.Instance == null) return;

            var account = await Web3.Instance.LoginWalletAdapter();
            if (account != null)
            {
                // Successfully logged in
                gameObject.SetActive(false);
            }
            else
            {
                // Handle failed login
                Debug.LogWarning("Wallet adapter login failed.");
            }
        }
    }
}
