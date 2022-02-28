try
{
    Console.WriteLine("Dining Philosophers\n" +
                      "Thomas Bründl\n" +
                      "Thomas Stummer\n");

    Console.Write("Number of the philosophers: ");
    var n = Convert.ToInt32(Console.ReadLine());
    Console.Write("Thinking time [ms}: ");
    var thinkingTime = Convert.ToInt32(Console.ReadLine());
    Console.Write("Eating time [ms]: ");
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
    var relativeWaitingTime =
        Convert.ToDouble(philosophers.Sum(x => x.WaitingTime)) /
        philosophers.Sum(x => x.TotalTime);
    Console.WriteLine($"Relative waiting time: {relativeWaitingTime}");
    Console.WriteLine("Press RETURN to exit the program.");
    Console.ReadLine();
}
catch (Exception e)
{
    Console.WriteLine($"Unhandled exception occurred: {e}");
    Console.WriteLine("Press RETURN to exit the program.");
    Console.ReadLine();
}

internal class Fork
{
    private readonly int _index;
    private int _takenByPhilosopher;

    public Fork(int index)
    {
        _index = index;
        _takenByPhilosopher = -1;
    }

    public void Take(int philosopher)
    {
        if (_takenByPhilosopher > 0)
        {
            throw new Exception(
                $"Fork {_index} was already taken by p{_takenByPhilosopher} " +
                $"when p{philosopher} tried to take it.");
        }

        _takenByPhilosopher = philosopher;
    }

    public void PutBack(int philosopher)
    {
        if (_takenByPhilosopher != philosopher)
        {
            throw new Exception(
                $"Fork {_index} was already taken by p{_takenByPhilosopher} " +
                $"when p{philosopher} tried to put it back.");
        }

        _takenByPhilosopher = -1;
    }
}

internal class Forks
{
    private readonly int _n;
    private readonly Fork[] _forks;
    private readonly SemaphoreSlim[] _locks;

    public Forks(int n)
    {
        _n = n;

        _forks = new Fork[n];
        for (var i = 0; i < _n; i++)
        {
            _forks[i] = new Fork(i);
        }

        _locks = new SemaphoreSlim[n];
        for (var i = 0; i < n; i++)
        {
            _locks[i] = new SemaphoreSlim(1, 1);
        }
    }

    public void TakeFork(int philosopher, bool leftFork, CancellationToken ct)
    {
        var forkIndex = leftFork ? philosopher : (philosopher + 1) % _n;
        _locks[forkIndex].Wait(ct);
        _forks[forkIndex].Take(philosopher);
    }

    public void PutBackForks(int philosopher)
    {
        this.PutBackFork(philosopher, philosopher);
        this.PutBackFork(philosopher, (philosopher + 1) % _n);
    }

    public void PutBackFork(int philosopher, int fork)
    {
        _forks[fork].PutBack(philosopher);
        _locks[fork].Release();
    }
}

internal class Philosopher
{
    private readonly int _index;
    private readonly Forks _forks;
    private readonly int _thinkingTime;
    private readonly int _eatingTime;
    private readonly System.Diagnostics.Stopwatch _totalStopwatch = new();
    private readonly System.Diagnostics.Stopwatch _waitingStopwatch = new();
    private readonly CancellationToken _ct;

    private const bool PreventDeadlocks = true;
    private const int DelayMs = 600;

    public long TotalTime => _totalStopwatch.ElapsedMilliseconds;
    public long WaitingTime => _waitingStopwatch.ElapsedMilliseconds;

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
        _totalStopwatch.Start();
        try
        {
            while (!_ct.IsCancellationRequested)
            {
                this.Think();
                if (PreventDeadlocks)
                {
                    if (_index % 2 == 0)
                    {
                        this.TakeFork(true);
                        Thread.Sleep(DelayMs);
                        this.TakeFork(false);
                    }
                    else
                    {
                        this.TakeFork(false);
                        Thread.Sleep(DelayMs);
                        this.TakeFork(true);
                    }
                }
                else
                {
                    this.TakeFork(true);
                    Thread.Sleep(DelayMs);
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
            _totalStopwatch.Stop();
            Console.WriteLine($"P{_index} ends dinner. TotalTime:  {_totalStopwatch.ElapsedMilliseconds}  WaitingTime: {_waitingStopwatch.ElapsedMilliseconds}");
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
        _waitingStopwatch.Start();
        _forks.TakeFork(_index, leftFork, _ct);
        _waitingStopwatch.Stop();
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
