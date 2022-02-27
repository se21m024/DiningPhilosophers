using System.Diagnostics;

namespace DiningPhilosophers;

internal class Philosopher
{
    private readonly int _index;
    private readonly Forks _forks;
    private readonly int _thinkingTime;
    private readonly int _eatingTime;
    private readonly CancellationToken _ct;

    private const bool production = true;
    private const int delayMs = 600;

    private Stopwatch totalStopwatch = new Stopwatch();
    private Stopwatch waitingStopwatch = new Stopwatch();

    public long TotalTime => totalStopwatch.ElapsedMilliseconds;
    public long WaitingTime => waitingStopwatch.ElapsedMilliseconds;

    public Philosopher(
        int index,
        Forks forks,
        int thinkingTime,
        int eatingTime,
        CancellationToken ct)
    {
        _index = index;
        _forks = forks;
        _thinkingTime = thinkingTime;
        _eatingTime = eatingTime;
        _ct = ct;
    }

    public void StartDinner()
    {
        totalStopwatch.Start();
        try
        {
            while (!_ct.IsCancellationRequested)
            {
                this.Think();
                if (production)
                {
                    if (_index % 2 == 0)
                    {
                        this.TakeFork(true);
                        Thread.Sleep(delayMs);
                        this.TakeFork(false);
                    }
                    else
                    {
                        this.TakeFork(false);
                        Thread.Sleep(delayMs);
                        this.TakeFork(true);
                    }
                }
                else
                {
                    this.TakeFork(true);
                    Thread.Sleep(delayMs);
                    this.TakeFork(false);
                }
                this.Eat();
                this.PutBackForks();
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            totalStopwatch.Stop();
            Console.WriteLine($"P{_index} ends dinner. TotalTime:  {totalStopwatch.ElapsedMilliseconds}  WaitingTime: {waitingStopwatch.ElapsedMilliseconds}");
        }
    }

    private void Think()
    {
        Console.WriteLine($"P{_index} is thinking.");
        Thread.Sleep(new Random().Next(_thinkingTime));
    }

    private void TakeFork(bool leftFork)
    {
        Console.WriteLine($"P{_index} waits for {(leftFork ? "left" : "right")} fork.");
        waitingStopwatch.Start();
        _forks.TakeFork(_index, leftFork, _ct);
        waitingStopwatch.Stop();
        Console.WriteLine($"P{_index} has taken {(leftFork ? "left" : "right")} fork.");
    }

    private void Eat()
    {
        Console.WriteLine($"P{_index} is eating.");
        Thread.Sleep(new Random().Next(_eatingTime));
    }

    private void PutBackForks()
    {
        Console.WriteLine($"P{_index} puts back forks.");
        _forks.PutBackForks(_index);
    }
}
