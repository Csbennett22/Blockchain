using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace Blockchain
{
    class BlockChain
    {
        public List<Block> Chain { get; set; }
        public int difficulty { get; set; }
        List<Transaction> pendingTransaction { get; set; }
        public decimal MiningReward { get; set; }

        public BlockChain(int difficulty, decimal miningreward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock());
            this.difficulty = difficulty;
            this.MiningReward = miningreward;
            this.pendingTransaction = new List<Transaction>();
        }

        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHmmssffff"), new List<Transaction>());
        }

        public Block GetLastBlock()
        {
            return this.Chain.Last();
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = this.GetLastBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);
        }
        public void addPendingTransaction(Transaction transaction)
        {
            if (transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transactions must have to and from addresses.");
            }

            if (transaction.Amount > this.GetBalanceOfWallet(transaction.FromAddress))
            {
                throw new Exception("There is not sufficient money in the wallet");
            }

            if (transaction.IsValid() == false)
            {
                throw new Exception("Cannot add invalid transactions to a block.");
            }
            this.pendingTransaction.Add(transaction);
        }

        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (!(transaction.FromAddress is null))
                    {
                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");
                        
                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }
                
                    string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");
                    if (toDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }

        public void MinePendingTransactions(PublicKey miningRewardWallet)
        {
            Transaction rewardTx = new Transaction(null, miningRewardWallet, MiningReward);
            this.pendingTransaction.Add(rewardTx);

            Block newBlock = new Block(GetLastBlock().Index + 1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), this.pendingTransaction, GetLastBlock().Hash);
            newBlock.Mine(this.difficulty);

            Console.WriteLine("Block successfully mined");
            this.Chain.Add(newBlock);
            this.pendingTransaction = new List<Transaction>();

        }

        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                //Check uf current block has is the same as the calcualted chash
                Block currentBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }

            }
            return true;
        }
    }
}
