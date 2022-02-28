# Parallel Programming

# Exercise 1, Concurrency - Dining Philosophers

# Autors: Thomas Bründl, Thomas Stummer

## What are the necessary conditions for deadlocks (discussed in the lecture)?

1. Mutual exculion
2. Hold and wait
3. No preemption
4. Curcular wait
   <br/>
   <br/>

## Why does the initial solution lead to a deadlock (by looking at the deadlock conditions)

Because all of the four deadlock conditions are met.

1. Mutual exculion

Every fork is limited and can only be used by one pilosipher at once.

2. Hold and wait

if the first fork is taken it is not taken until the second fork is taken.

3. No preemption

It is not allowed to use resources (fork) that are in use by another thread.

4. Curcular wait

If every consumer takes the first resources (left fork) then every waits for the right resource (right lock).
<br/>
<br/>

## Switch the order in which philosophers take the fork by using the following scheme: Odd philosophers start with the left fork, while even philosophers start with the right hand. Make sure to use concurrency primitives correctly!

Has been implemented via the const PreventDeadlocks in the Philosophers.cs file.
<br/>
<br/>

## Does this strategy resolve the deadlock and why?

The "Circular wait" will be resolved. It can't ahppen that every philosoper takes only one fork (e.g If p1 takes the right fork p2 cannot pick the right fork).
<br/>
<br/>

## Measure the total time spent in waiting for forks and compare it to the total runtime. Interpret the measurement - Was the result expected?

The following times were measured without artificial sleeps (DelayMs = 0).

Ab hier ich (Thomas S.)

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
