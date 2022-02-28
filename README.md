# Parallel Programming

# Exercise 1, Concurrency - Dining Philosophers

# Autors: Thomas Bründl, Thomas Stummer

## What are the necessary conditions for deadlocks (discussed in the lecture)?

1. Mutual exculion
2. Hold and wait
3. No preemption
4. Circular Wait
   <br/>
   <br/>

## Why does the initial solution lead to a deadlock (by looking at the deadlock conditions)

Because all four deadlock conditions are met.

1. Mutual Exclusion

Two or more resources are non-shareable.
This means that every fork is limited and can only be used by one pilosipher at once.

2. Hold and wait

A process is holding at least one resource and waiting for resources.
This means if a philosopher takes the first fork it will wait until it can take the second fork as well.
If the second fork is taken indefinitely by another process the philosopher is blocked and it won't release the first fork.

3. No Preemption

A resource cannot be taken from a process unless the process releases the resource.
This means that it is not allowed to use a resource (fork) that is in use by an other philosopher (thread).

4. Circular Wait

A set of processes waits for each other in a circle.
If each process takes the left fork first, it is not possible to take the right fork.
This is because of the circular form of the philosophers.

<br/>
<br/>

## Switch the order in which philosophers take the fork by using the following scheme: Odd philosophers start with the left fork, while even philosophers start with the right hand. Make sure to use concurrency primitives correctly!

This logic has been implemented in the Philosophers.cs.
The dead lock prevention logic can be switched on and off via the PreventDeadlocks (see Philosophers.cs line 149 ) const.

<br/>
<br/>

## Does this strategy resolve the deadlock and why?

The philosophers with an even index only take the left fork at first and philosophers with an uneven index take the right fork at first. This logic will prevent the "Curcular wait".

This prevents that every philosopher takes only the left fork at once which would result in a "Hold and Wait".
(Because every philosopher would try to take the right fork.
The right fork would be in use by every philosopher as their left fork.)

<br/>
<br/>

## Measure the total time spent in waiting for forks and compare it to the total runtime. Interpret the measurement - Was the result expected?

The following times were measured without artificial sleeps (DelayMs = 0).

| Philosophers            | 10     | 10     | 100     | 100      |
| ----------------------- | ------ | ------ | ------- | -------- |
| Thinking time [ms]      | 100    | 100    | 100     | 100      |
| Eating time [ms]        | 100    | 200    | 100     | 200      |
| Total time [ms]         | 589812 | 910514 | 7046645 | 10059224 |
| Total waiting time [ms] | 271809 | 420903 | 3241361 | 4656962  |
| Relative waiting time   | 46,1 % | 46,2 % | 46,0 %  | 46,3 %   |

**Conslusion:**<br/>
Different parameters (ratio of thinking time to eating time or number of philosophers) do not affect the relative waiting time (waiting time compared to the overall time). From our side an impact on the relative waiting time when changing the program parameters was expected.
<br/>
<br/>

## Can you think of other techniques for deadlock prevention?

- **Timeout for taking second fork**<br/>
  If a philosopher holding the first fork was not waiting for the second fork eternally but returning the first fork and go into thinking mode after a configured timeout if he is not allowed to take the second fork, the hold and wait deadlock condition could be prevented. Another philosophers could than pick up the fork the other philosopher dropped.

- **Mediator**<br/>
  A mediator that coordinates the access to the forks could be uses. The mediator assigns the forks pairwise to a philosopher. Therefore the hold and wait condition is avoided.

- **Exclude particular philosopher**<br/>
  Prevent all philosophers to access the forks at once. E.g. one philosopher must not access the forks while all other philosophers are allowed to access the forks any time. With this restriction the circular dependency constraint is bypassed.
  <br/>
  <br/>

## Make sure to always shutdown the program cooperatively and to always cleanup all allocated resources.

A cancellation token is passed to all philosopher threads. When the user wishes to terminate the program, the cancellation token is cancelled. If a philosopher was thinking or eating at the moment the token was cancelled, he finishes is current action. Afterwards he put down all forks he currently held. If a philosopher was waiting for one or more forks, the waiting was cancelled and he put down alls forks he currently held.
