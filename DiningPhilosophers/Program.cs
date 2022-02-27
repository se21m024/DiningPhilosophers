using DiningPhilosophers;
using System.Linq;

try
{
    Console.WriteLine("Dining Philosophers\n" +
                      "Thomas Bründl\n" +
                      "Thomas Stummer\n");

    Console.WriteLine("Number of the philosophers: ");
    var n = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("Thinking time [ms}: ");
    var thinkingTime = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("Eating time [ms]: ");
    var eatingTime = Convert.ToInt32(Console.ReadLine());

    var philosophers = new Philosopher[n];

    var philosopherThreads = new Thread[n];
    var forks = new Forks(n);

    var cts = new CancellationTokenSource();

    for (var i = 0; i < n; i++)
    {

        philosophers[i] = new Philosopher(i, forks, thinkingTime, eatingTime, cts.Token);
        philosopherThreads[i] = new Thread(philosophers[i].StartDinner);
        philosopherThreads[i].Start();
    }

    Console.WriteLine("\nPress RETURN to end the dinner.\n");
    Console.ReadLine();
    Console.WriteLine("\nEnding dinner.\n");
    cts.Cancel();

    foreach (var p in philosopherThreads)
    {
        p.Join();
    }

    Console.WriteLine($"TotalTime: {philosophers.Sum(x => x.TotalTime)}");
    Console.WriteLine($"WaitingTime: {philosophers.Sum(x => x.WaitingTime)}");
    Console.WriteLine($"Fraction: {Convert.ToDouble(philosophers.Sum(x => x.WaitingTime)) / philosophers.Sum(x => x.TotalTime)}");
    Console.WriteLine("Press RETURN to exit the program.");
    Console.ReadLine();

}
catch (Exception e)
{
    Console.WriteLine($"Unhandled exception occurred: {e}");
    Console.WriteLine("Press RETURN to exit the program.");
    Console.ReadLine();
}
