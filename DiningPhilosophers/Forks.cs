namespace DiningPhilosophers;

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
        for(var i = 0; i < n; i++)
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
