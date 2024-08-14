using System;

namespace Graffle.FlowSdk.Services.Nodes
{
    public sealed class Spork
    {
        public Spork(string name, string node, ulong rootHeight, ulong? maxHeight, bool isTestNet = false, bool isEmulator = false, bool isPreviewNet = false)
        {
            Name = name;
            Node = node;
            RootHeight = rootHeight;
            MaxHeight = maxHeight;
            IsTestNet = isTestNet;
            IsEmulator = isEmulator;
            IsPreviewNet = isPreviewNet;
        }

        public string Name { get; }
        public string Node { get; }
        public ulong RootHeight { get; }
        public ulong? MaxHeight { get; }
        public bool IsTestNet { get; }
        public bool IsEmulator { get; }
        public bool IsPreviewNet { get; }
    }

    public static class Sporks
    {
        private static readonly Spork _main = GetSporkByName(MainNetSporks.MainNet.Name);
        private static readonly Spork _test = GetSporkByName(TestNetSporks.TestNet.Name);
        private static readonly Spork _previewNet = new("PreviewNet", "access.previewnet.nodes.onflow.org:9000", 0, null, false, false, true);

        public static Spork MainNet() => _main;
        public static Spork TestNet() => _test;
        public static Spork PreviewNet() => _previewNet;

        /// <summary>
        /// Returns true if the spork occurs after Cadence 1.0 upgrade (ie crescendo)
        /// </summary>
        /// <param name="spork"></param>
        /// <returns>True if the network is runnning Cadence 1.0, false otherwise</returns>
        public static bool IsCrescendo(Spork spork)
        {
            return spork.IsTestNet switch
            {
                true => spork.RootHeight >= TestNetSporks.TestNet.ROOT_HEIGHT,
                false => false
            };
        }

        public static Spork GetSporkByName(string sporkName) =>
            sporkName switch
            {
                _ when sporkName == TestNetSporks.TestNet.Name => new Spork(TestNetSporks.TestNet.Name, TestNetSporks.TestNet.NODE, TestNetSporks.TestNet.ROOT_HEIGHT, null, true),
                _ when sporkName == TestNetSporks.TestNet50.Name => new Spork(TestNetSporks.TestNet50.Name, TestNetSporks.TestNet50.NODE, TestNetSporks.TestNet50.ROOT_HEIGHT, TestNetSporks.TestNet.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet49.Name => new Spork(TestNetSporks.TestNet49.Name, TestNetSporks.TestNet49.NODE, TestNetSporks.TestNet49.ROOT_HEIGHT, TestNetSporks.TestNet50.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet48.Name => new Spork(TestNetSporks.TestNet48.Name, TestNetSporks.TestNet48.NODE, TestNetSporks.TestNet48.ROOT_HEIGHT, TestNetSporks.TestNet49.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet47.Name => new Spork(TestNetSporks.TestNet47.Name, TestNetSporks.TestNet47.NODE, TestNetSporks.TestNet47.ROOT_HEIGHT, TestNetSporks.TestNet48.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet46.Name => new Spork(TestNetSporks.TestNet46.Name, TestNetSporks.TestNet46.NODE, TestNetSporks.TestNet46.ROOT_HEIGHT, TestNetSporks.TestNet47.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet45.Name => new Spork(TestNetSporks.TestNet45.Name, TestNetSporks.TestNet45.NODE, TestNetSporks.TestNet45.ROOT_HEIGHT, TestNetSporks.TestNet46.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet44.Name => new Spork(TestNetSporks.TestNet44.Name, TestNetSporks.TestNet44.NODE, TestNetSporks.TestNet44.ROOT_HEIGHT, TestNetSporks.TestNet45.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet43.Name => new Spork(TestNetSporks.TestNet43.Name, TestNetSporks.TestNet43.NODE, TestNetSporks.TestNet43.ROOT_HEIGHT, TestNetSporks.TestNet44.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet42.Name => new Spork(TestNetSporks.TestNet42.Name, TestNetSporks.TestNet42.NODE, TestNetSporks.TestNet42.ROOT_HEIGHT, TestNetSporks.TestNet43.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet41.Name => new Spork(TestNetSporks.TestNet41.Name, TestNetSporks.TestNet41.NODE, TestNetSporks.TestNet41.ROOT_HEIGHT, TestNetSporks.TestNet42.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet40.Name => new Spork(TestNetSporks.TestNet40.Name, TestNetSporks.TestNet40.NODE, TestNetSporks.TestNet40.ROOT_HEIGHT, TestNetSporks.TestNet41.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet39.Name => new Spork(TestNetSporks.TestNet39.Name, TestNetSporks.TestNet39.NODE, TestNetSporks.TestNet39.ROOT_HEIGHT, TestNetSporks.TestNet40.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet38.Name => new Spork(TestNetSporks.TestNet38.Name, TestNetSporks.TestNet38.NODE, TestNetSporks.TestNet38.ROOT_HEIGHT, TestNetSporks.TestNet39.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet37.Name => new Spork(TestNetSporks.TestNet37.Name, TestNetSporks.TestNet37.NODE, TestNetSporks.TestNet37.ROOT_HEIGHT, TestNetSporks.TestNet38.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet36.Name => new Spork(TestNetSporks.TestNet36.Name, TestNetSporks.TestNet36.NODE, TestNetSporks.TestNet36.ROOT_HEIGHT, TestNetSporks.TestNet37.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet35.Name => new Spork(TestNetSporks.TestNet35.Name, TestNetSporks.TestNet35.NODE, TestNetSporks.TestNet35.ROOT_HEIGHT, TestNetSporks.TestNet36.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet34.Name => new Spork(TestNetSporks.TestNet34.Name, TestNetSporks.TestNet34.NODE, TestNetSporks.TestNet34.ROOT_HEIGHT, TestNetSporks.TestNet35.ROOT_HEIGHT - 1, true),
                _ when sporkName == TestNetSporks.TestNet33.Name => new Spork(TestNetSporks.TestNet33.Name, TestNetSporks.TestNet33.NODE, TestNetSporks.TestNet33.ROOT_HEIGHT, TestNetSporks.TestNet34.ROOT_HEIGHT - 1, true),
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
                _ when sporkName == MainNetSporks.MainNet23.Name => new Spork(MainNetSporks.MainNet23.Name, MainNetSporks.MainNet23.NODE, MainNetSporks.MainNet23.ROOT_HEIGHT, MainNetSporks.MainNet.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet22.Name => new Spork(MainNetSporks.MainNet22.Name, MainNetSporks.MainNet22.NODE, MainNetSporks.MainNet22.ROOT_HEIGHT, MainNetSporks.MainNet23.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet21.Name => new Spork(MainNetSporks.MainNet21.Name, MainNetSporks.MainNet21.NODE, MainNetSporks.MainNet21.ROOT_HEIGHT, MainNetSporks.MainNet22.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet20.Name => new Spork(MainNetSporks.MainNet20.Name, MainNetSporks.MainNet20.NODE, MainNetSporks.MainNet20.ROOT_HEIGHT, MainNetSporks.MainNet21.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet19.Name => new Spork(MainNetSporks.MainNet19.Name, MainNetSporks.MainNet19.NODE, MainNetSporks.MainNet19.ROOT_HEIGHT, MainNetSporks.MainNet20.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet18.Name => new Spork(MainNetSporks.MainNet18.Name, MainNetSporks.MainNet18.NODE, MainNetSporks.MainNet18.ROOT_HEIGHT, MainNetSporks.MainNet19.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet17.Name => new Spork(MainNetSporks.MainNet17.Name, MainNetSporks.MainNet17.NODE, MainNetSporks.MainNet17.ROOT_HEIGHT, MainNetSporks.MainNet18.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet16.Name => new Spork(MainNetSporks.MainNet16.Name, MainNetSporks.MainNet16.NODE, MainNetSporks.MainNet16.ROOT_HEIGHT, MainNetSporks.MainNet17.ROOT_HEIGHT - 1),
                _ when sporkName == MainNetSporks.MainNet15.Name => new Spork(MainNetSporks.MainNet15.Name, MainNetSporks.MainNet15.NODE, MainNetSporks.MainNet15.ROOT_HEIGHT, MainNetSporks.MainNet16.ROOT_HEIGHT - 1),
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
                _ => throw new ArgumentException($"Invalid Spork {sporkName}")
            };

        public static Spork GetDevSporkByHeight(ulong blockHeight) =>
             blockHeight switch
             {
                 _ when blockHeight >= TestNetSporks.TestNet.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet.Name, TestNetSporks.TestNet.NODE, TestNetSporks.TestNet.ROOT_HEIGHT, null, true),
                 _ when blockHeight >= TestNetSporks.TestNet50.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet50.Name, TestNetSporks.TestNet50.NODE, TestNetSporks.TestNet50.ROOT_HEIGHT, TestNetSporks.TestNet.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet49.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet50.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet49.Name, TestNetSporks.TestNet49.NODE, TestNetSporks.TestNet49.ROOT_HEIGHT, TestNetSporks.TestNet50.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet48.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet49.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet48.Name, TestNetSporks.TestNet48.NODE, TestNetSporks.TestNet48.ROOT_HEIGHT, TestNetSporks.TestNet49.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet47.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet48.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet47.Name, TestNetSporks.TestNet47.NODE, TestNetSporks.TestNet47.ROOT_HEIGHT, TestNetSporks.TestNet48.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet46.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet47.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet46.Name, TestNetSporks.TestNet46.NODE, TestNetSporks.TestNet46.ROOT_HEIGHT, TestNetSporks.TestNet47.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet45.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet46.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet45.Name, TestNetSporks.TestNet45.NODE, TestNetSporks.TestNet45.ROOT_HEIGHT, TestNetSporks.TestNet46.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet44.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet45.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet44.Name, TestNetSporks.TestNet44.NODE, TestNetSporks.TestNet44.ROOT_HEIGHT, TestNetSporks.TestNet45.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet43.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet44.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet43.Name, TestNetSporks.TestNet43.NODE, TestNetSporks.TestNet43.ROOT_HEIGHT, TestNetSporks.TestNet44.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet42.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet43.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet42.Name, TestNetSporks.TestNet42.NODE, TestNetSporks.TestNet42.ROOT_HEIGHT, TestNetSporks.TestNet43.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet41.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet42.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet41.Name, TestNetSporks.TestNet41.NODE, TestNetSporks.TestNet41.ROOT_HEIGHT, TestNetSporks.TestNet42.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet40.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet41.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet40.Name, TestNetSporks.TestNet40.NODE, TestNetSporks.TestNet40.ROOT_HEIGHT, TestNetSporks.TestNet41.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet39.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet40.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet39.Name, TestNetSporks.TestNet39.NODE, TestNetSporks.TestNet39.ROOT_HEIGHT, TestNetSporks.TestNet40.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet38.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet39.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet38.Name, TestNetSporks.TestNet38.NODE, TestNetSporks.TestNet38.ROOT_HEIGHT, TestNetSporks.TestNet39.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet37.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet38.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet37.Name, TestNetSporks.TestNet37.NODE, TestNetSporks.TestNet37.ROOT_HEIGHT, TestNetSporks.TestNet38.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet36.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet37.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet36.Name, TestNetSporks.TestNet36.NODE, TestNetSporks.TestNet36.ROOT_HEIGHT, TestNetSporks.TestNet37.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet35.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet36.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet35.Name, TestNetSporks.TestNet35.NODE, TestNetSporks.TestNet35.ROOT_HEIGHT, TestNetSporks.TestNet36.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet34.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet35.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet34.Name, TestNetSporks.TestNet34.NODE, TestNetSporks.TestNet34.ROOT_HEIGHT, TestNetSporks.TestNet35.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet33.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet34.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet33.Name, TestNetSporks.TestNet33.NODE, TestNetSporks.TestNet33.ROOT_HEIGHT, TestNetSporks.TestNet34.ROOT_HEIGHT - 1, true),
                 _ when blockHeight >= TestNetSporks.TestNet32.ROOT_HEIGHT && blockHeight < TestNetSporks.TestNet33.ROOT_HEIGHT => new Spork(TestNetSporks.TestNet32.Name, TestNetSporks.TestNet32.NODE, TestNetSporks.TestNet32.ROOT_HEIGHT, TestNetSporks.TestNet33.ROOT_HEIGHT - 1, true),
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
                 _ => throw new ArgumentOutOfRangeException($"Invalid TestNet block height {blockHeight}")
             };

        public static Spork GetMainSporkByHeight(ulong blockHeight) =>
            blockHeight switch
            {
                _ when blockHeight >= MainNetSporks.MainNet.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet.Name, MainNetSporks.MainNet.NODE, MainNetSporks.MainNet.ROOT_HEIGHT, null),
                _ when blockHeight >= MainNetSporks.MainNet23.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet23.Name, MainNetSporks.MainNet23.NODE, MainNetSporks.MainNet23.ROOT_HEIGHT, MainNetSporks.MainNet.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet22.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet23.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet22.Name, MainNetSporks.MainNet22.NODE, MainNetSporks.MainNet22.ROOT_HEIGHT, MainNetSporks.MainNet23.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet21.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet22.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet21.Name, MainNetSporks.MainNet21.NODE, MainNetSporks.MainNet21.ROOT_HEIGHT, MainNetSporks.MainNet22.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet20.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet21.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet20.Name, MainNetSporks.MainNet20.NODE, MainNetSporks.MainNet20.ROOT_HEIGHT, MainNetSporks.MainNet21.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet19.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet20.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet19.Name, MainNetSporks.MainNet19.NODE, MainNetSporks.MainNet19.ROOT_HEIGHT, MainNetSporks.MainNet20.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet18.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet19.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet18.Name, MainNetSporks.MainNet18.NODE, MainNetSporks.MainNet18.ROOT_HEIGHT, MainNetSporks.MainNet19.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet17.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet18.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet17.Name, MainNetSporks.MainNet17.NODE, MainNetSporks.MainNet17.ROOT_HEIGHT, MainNetSporks.MainNet18.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet16.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet17.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet16.Name, MainNetSporks.MainNet16.NODE, MainNetSporks.MainNet16.ROOT_HEIGHT, MainNetSporks.MainNet17.ROOT_HEIGHT - 1),
                _ when blockHeight >= MainNetSporks.MainNet15.ROOT_HEIGHT && blockHeight < MainNetSporks.MainNet16.ROOT_HEIGHT => new Spork(MainNetSporks.MainNet15.Name, MainNetSporks.MainNet15.NODE, MainNetSporks.MainNet15.ROOT_HEIGHT, MainNetSporks.MainNet16.ROOT_HEIGHT - 1),
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
                _ => throw new ArgumentOutOfRangeException($"Invalid MainNet block height {blockHeight}")
            };


        public static Spork GetEmulatorSporkByHeight(ulong blockHeight) =>
             new Spork(EmulatorSporks.Emulator.Name, EmulatorSporks.Emulator.NODE, EmulatorSporks.Emulator.ROOT_HEIGHT, null, false, true);
    }
}