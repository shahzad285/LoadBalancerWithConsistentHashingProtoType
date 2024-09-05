

using LoadBalancerWithConsistentHashingProtoType;

LoadBalancer loadBalancer = new LoadBalancer();

// Create and start initial servers on different ports
Server server1 = new Server(5001);
Server server2 = new Server(5002);
Server server3 = new Server(5003);

loadBalancer.AddServer(server1);
loadBalancer.AddServer(server2);
loadBalancer.AddServer(server3);

// Start servers on separate threads
StartServerThread(server1);
StartServerThread(server2);
StartServerThread(server3);


Thread requestThread = new Thread(() =>
{
    int i = 1;
    while (true)
    {
        
            loadBalancer.RouteRequest("Req"+i);
            Thread.Sleep(3000); // Simulate time between requests
        i++;
    }
});
requestThread.Start();

// Listen for input to add a new server
while (true)
{
    Console.WriteLine("Enter a port number to add a new server, or 'q' to quit:");
    var input = Console.ReadLine();

    if (input.ToLower() == "q")
        break;

    if (int.TryParse(input, out int newPort))
    {
        Server newServer = new Server(newPort);
        loadBalancer.AddServer(newServer);
        StartServerThread(newServer);
    }
    else
    {
        Console.WriteLine("Invalid port number. Please try again.");
    }
}

static void StartServerThread(Server server)
{
    Thread serverThread = new Thread(new ThreadStart(server.Start));
    serverThread.Start();
}