using System;
using System.IO;

namespace DetroidBecameFast
{
    static class Main
    {
#if DEBUG
        static int Changes = 0;
        public static StreamWriter LOG = File.CreateText("Threads.log");
#endif
        static SetThreadPriority ThreadPriorityHook;
        public static void Initialize()
        {
#if DEBUG
            try
            {
#endif
                ThreadPriorityHook = new SetThreadPriority();
                ThreadPriorityHook.OnThreadPriorityChanged = OnThreadPriorityChanged;
                ThreadPriorityHook.Install();
#if DEBUG
            }
            catch (Exception ex)
            {
                LOG.WriteLine(ex);
                LOG.Flush();
            }
#endif
        }

        public static ThreadPriority OnThreadPriorityChanged(ThreadPriority Priority)
        {
#if DEBUG
            LOG.WriteLine($"{Changes++}: {Priority}");
            LOG.Flush();
#endif

            return Priority switch
            {
                ThreadPriority.TIME_CRITICAL => ThreadPriority.ABOVE_NORMAL,
                ThreadPriority.HIGHEST => ThreadPriority.ABOVE_NORMAL,
                _ => Priority
            };
        }
    }
}
