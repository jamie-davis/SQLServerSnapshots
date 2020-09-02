using SnapshotTests;

namespace SQLServerSnapshots.Tests.TestDefinitions
{
    [SnapshotDefinition("[Test].[A_Main]")]
    public static class AMain
    {
        [Predictable]
        public static int MainId { get; set; }
    }
}
