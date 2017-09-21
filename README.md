This is an implementation of a priority queue and a stable priority queue in C#.

I wrote this because I'm working on a Unity project that uses calendar scheduling and I figured it would be best to write a generic PQueue implementation and then use that to implment the calendar.

If you're wondering what I mean by calendar, it's an alternative to real-time progression. Instead of ticking off individual seconds until it's time for something to happen, a calendar system places events in a queue with a timestamp indicating when they should occur. The events are sorted in timestamp order and the scheduler sets "now" to be equal to the timestamp on the next event, triggers that event, then removes it from the queue and proceeds to the next one in the same fashion. The game I'm working on is a TRPG, similar to Final Fantasy Tactics, so this kind of timing system fits well.

In any case, I'm more or less satisfied with this queue for the moment. I haven't really seriously put it through its paces yet and it's nothing like optimized, but I don't expect any problems from it. I've included a small test driver, but if there are any bugs I expect them to pop up readily as I use the queue in the game project, since it will be exercising it quite well and any issues should become obvious pretty quickly.

If you spot any bugs or have any suggestions please create a comment or pull request.

Cheers.
