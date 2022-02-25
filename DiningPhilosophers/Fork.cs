namespace DiningPhilosophers;

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

