using System.Reflection;
using System.Diagnostics;

namespace ILSpy.PE
{
    static class ProcessorArchUtils
    {
        static Machine cachedMachine = 0;

        public static Machine GetProcessCpuArchitecture()
        {
            if (cachedMachine == 0)
                cachedMachine = GetProcessCpuArchitectureCore();
            return cachedMachine;
        }

        static Machine GetProcessCpuArchitectureCore()
        {
            Assembly runtimeAssembly = typeof(object).Assembly;
            Type? interopRuntimeType = runtimeAssembly.GetType("System.Runtime.InteropServices.RuntimeInformation", throwOnError: false);
            MethodInfo? processArchitectureMethod = interopRuntimeType?.GetMethod("get_ProcessArchitecture", []);
            if (processArchitectureMethod is not null)
            {
                object? result = processArchitectureMethod.Invoke(null, []);
                if (result is not null)
                {
                    if (TryGetArchitecture((int)result, out Machine machine))
                    {
                        return machine;
                    }
                }
            }

            Debug.WriteLine("Couldn't detect CPU arch, assuming x86 or x64");
            return IntPtr.Size == 4 ? Machine.I386 : Machine.AMD64;
        }

        static bool TryGetArchitecture(int architecture, out Machine machine)
        {
            switch (architecture)
            {
                case 0: // Architecture.X86
                    Debug.Assert(IntPtr.Size == 4);
                    machine = Machine.I386;
                    return true;

                case 1: // Architecture.X64
                    Debug.Assert(IntPtr.Size == 8);
                    machine = Machine.AMD64;
                    return true;

                case 2: // Architecture.Arm
                    Debug.Assert(IntPtr.Size == 4);
                    machine = Machine.ARMNT;
                    return true;

                case 3: // Architecture.Arm64
                    Debug.Assert(IntPtr.Size == 8);
                    machine = Machine.ARM64;
                    return true;

                default:
                    Debug.Fail($"Unknown process architecture: {architecture}");
                    machine = 0;
                    return false;
            }
        }
    }
}