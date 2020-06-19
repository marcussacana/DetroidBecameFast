using System;

namespace DetroidBecameFast
{
    enum ThreadPriority : int
    {
        THREAD_MODE_BACKGROUND_BEGIN = 0x00010000,
        THREAD_MODE_BACKGROUND_END = 0x00020000,
        ABOVE_NORMAL = 1,
        BELOW_NORMAL = -1,
        HIGHEST = 2,
        IDLE = -15,
        LOWEST = -2,
        NORMAL = 0,
        TIME_CRITICAL = 15
    }

    unsafe delegate bool dSetThreadPriority(void* hThread, ThreadPriority Priority);
    unsafe class SetThreadPriority : Hook.Base.Hook<dSetThreadPriority>
    {
        public override string Library => "kernelbase.dll";

        public override string Export => "SetThreadPriority";

        public override void Initialize()
        {
            HookDelegate = new dSetThreadPriority(hSetThreadPriority);
            Compile();
        }

        public Func<ThreadPriority, ThreadPriority> OnThreadPriorityChanged;

        public bool hSetThreadPriority(void* hThread, ThreadPriority Priority)
        {
            return Bypass(hThread, OnThreadPriorityChanged?.Invoke(Priority) ?? Priority);
        }
    }
}
