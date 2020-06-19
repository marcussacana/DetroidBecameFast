using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DetroidBecameFast
{
    /// <summary>
    /// This is a wrapper to the libScePad.dll
    /// </summary>
    public unsafe static class libScePad
    {
        static string CurrentDllName => Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        static string CurrentDllPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void* RealHandler;
        public static void* WrapperHandler;
        static libScePad()
        {
            Main.Initialize();

            WrapperHandler = LoadLibraryW(CurrentDllName);
            RealHandler = LoadLibrary(CurrentDllName);

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            dscePadIsSupportedAudioFunction = GetDelegate<tscePadIsSupportedAudioFunction>(RealHandler, "scePadIsSupportedAudioFunction", false);
            dscePadSetAudioOutPath = GetDelegate<tscePadSetAudioOutPath>(RealHandler, "scePadSetAudioOutPath", false);
            dscePadSetVolumeGain = GetDelegate<tscePadSetVolumeGain>(RealHandler, "scePadSetVolumeGain", false);
            dscePadGetJackState = GetDelegate<tscePadGetJackState>(RealHandler, "scePadGetJackState", false);
            dscePadInit = GetDelegate<tscePadInit>(RealHandler, "scePadInit", false);
            dscePadOpen = GetDelegate<tscePadOpen>(RealHandler, "scePadOpen", false);
            dscePadClose = GetDelegate<tscePadClose>(RealHandler, "scePadClose", false);
            dscePadReadState = GetDelegate<tscePadReadState>(RealHandler, "scePadReadState", false);
            dscePadRead = GetDelegate<tscePadRead>(RealHandler, "scePadRead", false);
            dscePadResetOrientation = GetDelegate<tscePadResetOrientation>(RealHandler, "scePadResetOrientation", false);
            dscePadSetAngularVelocityDeadbandState = GetDelegate<tscePadSetAngularVelocityDeadbandState>(RealHandler, "scePadSetAngularVelocityDeadbandState", false);
            dscePadSetMotionSensorState = GetDelegate<tscePadSetMotionSensorState>(RealHandler, "scePadSetMotionSensorState", false);
            dscePadSetTiltCorrectionState = GetDelegate<tscePadSetTiltCorrectionState>(RealHandler, "scePadSetTiltCorrectionState", false);
            dscePadSetVibration = GetDelegate<tscePadSetVibration>(RealHandler, "scePadSetVibration", false);
            dscePadSetLightBar = GetDelegate<tscePadSetLightBar>(RealHandler, "scePadSetLightBar", false);
            dscePadResetLightBar = GetDelegate<tscePadResetLightBar>(RealHandler, "scePadResetLightBar", false);
            dscePadGetControllerInformation = GetDelegate<tscePadGetControllerInformation>(RealHandler, "scePadGetControllerInformation", false);
            dscePadSetParticularMode = GetDelegate<tscePadSetParticularMode>(RealHandler, "scePadSetParticularMode", false);
            dscePadGetParticularMode = GetDelegate<tscePadGetParticularMode>(RealHandler, "scePadGetParticularMode", false);


            BypassWrapper();
        }

        static List<string> ImportNameList = new List<string>();
        unsafe static void BypassWrapper()
        {
            if (RealHandler != null)
            {
                foreach (var ImportName in ImportNameList)
                {
                    var WProc = Hook.Base.Extensions.GetProcAddress(WrapperHandler, ImportName);
                    var RProc = Hook.Base.Extensions.GetProcAddress(RealHandler, ImportName);

                    if (WProc == null || RProc == null)
                        continue;

                    byte[] Jmp = AssembleJump(RProc);
                    Write(WProc, Jmp);
                }
            }
        }

        static byte[] AssembleJump(void* Destination)
        {
            const int JmpSize = 12;
            byte[] jmp = new byte[JmpSize];

            new byte[] { 0x48, 0xb8 }.CopyTo(jmp, 0);
            BitConverter.GetBytes((ulong)Destination).CopyTo(jmp, 2);
            new byte[] { 0xFF, 0xE0 }.CopyTo(jmp, 10);

            return jmp;
        }

        unsafe static void Write(void* Address, byte[] Content)
        {
            uint Saved = (uint)Content.LongLength;
            Hook.Base.Extensions.DeprotectMemory(Address, Saved);
            Marshal.Copy(Content, 0, new IntPtr(Address), Content.Length);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetAudioOutPath(uint a1, int a2)
        {
            return dscePadSetAudioOutPath(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetVolumeGain(uint a1, uint a2)
        {
            return dscePadSetVolumeGain(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadGetJackState(uint a1, uint a2)
        {
            return dscePadGetJackState(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static uint scePadInit()
        {
            return dscePadInit();
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadOpen(uint a1, uint a2, uint a3)
        {
            return dscePadOpen(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadClose(uint a1)
        {
            return dscePadClose(a1);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadReadState(uint a1, uint a2)
        {
            return dscePadReadState(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadRead(uint a1, uint a2, int a3)
        {
            return dscePadRead(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadResetOrientation(uint a1)
        {
            return dscePadResetOrientation(a1);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetAngularVelocityDeadbandState(uint a1, IntPtr a2)
        {
            return dscePadSetAngularVelocityDeadbandState(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetMotionSensorState(uint a1, IntPtr a2)
        {
            return dscePadSetMotionSensorState(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetTiltCorrectionState(uint a1, IntPtr a2)
        {
            return dscePadSetTiltCorrectionState(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetVibration(uint a1, IntPtr a2)
        {
            return dscePadSetVibration(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetLightBar(uint a1, IntPtr a2)
        {
            return dscePadSetLightBar(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadResetLightBar(uint a1)
        {
            return dscePadResetLightBar(a1);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadGetControllerInformation(uint a1, uint a2)
        {
            return dscePadGetControllerInformation(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.FastCall)]
        public static uint scePadSetParticularMode(IntPtr a1)
        {
            return dscePadSetParticularMode(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static uint scePadGetParticularMode()
        {
            return dscePadGetParticularMode();
        }


        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern void* GetProcAddress(void* hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern void* LoadLibraryW(string lpFileName);

        internal static void* LoadLibrary(string lpFileName)
        {
            string DllPath = lpFileName;
            if (lpFileName.Length < 2 || lpFileName[1] != ':')
            {
                string DLL = Path.GetFileNameWithoutExtension(lpFileName);
                DllPath = Path.Combine(Environment.CurrentDirectory, $"{DLL}_ori.dll");
                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(Environment.CurrentDirectory, $"{DLL}.dll");
                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(CurrentDllPath, $"{DLL}_ori.dll");
                if (!File.Exists(DllPath) && CurrentDllName != lpFileName.ToLower())
                    DllPath = Path.Combine(CurrentDllPath, $"{DLL}.dll.ori");
                if (!File.Exists(DllPath))
                {
                    DllPath = Environment.SystemDirectory;
                    DllPath = Path.Combine(DllPath, $"{DLL}.dll");
                }
            }

            void* Handler = LoadLibraryW(DllPath);

            if (Handler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            return Handler;
        }

        internal static T GetDelegate<T>(void* Handler, string Function, bool Optional = true) where T : Delegate
        {
            void* Address = GetProcAddress(Handler, Function);
            if (Address == null)
            {
                if (Optional)
                {
                    return null;
                }
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED
            }
            ImportNameList.Add(Function);
            return (T)Marshal.GetDelegateForFunctionPointer(new IntPtr(Address), typeof(T));
        }

        static tscePadIsSupportedAudioFunction dscePadIsSupportedAudioFunction;
        static tscePadSetAudioOutPath dscePadSetAudioOutPath;
        static tscePadSetVolumeGain dscePadSetVolumeGain;
        static tscePadGetJackState dscePadGetJackState;
        static tscePadInit dscePadInit;
        static tscePadOpen dscePadOpen;
        static tscePadClose dscePadClose;
        static tscePadReadState dscePadReadState;
        static tscePadRead dscePadRead;
        static tscePadResetOrientation dscePadResetOrientation;
        static tscePadSetAngularVelocityDeadbandState dscePadSetAngularVelocityDeadbandState;
        static tscePadSetMotionSensorState dscePadSetMotionSensorState;
        static tscePadSetTiltCorrectionState dscePadSetTiltCorrectionState;
        static tscePadSetVibration dscePadSetVibration;
        static tscePadSetLightBar dscePadSetLightBar;
        static tscePadResetLightBar dscePadResetLightBar;
        static tscePadGetControllerInformation dscePadGetControllerInformation;
        static tscePadSetParticularMode dscePadSetParticularMode;
        static tscePadGetParticularMode dscePadGetParticularMode;

        delegate uint tscePadIsSupportedAudioFunction(uint a1);
        delegate uint tscePadSetAudioOutPath(uint a1, int a2);
        delegate uint tscePadSetVolumeGain(uint a1, uint a2);
        delegate uint tscePadGetJackState(uint a1, uint a2);
        delegate uint tscePadInit();
        delegate uint tscePadOpen(uint a1, uint a2, uint a3);
        delegate uint tscePadClose(uint a1);
        delegate uint tscePadReadState(uint a1, uint a2);
        delegate uint tscePadRead(uint a1, uint a2, int a3);
        delegate uint tscePadResetOrientation(uint a1);
        delegate uint tscePadSetAngularVelocityDeadbandState(uint a1, IntPtr a2);
        delegate uint tscePadSetMotionSensorState(uint a1, IntPtr a2);
        delegate uint tscePadSetTiltCorrectionState(uint a1, IntPtr a2);
        delegate uint tscePadSetVibration(uint a1, IntPtr a2);
        delegate uint tscePadSetLightBar(uint a1, IntPtr a2);
        delegate uint tscePadResetLightBar(uint a1);
        delegate uint tscePadGetControllerInformation(uint a1, uint a2);
        delegate uint tscePadSetParticularMode(IntPtr a1);
        delegate uint tscePadGetParticularMode();

    }
}
