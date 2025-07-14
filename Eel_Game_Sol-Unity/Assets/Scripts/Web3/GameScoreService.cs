using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;                                // for Debug.Log
using Solana.Unity.Rpc;                           // IRpcClient
using Solana.Unity.Rpc.Models;                    // Transaction, TransactionInstruction, AccountMeta, SignaturePubKeyPair
using Solana.Unity.Rpc.Types;                     // Commitment
using Solana.Unity.Wallet;                        // Account
using Solana.Unity.Programs;                      // SystemProgram (ProgramIdKey) :contentReference[oaicite:0]{index=0}
using Solana.Unity.SDK;                           // Web3

public static class GameScoreService
{
    private static readonly PublicKey ProgramId      = new PublicKey("3KepTTF6YqFgCwJXCubwzNTXsP2z5HyQTsW3Y6hG8vWh");
    private static readonly PublicKey SystemProgramId = SystemProgram.ProgramIdKey;

    public static async Task<string> RegisterPlayerAsync()
    {
        // 1) new on‐chain player account keypair
        var playerKeypair = new Account();

        // 2) Anchor discriminator for `register_player`
        byte[] data = new byte[] { 242, 146, 194, 234, 234, 145, 228, 42 };

        // 3) build the instruction
        var ix = new TransactionInstruction
        {
            ProgramId = ProgramId,
            Keys = new List<AccountMeta>
            {
                AccountMeta.Writable(playerKeypair.PublicKey, true),  // new account must sign
                AccountMeta.Writable(Web3.Account.PublicKey,    true), // payer
                AccountMeta.ReadOnly (SystemProgramId,          false) // system_program
            },
            Data = data
        };

        // 4) fetch recent blockhash
        string blockHash = await Web3.BlockHash();

        // 5) build the raw Transaction (no SetXXX helpers here)
        var tx = new Transaction
        {
            FeePayer = Web3.Account.PublicKey,
            RecentBlockHash = blockHash,
            Instructions = new List<TransactionInstruction> { ix },
            Signatures = new List<SignaturePubKeyPair>()
        };

        // 6) sign it with both your wallet & the new account
        var signed = await Web3.Wallet.SignTransaction(tx);

        // 7) send it (note the `false` skipPreflight param) :contentReference[oaicite:1]{index=1}
        var raw = Convert.ToBase64String(signed.Serialize());
        var res = await Web3.Rpc.SendTransactionAsync(raw, false, Commitment.Confirmed);
        return res.Result;
    }

    public static async Task<string> SaveScoreAsync(PublicKey playerPubkey, uint newScore)
    {
        // 1) discriminator + little-endian u32
        byte[] disc       = new byte[] {228, 233, 177, 6, 181, 63, 194, 152};
        byte[] scoreBytes = BitConverter.GetBytes(newScore);
        byte[] data       = disc.Concat(scoreBytes).ToArray();

        // 2) instruction
        var ix = new TransactionInstruction
        {
            ProgramId = ProgramId,
            Keys = new List<AccountMeta>
            {
                AccountMeta.Writable(playerPubkey, false),
                AccountMeta.ReadOnly (Web3.Account.PublicKey, true)
            },
            Data = data
        };

        // 3) recent blockhash
        string blockHash = await Web3.BlockHash();

        // 4) tx
        var tx = new Transaction
        {
            FeePayer       = Web3.Account.PublicKey,
            RecentBlockHash = blockHash,
            Instructions   = new List<TransactionInstruction> { ix },
            Signatures     = new List<SignaturePubKeyPair>()
        };

        // 5) sign & send
        var signed = await Web3.Wallet.SignTransaction(tx);
        var raw    = Convert.ToBase64String(signed.Serialize());
        var res    = await Web3.Rpc.SendTransactionAsync(raw, false, Commitment.Confirmed);
        return res.Result;
    }

    public static async Task SubscribeToScoreUpdatesAsync(
    PublicKey playerPubkey,
    Action<uint> onScoreUpdated
) {
    await Web3.WsRpc.SubscribeAccountInfoAsync(
        playerPubkey,
        (subId, accountInfo) =>
        {
            var raw = accountInfo.Value.Data;
            if (raw == null || raw.Count == 0) return;

            var bytes = Convert.FromBase64String(raw[0]);
            uint score = BitConverter.ToUInt32(bytes, 8 + 32);

            // marshal back to Unity's main thread
            Cysharp.Threading.Tasks.UniTask.Void(async () =>
            {
                await Cysharp.Threading.Tasks.UniTask.SwitchToMainThread();
                onScoreUpdated(score);
            });
        },
        Commitment.Confirmed
    );
}

}
