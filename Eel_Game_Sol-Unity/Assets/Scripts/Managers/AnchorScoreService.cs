using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Solana.Unity.SDK;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Wallet;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Eel_Game;
using Eel_Game.Program;
using Eel_Game.Accounts;

public class AnchorScoreService
{
    // Get current connected wallet base
    private WalletBase Wallet => Web3.Instance.WalletBase;
    private IRpcClient RpcClient => Wallet?.ActiveRpcClient;

    public bool TryDerivePlayerAccount(out PublicKey pda)
    {
        pda = null;
        if (Wallet == null) return false;
        var seeds = new List<byte[]>
        {
            Encoding.UTF8.GetBytes("player_account"),
            Wallet.Account.PublicKey.KeyBytes
        };
        return PublicKey.TryFindProgramAddress(
            seeds.ToArray(),
            new PublicKey(XyzProgram.ID),
            out pda,
            out _
        );
    }

    public async Task<RequestResult<string>> RegisterPlayer()
    {
        if (Wallet == null) { Debug.LogError("No wallet connected!"); return null; }
        if (!TryDerivePlayerAccount(out var playerAccount))
        {
            Debug.LogError("Failed to derive player PDA.");
            return null;
        }
        var accounts = new RegisterPlayerAccounts
        {
            PlayerAccount = playerAccount,
            Authority = Wallet.Account.PublicKey,
            SystemProgram = new PublicKey("11111111111111111111111111111111")
        };
        var ix = XyzProgram.RegisterPlayer(accounts);
        var recentBlockHash = (await RpcClient.GetRecentBlockHashAsync(Commitment.Confirmed)).Result.Value.Blockhash;
        var tx = new Transaction
        {
            FeePayer = Wallet.Account.PublicKey,
            RecentBlockHash = recentBlockHash,
            Instructions = new List<TransactionInstruction> { ix }
        };
        var signedTx = await Wallet.SignTransaction(tx);
        var result = await RpcClient.SendTransactionAsync(
            Convert.ToBase64String(signedTx.Serialize()), true, Commitment.Confirmed
        );
        Debug.Log($"RegisterPlayer tx sent: {result.Result}");
        return result;
    }

    public async Task<RequestResult<string>> SaveScore(uint newScore)
    {
        if (Wallet == null) { Debug.LogError("No wallet connected!"); return null; }
        if (!TryDerivePlayerAccount(out var playerAccount))
        {
            Debug.LogError("Failed to derive player PDA.");
            return null;
        }
        var accounts = new SaveScoreAccounts
        {
            PlayerAccount = playerAccount,
            Authority = Wallet.Account.PublicKey
        };
        var ix = XyzProgram.SaveScore(accounts, newScore);
        var recentBlockHash = (await RpcClient.GetRecentBlockHashAsync(Commitment.Confirmed)).Result.Value.Blockhash;
        var tx = new Transaction
        {
            FeePayer = Wallet.Account.PublicKey,
            RecentBlockHash = recentBlockHash,
            Instructions = new List<TransactionInstruction> { ix }
        };
        var signedTx = await Wallet.SignTransaction(tx);
        var result = await RpcClient.SendTransactionAsync(
            Convert.ToBase64String(signedTx.Serialize()), true, Commitment.Confirmed
        );
        Debug.Log($"SaveScore tx sent: {result.Result}");
        return result;
    }

    public async Task<PlayerAccount> GetPlayerAccount()
    {
        if (Wallet == null) { Debug.LogError("No wallet connected!"); return null; }
        if (!TryDerivePlayerAccount(out var playerAccount))
        {
            Debug.LogError("Failed to derive player PDA.");
            return null;
        }
        var xyzClient = new XyzClient(RpcClient, Wallet.ActiveStreamingRpcClient);
        var response = await xyzClient.GetPlayerAccountAsync(playerAccount.Key);
        if (response.ParsedResult != null)
        {
            Debug.Log($"Fetched PlayerAccount: score={response.ParsedResult.Score} authority={response.ParsedResult.Authority}");
        }
        else
        {
            Debug.LogWarning("PlayerAccount not found or not initialized.");
        }
        return response.ParsedResult;
    }
}
