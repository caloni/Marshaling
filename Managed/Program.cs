using System.Runtime.InteropServices;
using static Program.Config;

[StructLayout(LayoutKind.Sequential)]
public struct Inner
{
    public int count;
}

[StructLayout(LayoutKind.Sequential)]
public struct Sample
{
    public Inner inner;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public int[] values;
}

namespace Program
{
    public static class Config
    {
        //[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)] // force struct padding to 24 (as in C) instead of 20
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IP
        {
            public uint number;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct CONFIG
        {
            public uint number;
            public byte b;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public IP[] ips;
        }
    }

    public static class NativeMethods
    {
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);

        [DllImport("Unmanaged", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConfiguration(ref CONFIG config, uint bytes);

        [DllImport("Unmanaged", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConfigurationRaw(IntPtr config, uint bytes);

        [DllImport("Unmanaged", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool WireGuardSetConfiguration(IntPtr adapter, IntPtr wireGuardConfig, UInt32 bytes);
    }

    public class Program
    {
        public static void CustomMarshaler()
        {
            Sample sample = new Sample
            {
                inner = new Inner { count = 3 },
                values = new int[] { 10, 20, 30 }
            };

            ICustomMarshaler marshaler = VariableLengthArrayMarshaler.GetInstance("YourNamespace.Sample;inner.count");
            IntPtr nativePtr = marshaler.MarshalManagedToNative(sample);

            // Pass nativePtr to unmanaged code...

            // After usage, clean up
            marshaler.CleanUpNativeData(nativePtr);
        }
        public static void Main(string[] args)
        {
            CustomMarshaler();

            string architecture = IntPtr.Size == 8 ? "x64" : "x86";
            NativeMethods.SetDllDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, architecture));
            var config = new CONFIG()
            {
                number = 0xDEADBEEF,
                b = 0x66,
                ips = new IP[256]
            };
            config.ips[0] = new IP()
            {
                number = 0xFF
            };
            NativeMethods.SetConfiguration(ref config, (uint)Marshal.SizeOf<CONFIG>());

            uint ptrSize = (uint)Marshal.SizeOf(config);
            IntPtr ptr = Marshal.AllocHGlobal((int)ptrSize);
            try
            {
                // Copy the struct to unmanaged memory
                Marshal.StructureToPtr(config, ptr, false);

                // Pass 'ptr' to the native DLL function
                NativeMethods.SetConfigurationRaw(ptr, ptrSize);

                NativeMethods.WireGuardSetConfiguration(ptr, ptr, (uint)Marshal.SizeOf<CONFIG>());
            }
            finally
            {
                // Free the unmanaged memory
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}

