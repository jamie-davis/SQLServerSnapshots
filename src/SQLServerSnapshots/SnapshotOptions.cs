using System;

namespace SQLServerSnapshots
{
    /// <summary>
    /// This enum defines flags that can be used to control the database snapshot process.
    /// </summary>
    [Flags]
    public enum SnapshotOptions
    {
        /// <summary>
        /// Perform an automatic snapshot.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Do not automatically snapshot the database. This allows the returned snapshot builder to be used to populate the snapshot manually.
        /// <remarks>This is useful to allow data to be added to the snapshot and benefit from the database schema analysis.</remarks>
        /// </summary>
        NoAutoSnapshot = 1
    }
}