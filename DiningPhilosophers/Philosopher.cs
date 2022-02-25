namespace DiningPhilosophers;

internal class Philosopher
{
    private readonly int _index;
    private readonly Forks _forks;
    private readonly int _thinkingTime;
    private readonly int _eatingTime;
    private readonly CancellationToken _ct;

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
        try
        {
            while (!_ct.IsCancellationRequested)
            {
                this.Think();
                this.TakeFork(true);
                this.TakeFork(false);
                this.Eat();
                this.PutBackForks();
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Console.WriteLine($"P{_index} ends dinner.");
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
        _forks.TakeFork(_index, leftFork, _ct);
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
