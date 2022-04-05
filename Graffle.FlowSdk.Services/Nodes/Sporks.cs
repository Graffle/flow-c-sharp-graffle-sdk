namespace Graffle.FlowSdk.Services.Nodes
{
    public sealed class Spork
    {
        internal Spork(string name, string node, ulong rootHeight, ulong? maxHeight, bool isTestNet = false, bool isEmulator = false)
        {
            Name = name;
            Node = node;
            RootHeight = rootHeight;
            MaxHeight = maxHeight;
            IsTestNet = isTestNet;
            IsEmulator = isEmulator;
        }

        public string Name { get; }
        public string Node { get; }
        public ulong RootHeight { get; }
        public ulong? MaxHeight { get; }
        public bool IsTestNet { get; }
        public bool IsEmulator { get; }
    }
    public static class Sporks
    {

        public static Spork GetSporkByName(string sporkName)
        {
            return sporkName switch
            {
                _ when sporkName == TestNetSporks.TestNet.Name => new Spork(TestNetSporks.TestNet.Name, TestNetSporks.TestNet.NODE, TestNetSporks.TestNet.ROOT_HEIGHT, null, true),

                _ when sporkName == TestNetSporks.TestNet33.Name => new Spork(TestNetSporks.TestNet33.Name, TestNetSporks.TestNet33.NODE, TestNetSporks.TestNet33.ROOT_HEIGHT, TestNetSporks.TestNet.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet32.Name => new Spork(TestNetSporks.TestNet32.Name, TestNetSporks.TestNet32.NODE, TestNetSporks.TestNet32.ROOT_HEIGHT, TestNetSporks.TestNet33.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet31.Name => new Spork(TestNetSporks.TestNet31.Name, TestNetSporks.TestNet31.NODE, TestNetSporks.TestNet31.ROOT_HEIGHT, TestNetSporks.TestNet32.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet30.Name => new Spork(TestNetSporks.TestNet30.Name, TestNetSporks.TestNet30.NODE, TestNetSporks.TestNet30.ROOT_HEIGHT, TestNetSporks.TestNet31.ROOT_HEIGHT - 1, true),

                _ when sporkName == TestNetSporks.TestNet29.Name => new Spork(TestNetSporks.TestNet29.Name, TestNetSporks.TestNet29.NODE, TestNetSporks.TestNet29.ROOT_HEIGHT, TestNetSporks.TestNet30.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet28.Name => new Spork(TestNetSporks.TestNet28.Name, TestNetSporks.TestNet28.NODE, TestNetSporks.TestNet28.ROOT_HEIGHT, TestNetSporks.TestNet29.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet27.Name => new Spork(TestNetSporks.TestNet27.Name, TestNetSporks.TestNet27.NODE, TestNetSporks.TestNet27.ROOT_HEIGHT, TestNetSporks.TestNet28.ROOT_HEIGHT - 1, true),

                _ when sporkName == TestNetSporks.TestNet26.Name => new Spork(TestNetSporks.TestNet26.Name, TestNetSporks.TestNet26.NODE, TestNetSporks.TestNet26.ROOT_HEIGHT, TestNetSporks.TestNet27.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet25.Name => new Spork(TestNetSporks.TestNet25.Name, TestNetSporks.TestNet25.NODE, TestNetSporks.TestNet25.ROOT_HEIGHT, TestNetSporks.TestNet26.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet24.Name => new Spork(TestNetSporks.TestNet24.Name, TestNetSporks.TestNet24.NODE, TestNetSporks.TestNet24.ROOT_HEIGHT, TestNetSporks.TestNet25.ROOT_HEIGHT - 1, true),

                _ when sporkName == TestNetSporks.TestNet23.Name => new Spork(TestNetSporks.TestNet23.Name, TestNetSporks.TestNet23.NODE, TestNetSporks.TestNet23.ROOT_HEIGHT, TestNetSporks.TestNet24.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet22.Name => new Spork(TestNetSporks.TestNet22.Name, TestNetSporks.TestNet22.NODE, TestNetSporks.TestNet22.ROOT_HEIGHT, TestNetSporks.TestNet23.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet21.Name => new Spork(TestNetSporks.TestNet21.Name, TestNetSporks.TestNet21.NODE, TestNetSporks.TestNet21.ROOT_HEIGHT, TestNetSporks.TestNet22.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet20.Name => new Spork(TestNetSporks.TestNet20.Name, TestNetSporks.TestNet20.NODE, TestNetSporks.TestNet20.ROOT_HEIGHT, TestNetSporks.TestNet21.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet19.Name => new Spork(TestNetSporks.TestNet19.Name, TestNetSporks.TestNet19.NODE, TestNetSporks.TestNet19.ROOT_HEIGHT, TestNetSporks.TestNet20.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet18.Name => new Spork(TestNetSporks.TestNet18.Name, TestNetSporks.TestNet18.NODE, TestNetSporks.TestNet18.ROOT_HEIGHT, TestNetSporks.TestNet19.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet17.Name => new Spork(TestNetSporks.TestNet17.Name, TestNetSporks.TestNet17.NODE, TestNetSporks.TestNet17.ROOT_HEIGHT, TestNetSporks.TestNet18.ROOT_HEIGHT - 1, true),

                _ when sporkName == MainNetSporks.MainNet.Name => new Spork(MainNetSporks.MainNet.Name, MainNetSporks.MainNet.NODE, MainNetSporks.MainNet.ROOT_HEIGHT, null),
                _ when sporkName == MainNetSporks.MainNet15.Name => new Spork(MainNetSporks.MainNet15.Name, MainNetSporks.MainNet15.NODE, MainNetSporks.MainNet15.ROOT_HEIGHT, MainNetSporks.MainNet.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet14.Name => new Spork(MainNetSporks.MainNet14.Name, MainNetSporks.MainNet14.NODE, MainNetSporks.MainNet14.ROOT_HEIGHT, MainNetSporks.MainNet15.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet13.Name => new Spork(MainNetSporks.MainNet13.Name, MainNetSporks.MainNet13.NODE, MainNetSporks.MainNet13.ROOT_HEIGHT, MainNetSporks.MainNet14.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet12.Name => new Spork(MainNetSporks.MainNet12.Name, MainNetSporks.MainNet12.NODE, MainNetSporks.MainNet12.ROOT_HEIGHT, MainNetSporks.MainNet13.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet11.Name => new Spork(MainNetSporks.MainNet11.Name, MainNetSporks.MainNet11.NODE, MainNetSporks.MainNet11.ROOT_HEIGHT, MainNetSporks.MainNet12.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet10.Name => new Spork(MainNetSporks.MainNet10.Name, MainNetSporks.MainNet10.NODE, MainNetSporks.MainNet10.ROOT_HEIGHT, MainNetSporks.MainNet11.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet9.Name => new Spork(MainNetSporks.MainNet9.Name, MainNetSporks.MainNet9.NODE, MainNetSporks.MainNet9.ROOT_HEIGHT, MainNetSporks.MainNet10.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet8.Name => new Spork(MainNetSporks.MainNet8.Name, MainNetSporks.MainNet8.NODE, MainNetSporks.MainNet8.ROOT_HEIGHT, MainNetSporks.MainNet9.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet7.Name => new Spork(MainNetSporks.MainNet7.Name, MainNetSporks.MainNet7.NODE, MainNetSporks.MainNet7.ROOT_HEIGHT, MainNetSporks.MainNet8.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet6.Name => new Spork(MainNetSporks.MainNet6.Name, MainNetSporks.MainNet6.NODE, MainNetSporks.MainNet6.ROOT_HEIGHT, MainNetSporks.MainNet7.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet5.Name => new Spork(MainNetSporks.MainNet5.Name, MainNetSporks.MainNet5.NODE, MainNetSporks.MainNet5.ROOT_HEIGHT, MainNetSporks.MainNet6.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet4.Name => new Spork(MainNetSporks.MainNet4.Name, MainNetSporks.MainNet4.NODE, MainNetSporks.MainNet4.ROOT_HEIGHT, MainNetSporks.MainNet5.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet3.Name => new Spork(MainNetSporks.MainNet3.Name, MainNetSporks.MainNet3.NODE, MainNetSporks.MainNet3.ROOT_HEIGHT, MainNetSporks.MainNet4.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet2.Name => new Spork(MainNetSporks.MainNet2.Name, MainNetSporks.MainNet2.NODE, MainNetSporks.MainNet2.ROOT_HEIGHT, MainNetSporks.MainNet3.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet1.Name => new Spork(MainNetSporks.MainNet1.Name, MainNetSporks.MainNet1.NODE, MainNetSporks.MainNet1.ROOT_HEIGHT, MainNetSporks.MainNet2.ROOT_HEIGHT - 1),

                _ when sporkName == EmulatorSporks.Emulator.Name => new Spork(EmulatorSporks.Emulator.Name, EmulatorSporks.Emulator.NODE, EmulatorSporks.Emulator.ROOT_HEIGHT, null, false, true),
                _ => throw new System.NotSupportedException("Spork not supported")
            };
        }

        public static Spork GetDevSporkByHeight(ulong blockHeight)
        {
            return blockHeight switch
            {
                _ when blockHeight >= TestNetSporks.TestNet.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet.Name, TestNetSporks.TestNet.NODE, TestNetSporks.TestNet.ROOT_HEIGHT, null, true),

                _ when blockHeight >= TestNetSporks.TestNet32.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet32.Name, TestNetSporks.TestNet32.NODE, TestNetSporks.TestNet32.ROOT_HEIGHT, TestNetSporks.TestNet.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet31.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet32.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet31.Name, TestNetSporks.TestNet31.NODE, TestNetSporks.TestNet31.ROOT_HEIGHT, TestNetSporks.TestNet32.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet30.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet31.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet30.Name, TestNetSporks.TestNet30.NODE, TestNetSporks.TestNet30.ROOT_HEIGHT, TestNetSporks.TestNet31.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet29.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet30.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet29.Name, TestNetSporks.TestNet29.NODE, TestNetSporks.TestNet29.ROOT_HEIGHT, TestNetSporks.TestNet30.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet28.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet29.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet28.Name, TestNetSporks.TestNet28.NODE, TestNetSporks.TestNet28.ROOT_HEIGHT, TestNetSporks.TestNet29.ROOT_HEIGHT - 1, true),

                _ when blockHeight >= TestNetSporks.TestNet27.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet28.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet27.Name, TestNetSporks.TestNet27.NODE, TestNetSporks.TestNet27.ROOT_HEIGHT, TestNetSporks.TestNet28.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet26.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet27.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet26.Name, TestNetSporks.TestNet26.NODE, TestNetSporks.TestNet26.ROOT_HEIGHT, TestNetSporks.TestNet27.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet25.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet26.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet25.Name, TestNetSporks.TestNet25.NODE, TestNetSporks.TestNet25.ROOT_HEIGHT, TestNetSporks.TestNet26.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet24.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet25.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet24.Name, TestNetSporks.TestNet24.NODE, TestNetSporks.TestNet24.ROOT_HEIGHT, TestNetSporks.TestNet25.ROOT_HEIGHT - 1, true),

                _ when blockHeight >= TestNetSporks.TestNet23.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet24.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet23.Name, TestNetSporks.TestNet23.NODE, TestNetSporks.TestNet23.ROOT_HEIGHT, TestNetSporks.TestNet24.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet22.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet23.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet22.Name, TestNetSporks.TestNet22.NODE, TestNetSporks.TestNet22.ROOT_HEIGHT, TestNetSporks.TestNet23.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet21.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet22.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet21.Name, TestNetSporks.TestNet21.NODE, TestNetSporks.TestNet21.ROOT_HEIGHT, TestNetSporks.TestNet22.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet20.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet21.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet20.Name, TestNetSporks.TestNet20.NODE, TestNetSporks.TestNet20.ROOT_HEIGHT, TestNetSporks.TestNet21.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet19.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet20.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet19.Name, TestNetSporks.TestNet19.NODE, TestNetSporks.TestNet19.ROOT_HEIGHT, TestNetSporks.TestNet20.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet18.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet19.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet18.Name, TestNetSporks.TestNet18.NODE, TestNetSporks.TestNet18.ROOT_HEIGHT, TestNetSporks.TestNet19.ROOT_HEIGHT - 1, true),
                _ when blockHeight >= TestNetSporks.TestNet17.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet18.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet17.Name, TestNetSporks.TestNet17.NODE, TestNetSporks.TestNet17.ROOT_HEIGHT, TestNetSporks.TestNet18.ROOT_HEIGHT - 1, true),
                _ => throw new System.NotSupportedException("Spork not supported")
            };
        }

        public static Spork GetMainSporkByHeight(ulong blockHeight)
        {
            return blockHeight switch
            {
                _ when blockHeight >= MainNetSporks.MainNet.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet.Name, MainNetSporks.MainNet.NODE, MainNetSporks.MainNet.ROOT_HEIGHT, null),
                _ when blockHeight >= MainNetSporks.MainNet15.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet15.Name, MainNetSporks.MainNet15.NODE, MainNetSporks.MainNet15.ROOT_HEIGHT, MainNetSporks.MainNet.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet14.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet15.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet14.Name, MainNetSporks.MainNet14.NODE, MainNetSporks.MainNet14.ROOT_HEIGHT, MainNetSporks.MainNet15.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet13.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet14.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet13.Name, MainNetSporks.MainNet13.NODE, MainNetSporks.MainNet13.ROOT_HEIGHT, MainNetSporks.MainNet14.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet12.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet13.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet12.Name, MainNetSporks.MainNet12.NODE, MainNetSporks.MainNet12.ROOT_HEIGHT, MainNetSporks.MainNet13.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet11.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet12.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet11.Name, MainNetSporks.MainNet11.NODE, MainNetSporks.MainNet11.ROOT_HEIGHT, MainNetSporks.MainNet12.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet10.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet11.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet10.Name, MainNetSporks.MainNet10.NODE, MainNetSporks.MainNet10.ROOT_HEIGHT, MainNetSporks.MainNet11.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet9.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet10.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet9.Name, MainNetSporks.MainNet9.NODE, MainNetSporks.MainNet9.ROOT_HEIGHT, MainNetSporks.MainNet10.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet8.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet9.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet8.Name, MainNetSporks.MainNet8.NODE, MainNetSporks.MainNet8.ROOT_HEIGHT, MainNetSporks.MainNet9.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet7.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet8.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet7.Name, MainNetSporks.MainNet7.NODE, MainNetSporks.MainNet7.ROOT_HEIGHT, MainNetSporks.MainNet8.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet6.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet7.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet6.Name, MainNetSporks.MainNet6.NODE, MainNetSporks.MainNet6.ROOT_HEIGHT, MainNetSporks.MainNet7.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet5.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet6.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet5.Name, MainNetSporks.MainNet5.NODE, MainNetSporks.MainNet5.ROOT_HEIGHT, MainNetSporks.MainNet6.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet4.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet5.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet4.Name, MainNetSporks.MainNet4.NODE, MainNetSporks.MainNet4.ROOT_HEIGHT, MainNetSporks.MainNet5.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet3.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet4.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet3.Name, MainNetSporks.MainNet3.NODE, MainNetSporks.MainNet3.ROOT_HEIGHT, MainNetSporks.MainNet4.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet2.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet3.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet2.Name, MainNetSporks.MainNet2.NODE, MainNetSporks.MainNet2.ROOT_HEIGHT, MainNetSporks.MainNet3.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet1.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet2.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet1.Name, MainNetSporks.MainNet1.NODE, MainNetSporks.MainNet1.ROOT_HEIGHT, MainNetSporks.MainNet2.ROOT_HEIGHT - 1),
                _ => throw new System.NotSupportedException("Spork not supported")
            };
        }

        public static Spork GetEmulatorSporkByHeight(ulong blockHeight)
        {
            return new Spork(EmulatorSporks.Emulator.Name, EmulatorSporks.Emulator.NODE, EmulatorSporks.Emulator.ROOT_HEIGHT, null, false, true);
        }
    }
}