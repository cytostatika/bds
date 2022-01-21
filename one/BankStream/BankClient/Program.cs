using Common;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

using var client = new ClientBuilder()
    .UseLocalhostClustering()
    .ConfigureLogging(logging => logging.AddConsole())
    .AddSimpleMessageStreamProvider(Constants.StreamProvider)
    .Build();

await client.Connect();


var accountNames = new[] {"Xaawo", "Pasqualino", "Derick", "Ida", "Stacy", "Xiao"};
var random = new Random();
while (!Console.KeyAvailable)
{
    var atm = client.GetGrain<IAtmGrain>(0);

    // Choose some random accounts to exchange money
    var fromId = random.Next(accountNames.Length);
    var from2Id = random.Next(accountNames.Length);
    while (from2Id == fromId)
        // Avoid transferring to/from the same account, since it would be meaningless
        from2Id = (from2Id + 1) % accountNames.Length;

    var toId = random.Next(accountNames.Length);
    while (toId == fromId || toId == fromId)
        // Avoid transferring to/from the same account, since it would be meaningless
        toId = (toId + 1) % accountNames.Length;


    var fromName = accountNames[fromId];
    var from2Name = accountNames[from2Id];

    var toName = accountNames[toId];
    var from = client.GetGrain<IAccountGrain>(fromName);
    var from2 = client.GetGrain<IAccountGrain>(from2Name);

    var to = client.GetGrain<IAccountGrain>(toName);

    // Perform the transfer and query the results
    try
    {
        await atm.Transfer(from, from2, to, 100);

        var fromBalance = await from.GetBalance();
        var from2Balance = await from2.GetBalance();
        var toBalance = await to.GetBalance();


        Console.WriteLine(
            $"We transferred 100 credits from {fromName} and {from2Name} to {toName}.\n{fromName} balance: {fromBalance}, {from2Name} balance:{from2Balance}\n{toName} balance: {toBalance}\n");
    }
    catch (InvalidOperationException exception)
    {
        Console.WriteLine(
            $"Error transferring 100 credits from {fromName} and {from2Name} to {toName}: {exception.Message}");
        if (exception.InnerException is { } inner) Console.WriteLine($"\tInnerException: {inner.Message}\n");

        Console.WriteLine();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }

    // Sleep and run again
    await Task.Delay(200);
}