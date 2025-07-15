using UnityEngine;
using UnityEngine.UI;
using Solana.Unity.Wallet;

namespace Solana.Unity.SDK.Example
{
    public class GoogleLogin : SimpleScreen
    {
        [SerializeField]
        private Button loginBtnGoogle;

        private void Start()
        {
            loginBtnGoogle.onClick.AddListener(() => LoginWithGoogle());
        }

        private async void LoginWithGoogle()
        {
            var account = await Web3.Instance.LoginWeb3Auth(Provider.GOOGLE);
            if (account != null)
            {
                // Successfully logged in
                gameObject.SetActive(false);
            }
            else
            {
                // Handle failed login if needed
                Debug.LogWarning("Google login failed.");
            }
        }
    }
}
