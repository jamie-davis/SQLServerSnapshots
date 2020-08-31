using SnapshotTests;

namespace SQLServerSnapshots.Tests.TestDefinitions
{
    [SnapshotDefinition("[Test].[AMain]")]
    public static class AMain
    {
        [Predictable]
        public static int MainId { get; set; }
    }
}
