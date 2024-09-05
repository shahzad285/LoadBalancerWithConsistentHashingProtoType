using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancerWithConsistentHashingProtoType
{
    public class LoadBalancer
    {
        private SortedDictionary<BigInteger, Server> hashRing = new SortedDictionary<BigInteger, Server>();
        private int numberOfReplicas;

        public LoadBalancer(int numberOfReplicas = 0)
        {
            this.numberOfReplicas = numberOfReplicas;
        }

        private BigInteger Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return new BigInteger(hashBytes);
            }
        }

        public void AddServer(Server server)
        {
            numberOfReplicas++;
            string replicaId = $"{server._port}";
            var position = Hash(replicaId);
            hashRing[position] = server;
            Console.WriteLine($"Server on port {server._port} added to hash ring at position {position}");

        }

        public Server GetServer(string key)
        {
            var keyHash = Hash(key);

            foreach (var position in hashRing.Keys)
            {
                if (keyHash <= position)
                {
                    return hashRing[position];
                }
            }

            return hashRing[hashRing.First().Key]; // Wrap around to the first server
        }

        public void RouteRequest(string request)
        {
            var server = GetServer(request);
            Console.WriteLine($"Number of Servers {numberOfReplicas} Routing request '{request}' to server on port {server._port}");
        }
    }
}
