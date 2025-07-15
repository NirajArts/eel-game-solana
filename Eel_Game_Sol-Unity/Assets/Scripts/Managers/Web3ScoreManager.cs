using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Rpc.Models;

public class Web3ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("TextMeshPro UGUI component to display the score")]
    [SerializeField] private TMP_Text scoreText;

    private PublicKey _playerPubkey;
    private uint      _currentScore = 0;

    private async void Start()
    {
        // 1) Wait until the wallet connects
        await UniTask.WaitUntil(() => Web3.Account != null);

        // 2) Grab the wallet's public key as your on-chain account
        _playerPubkey = Web3.Account.PublicKey;
        Debug.Log($"Using on-chain account: {_playerPubkey}");

        // 3) Give a little breathing room for RPC initialization
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        // 4) Fetch initial score
        var resp = await Web3.Rpc.GetAccountInfoAsync(_playerPubkey, Commitment.Confirmed);
        if (resp?.Result?.Value?.Data?.Count > 0)
        {
            byte[] raw = Convert.FromBase64String(resp.Result.Value.Data[0]);
            _currentScore = BitConverter.ToUInt32(raw, 8 + 32);
        }
        else _currentScore = 0;
        UpdateScoreText(_currentScore);

        // 5) Subscribe to live updates
        await GameScoreService.SubscribeToScoreUpdatesAsync(
            _playerPubkey,
            newScore =>
            {
                _currentScore = newScore;
                UpdateScoreText(_currentScore);
            }
        );
    }

    private void UpdateScoreText(uint score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    /// <summary>
    /// Call this when you want to submit the final score on-chain.
    /// </summary>
    public async Task SubmitScoreAsync(uint newScore)
    {
        _currentScore = newScore;
        UpdateScoreText(_currentScore);

        try
        {
            string txSig = await GameScoreService.SaveScoreAsync(_playerPubkey, newScore);
            Debug.Log($"Score submitted: {newScore}, txSig: {txSig}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to submit score: {e.Message}");
        }
    }
}
