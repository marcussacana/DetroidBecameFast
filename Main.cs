using NvAPIWrapper;
using NvAPIWrapper.DRS;
using System;
using System.Diagnostics;
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

                var NVPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "nvapi64.dll");
                if (File.Exists(NVPath))
                {
                    NVIDIA.Initialize();
                    using (var Session = DriverSettingsSession.CreateAndLoad())
                    {
                        var Application = Session.FindApplication(Process.GetCurrentProcess().MainModule.FileName);
                        if (Application != null)
                        {
                            Application.Profile.SetSetting(KnownSettingId.OpenGLThreadControl, 1);
                            Session.Save();
                        }
#if DEBUG
                        else
                        {
                            LOG.WriteLine("Failed to Find the Application Profile");
                            LOG.Flush();
                        }
#endif
                    }
                    NVIDIA.Unload();
                }
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
                ThreadPriority.TIME_CRITICAL => ThreadPriority.HIGHEST,
                //ThreadPriority.HIGHEST => ThreadPriority.ABOVE_NORMAL,
                _ => Priority
            };
        }
    }
}
