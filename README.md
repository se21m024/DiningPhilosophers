## What are the necessary conditions for deadlocks (discussed in the lecture) [0.5 points]?

1. Mutual exculion

Every fork is limited and can only be used by one pilosipher at once.

2. Hold and wait

if the first fork is taken it is not taken until the second fork is taken.

3. No preemption

It is not allowed to use resources (fork) that are in use by another thread.

4. Curcular wait

If every consumer takes the first resources (left fork) then every waits for the right resource (right lock).

## Switch the order in which philosophers take the fork by using the following scheme: Odd philoso- phers start with the left fork, while even philosophers start with the right hand [6 points]. Make sure to use concurrency primitives correctly!

Has been implemented via the production const in the Philosophers.cs file (see line 11).

## Does this strategy resolve the deadlock and why [1 point]?

The "Curcular wait" will be resolved. It can't ahppen that every philosoper takes only one fork (e.g If p1 takes the right fork p2 cannot pick the right fork).

## Measure the total time spent in waiting for forks and compare it to the total runtime. Interpret the measurement - Was the result expected? [3 points].

5 Philosophers
Thinkingtime: 100 ms
Eatingtime: 99 ms

(Without sleep time)

Different input parameters do not affect the ratio of waitingTime and totalTime.

ToDo.... Insert Table with data.

# Can you think of other techniques for deadlock prevention?

## Option 1

Use a timeout for accessing forks. Provides other philosophers with the opportnety to access the fork during the on purpose timeout.

## Option 2

Use mediator that coordinates the access to the forks. The mediator assigns the forks pairwise. When using a mediator the use of a hold and wait condition is not neccessary any more.

## Option 3

Prevent all philosophers to access the forks at once (e.g. One philosopher must not access the for while all other philosophers are allowed to access the forks.)

# Make sure to always shutdown the program cooperatively and to always cleanup all allocated resources.

<!-- A CancellationToken was used to ensure that the program can be gracefully shut down. When shutting down the program the philosophers have time to finish eating and putting back the forks or cancel the waiting if they are waiting for a fork. -->

A CancellationToken was used to ensure that the program could be terminated properly. When the program shuts down, the philosophers have time to finish eating and reset the forks or cancel the wait if they are waiting for a fork.
