using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Cysharp.Threading.Tasks;
using Solana.Unity.Programs;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using UnityEngine;
using System.Net.WebSockets;
using Solana.Unity.Wallet.Bip39;
// using Unity.PlasticSCM.Editor.WebApi;
using Solana.Unity.SDK;
using Eel_Game;
using Eel_Game.Program;
using Eel_Game.Accounts;

[DefaultExecutionOrder(-50)]        // make sure this runs after Web3.Setup()
public class EelGameIntegration : MonoBehaviour
{
    private static readonly PublicKey ProgramId = new PublicKey(XyzProgram.ID);

    IRpcClient           _rpc;
    IStreamingRpcClient  _ws;
    XyzClient            _client;
    Account              _wallet;
    PublicKey            _playerPda;
    SubscriptionState    _subscription;

    void Start()
    {
        
    }
    
    void OnEnable()
    {
        // if you’re already logged in:
        if (Web3.Account != null)
            InitializeWith(Web3.Account);

        Web3.OnLogin   += InitializeWith;
        Web3.OnLogout  += TearDown;
    }

    void OnDisable()
    {
        Web3.OnLogin   -= InitializeWith;
        Web3.OnLogout  -= TearDown;
        if (_subscription != null)
            _ = Web3.WsRpc.UnsubscribeAsync(_subscription);
    }

    void InitializeWith(Account account)
    {
        // grab the wallet + clients
        _wallet = account;
        _rpc    = Web3.Rpc;
        _ws     = Web3.WsRpc;
        _client = new XyzClient(_rpc, _ws, ProgramId);

        // derive your PDA seed exactly as in your Anchor program:
        PublicKey.TryFindProgramAddress(
            new[] { Encoding.UTF8.GetBytes("player-account"), _wallet.PublicKey.KeyBytes },
            ProgramId,
            out _playerPda,
            out _);

        Debug.Log($"[Xyz] logged in as {_wallet.PublicKey}, PDA = {_playerPda}");
    }

    void TearDown()
    {
        _wallet = null;
        _client = null;
        // unsubscribe if needed...
    }

    async Task<RequestResult<string>> SignAndSend(Transaction tx)
    {
        tx.FeePayer        = _wallet.PublicKey;
        tx.RecentBlockHash = (await _rpc.GetLatestBlockHashAsync()).Result.Value.Blockhash;
        tx.Sign(_wallet);
        return await _rpc.SendTransactionAsync(tx.Serialize(),
                                              skipPreflight: true,
                                              commitment: Commitment.Confirmed);
    }

    public async Task RegisterPlayer()
    {
        var tx = new Transaction();
        tx.Add(XyzProgram.RegisterPlayer(new RegisterPlayerAccounts {
            PlayerAccount = _playerPda,
            Authority     = _wallet.PublicKey
        }));

        var res = await SignAndSend(tx);
        Debug.Log(res.WasSuccessful
            ? $"Player registered! Tx: {res.Result}"
            : $"Register failed: {res.Reason}");
    }

    public async Task SaveScore(uint score)
    {
        var tx = new Transaction();
        tx.Add(XyzProgram.SaveScore(new SaveScoreAccounts {
            PlayerAccount = _playerPda,
            Authority     = _wallet.PublicKey
        }, score));

        var res = await SignAndSend(tx);
        Debug.Log(res.WasSuccessful
            ? $"Score {score} saved."
            : $"SaveScore failed: {res.Reason}");
    }

    public async Task SubscribeToPlayer()
    {
        if (_subscription != null)
            await _ws.UnsubscribeAsync(_subscription);

        _subscription = await _ws.SubscribeAccountInfoAsync(
            _playerPda.ToString(),
            (state, info) => {
                if (info.Value?.Data?.Count > 0)
                {
                    var acc = PlayerAccount.Deserialize(
                        Convert.FromBase64String(info.Value.Data[0])
                    );
                    Debug.Log($"On‐chain update: Score = {acc.Score}");
                }
            },
            commitment: Commitment.Processed
        );
    }
}
