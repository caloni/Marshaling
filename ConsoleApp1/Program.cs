using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using static Program.Config;

namespace Program
{
    public static class Config
    {
        public const byte RESERVED_HEADER_BYTES_LENGTH = 3;
        public const byte KEY_LENGTH = 32;

        [Flags]
        public enum WORKPLACE_GATEWAY_INTERFACE_FLAG : uint
        {
            WORKPLACE_GATEWAY_INTERFACE_HAS_PUBLIC_KEY = 1 << 0,
            WORKPLACE_GATEWAY_INTERFACE_HAS_PRIVATE_KEY = 1 << 1,
            WORKPLACE_GATEWAY_INTERFACE_HAS_LISTEN_PORT = 1 << 2,
            WORKPLACE_GATEWAY_INTERFACE_REPLACE_PEERS = 1 << 3
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct INTERFACE
        {
            public WORKPLACE_GATEWAY_INTERFACE_FLAG Flags;
            public ushort ListenPort;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = KEY_LENGTH)]
            public byte[] PrivateKey;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = KEY_LENGTH)]
            public byte[] PublicKey;

            public uint PeersCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RESERVED_HEADER_BYTES_LENGTH)]
            public byte[] ReservedHeaderBytes;
        }

        [Flags]
        public enum WORKPLACE_GATEWAY_PEER_FLAG : uint
        {
            WORKPLACE_GATEWAY_PEER_HAS_PUBLIC_KEY = 1 << 0,
            WORKPLACE_GATEWAY_PEER_HAS_PRESHARED_KEY = 1 << 1,
            WORKPLACE_GATEWAY_PEER_HAS_PERSISTENT_KEEPALIVE = 1 << 2,
            WORKPLACE_GATEWAY_PEER_HAS_ENDPOINT = 1 << 3,
            WORKPLACE_GATEWAY_PEER_REPLACE_ALLOWED_IPS = 1 << 5,
            WORKPLACE_GATEWAY_PEER_REMOVE = 1 << 6,
            WORKPLACE_GATEWAY_PEER_UPDATE_ONLY = 1 << 7
        }

        public enum ADDRESS_FAMILY : ushort
        {
            /// <summary>Unspecified address family.</summary>
            AF_UNSPEC = 0,

            /// <summary>Unix local to host address.</summary>
            AF_UNIX = 1,

            /// <summary>Address for IP version 4.</summary>
            AF_INET = 2,

            /// <summary>ARPANET IMP address.</summary>
            AF_IMPLINK = 3,

            /// <summary>Address for PUP protocols.</summary>
            AF_PUP = 4,

            /// <summary>Address for MIT CHAOS protocols.</summary>
            AF_CHAOS = 5,

            /// <summary>Address for Xerox NS protocols.</summary>
            AF_NS = 6,

            /// <summary>IPX or SPX address.</summary>
            AF_IPX = AF_NS,

            /// <summary>Address for ISO protocols.</summary>
            AF_ISO = 7,

            /// <summary>Address for OSI protocols.</summary>
            AF_OSI = AF_ISO,

            /// <summary>European Computer Manufacturers Association (ECMA) address.</summary>
            AF_ECMA = 8,

            /// <summary>Address for Datakit protocols.</summary>
            AF_DATAKIT = 9,

            /// <summary>Addresses for CCITT protocols, such as X.25.</summary>
            AF_CCITT = 10,

            /// <summary>IBM SNA address.</summary>
            AF_SNA = 11,

            /// <summary>DECnet address.</summary>
            AF_DECnet = 12,

            /// <summary>Direct data-link interface address.</summary>
            AF_DLI = 13,

            /// <summary>LAT address.</summary>
            AF_LAT = 14,

            /// <summary>NSC Hyperchannel address.</summary>
            AF_HYLINK = 15,

            /// <summary>AppleTalk address.</summary>
            AF_APPLETALK = 16,

            /// <summary>NetBios address.</summary>
            AF_NETBIOS = 17,

            /// <summary>VoiceView address.</summary>
            AF_VOICEVIEW = 18,

            /// <summary>FireFox address.</summary>
            AF_FIREFOX = 19,

            /// <summary>Undocumented.</summary>
            AF_UNKNOWN1 = 20,

            /// <summary>Banyan address.</summary>
            AF_BAN = 21,

            /// <summary>Native ATM services address.</summary>
            AF_ATM = 22,

            /// <summary>Address for IP version 6.</summary>
            AF_INET6 = 23,

            /// <summary>Address for Microsoft cluster products.</summary>
            AF_CLUSTER = 24,

            /// <summary>IEEE 1284.4 workgroup address.</summary>
            AF_12844 = 25,

            /// <summary>IrDA address.</summary>
            AF_IRDA = 26,

            /// <summary>Address for Network Designers OSI gateway-enabled protocols.</summary>
            AF_NETDES = 28,

            /// <summary>Undocumented.</summary>
            AF_TCNPROCESS = 29,

            /// <summary>Undocumented.</summary>
            AF_TCNMESSAGE = 30,

            /// <summary>Undocumented.</summary>
            AF_ICLFXBM = 31,

            /// <summary>Bluetooth RFCOMM/L2CAP protocols.</summary>
            AF_BTH = 32,

            /// <summary>Link layer interface.</summary>
            AF_LINK = 33,

            /// <summary>Windows Hyper-V.</summary>
            AF_HYPERV = 34,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InAddr
        {
            public InAddress S_un;

            [StructLayout(LayoutKind.Explicit)]
            public struct InAddress
            {
                [FieldOffset(0)]
                public BytePart S_un_b;

                [FieldOffset(0)]
                public ShortPart S_un_w;

                [FieldOffset(0)]
                public uint S_addr;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct BytePart
            {
                public byte s_b1;
                public byte s_b2;
                public byte s_b3;
                public byte s_b4;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct ShortPart
            {
                public ushort s_w1;
                public ushort s_w2;
            }

            public InAddr(string ipv4Address) : this(IPAddress.Parse(ipv4Address)) { }

            public InAddr(IPAddress address) : this(address.GetAddressBytes()) { }

            public InAddr(uint ipv4Address) : this(new IPAddress(ipv4Address)) { }

            public InAddr(byte[] address)
            {
                // Set this first, otherwise it wipes out the other fields
                S_un.S_un_w = new ShortPart();

                S_un.S_addr = (uint)BitConverter.ToInt32(address, 0);

                S_un.S_un_b.s_b1 = address[0];
                S_un.S_un_b.s_b2 = address[1];
                S_un.S_un_b.s_b3 = address[2];
                S_un.S_un_b.s_b4 = address[3];
            }

            /// <summary>
            /// Unpacks an in_addr struct to an IPAddress object
            /// </summary>
            /// <returns></returns>
            public IPAddress ToIpAddress()
            {
                var bytes = new[] {
                    S_un.S_un_b.s_b1,
                    S_un.S_un_b.s_b2,
                    S_un.S_un_b.s_b3,
                    S_un.S_un_b.s_b4
                };

                return new IPAddress(bytes);
            }

            public override string ToString()
            {
                return ToIpAddress().ToString();
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SOCKADDR_IN
        {
            public ADDRESS_FAMILY sin_family;
            public ushort sin_port;

            public InAddr sin_addr;

            public ulong sin_zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InAddr6
        {
            public InAddress6 u;

            [StructLayout(LayoutKind.Sequential)]
            public struct Byte
            {
                public byte sin6_addr0;
                public byte sin6_addr1;
                public byte sin6_addr2;
                public byte sin6_addr3;
                public byte sin6_addr4;
                public byte sin6_addr5;
                public byte sin6_addr6;
                public byte sin6_addr7;
                public byte sin6_addr8;
                public byte sin6_addr9;
                public byte sin6_addr10;
                public byte sin6_addr11;
                public byte sin6_addr12;
                public byte sin6_addr13;
                public byte sin6_addr14;
                public byte sin6_addr15;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Word
            {
                public ushort sin6_addr0;
                public ushort sin6_addr1;
                public ushort sin6_addr2;
                public ushort sin6_addr3;
                public ushort sin6_addr4;
                public ushort sin6_addr5;
                public ushort sin6_addr6;
                public ushort sin6_addr7;
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct InAddress6
            {
                [FieldOffset(0)]
                public Byte Byte;

                [FieldOffset(0)]
                public Word Word;
            }

            public byte[] AddressRaw
            {
                get
                {
                    return new byte[]
                    {
                        u.Byte.sin6_addr0, u.Byte.sin6_addr1, u.Byte.sin6_addr2, u.Byte.sin6_addr3,
                        u.Byte.sin6_addr4, u.Byte.sin6_addr5, u.Byte.sin6_addr6, u.Byte.sin6_addr7,
                        u.Byte.sin6_addr8, u.Byte.sin6_addr9, u.Byte.sin6_addr10, u.Byte.sin6_addr11,
                        u.Byte.sin6_addr12, u.Byte.sin6_addr13, u.Byte.sin6_addr14, u.Byte.sin6_addr15
                    };
                }
            }

            public IPAddress Address
            {
                get => new IPAddress(AddressRaw);
                set
                {
                    u.Byte.sin6_addr0 = value.GetAddressBytes()[0];
                    u.Byte.sin6_addr1 = value.GetAddressBytes()[1];
                    u.Byte.sin6_addr2 = value.GetAddressBytes()[2];
                    u.Byte.sin6_addr3 = value.GetAddressBytes()[3];
                    u.Byte.sin6_addr4 = value.GetAddressBytes()[4];
                    u.Byte.sin6_addr5 = value.GetAddressBytes()[5];
                    u.Byte.sin6_addr6 = value.GetAddressBytes()[6];
                    u.Byte.sin6_addr7 = value.GetAddressBytes()[7];
                    u.Byte.sin6_addr8 = value.GetAddressBytes()[8];
                    u.Byte.sin6_addr9 = value.GetAddressBytes()[9];
                    u.Byte.sin6_addr10 = value.GetAddressBytes()[10];
                    u.Byte.sin6_addr11 = value.GetAddressBytes()[11];
                    u.Byte.sin6_addr12 = value.GetAddressBytes()[12];
                    u.Byte.sin6_addr13 = value.GetAddressBytes()[13];
                    u.Byte.sin6_addr14 = value.GetAddressBytes()[14];
                    u.Byte.sin6_addr15 = value.GetAddressBytes()[15];
                }
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SOCKADDR_IN6
        {
            public ADDRESS_FAMILY sin6_family;
            public ushort sin6_port;
            public uint sin6_flowinfo;

            public InAddr6 sin6_addr;

            public uint sin6_scope_id;
        }



        [StructLayout(LayoutKind.Explicit)]
        public struct SOCKADDR_INET
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.Struct)]
            public SOCKADDR_IN Ipv4;

            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.Struct)]
            public SOCKADDR_IN6 Ipv6;

            [FieldOffset(0)]
            public ADDRESS_FAMILY si_family;

            public override string ToString()
            {
                if (si_family == ADDRESS_FAMILY.AF_INET)
                {
                    return Ipv4.sin_addr.ToIpAddress().ToString();
                }
                else if (si_family == ADDRESS_FAMILY.AF_INET6)
                {
                    return Ipv6.sin6_addr.Address.ToString();
                }

                return "";
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PEER
        {
            public WORKPLACE_GATEWAY_PEER_FLAG Flags;
            public uint Reserved;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = KEY_LENGTH)]
            public byte[] PublicKey;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = KEY_LENGTH)]
            public byte[] PresharedKey;

            public ushort PersistentKeepalive;
            public SOCKADDR_INET Endpoint;
            public ulong TxBytes;
            public ulong RxBytes;
            public ulong LastHandshake;
            public uint AllowedIPsCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)] // force struct padding to 24 (as in C) instead of 20
        public struct IP
        {
            [StructLayout(LayoutKind.Explicit)]
            public struct Address
            {
                [FieldOffset(0)] public InAddr In4Addr;
                [FieldOffset(0)] public InAddr6 In6Addr;
            }

            public Address address;
            public ADDRESS_FAMILY AddressFamily;
            public byte Cidr;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct CONFIG
        {
            public INTERFACE Interface;
            public PEER Peer;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public IP[] ips;
        }
    }

    public static class Lib
    {
        [DllImport("Dll1.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConfiguration(ref CONFIG config, uint bytes);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new CONFIG();
            Lib.SetConfiguration(ref config, (uint)Marshal.SizeOf<CONFIG>());
        }
    }
}

