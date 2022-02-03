using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace Blockchain
{
    class Program
    {
        static void Main(string[] args)
        {
            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();

            PrivateKey key2 = new PrivateKey();
            PublicKey wallet2 = key2.publicKey();

            BlockChain astrocoin = new BlockChain(2, 100);


            //create transactions 
            Console.WriteLine("Start the miner.");
            astrocoin.MinePendingTransactions(wallet1);
            //decimal bal1 = astrocoin.GetBalanceOfWallet(wallet1);
            Console.WriteLine("\nBalance of wallet1 is $" + astrocoin.GetBalanceOfWallet(wallet1).ToString());




            //astrocoin.AddBlock(new Block(1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "amo));
            //astrocoin.AddBlock(new Block(2, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "amount: 200"));


            Transaction tx1 = new Transaction(wallet1, wallet2, 10);
            tx1.SignTransaction(key1);
            astrocoin.addPendingTransaction(tx1);
            Console.WriteLine("Start the miner.");
            astrocoin.MinePendingTransactions(wallet2);

            Console.WriteLine("\nBalance of wallet1 is $" + astrocoin.GetBalanceOfWallet(wallet1).ToString());
            Console.WriteLine("\nBalance of wallet2 is $" + astrocoin.GetBalanceOfWallet(wallet2).ToString());


            string blockJSON = JsonConvert.SerializeObject(astrocoin, Formatting.Indented);
            Console.WriteLine(blockJSON);

            if (astrocoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is Valid!");
            }
            else
            {
                Console.WriteLine("Blockchain is not valid");
            }
        }
    }
}