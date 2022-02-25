namespace DiningPhilosophers;

internal class Forks
{
    private readonly int _n;
    private readonly SemaphoreSlim[] _forks;

    public Forks(int n)
    {
        _n = n;
        _forks = new SemaphoreSlim[n];
        for(var i = 0; i < n; i++)
        {
            _forks[i] = new SemaphoreSlim(1, 1);
        }
    }

    public void TakeFork(int philosopher, bool leftFork, CancellationToken ct)
    {
        var forkIndex = leftFork ? philosopher : (philosopher + 1) % _n;
        _forks[forkIndex].Wait(ct);
    }

    public void PutBackForks(int philosopher)
    {
        _forks[philosopher].Release();
        _forks[(philosopher + 1) % _n].Release();
    }
}
