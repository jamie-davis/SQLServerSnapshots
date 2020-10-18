using System;
using System.Collections.Generic;
using System.Text;
using SnapshotTests;

namespace DbTests.SchemaConfig
{
    [SnapshotDefinition("[Chess].[Rating]")]
    public static class RatingTable
    {
        /// <summary>
        /// RatingDate is not defaulted to GETDATE() or any of the automatic "get the time" variants. This was deliberate, to illustrate that we can alter the automated decisions.
        /// In a similar vein, a GUID was used as the table key for [Chess].[Rating] to illustrate that GUIDs can be handled, however, when they are the primary key they can
        /// randomise the order rows are returned, so we need to force a sort on other fields to make the differences repeatable.
        /// </summary>
        [Unpredictable]
        [SortField(0)]
        public static DateTime RatingDate { get; set; }

        /// <summary>
        /// Rating source is the second sort field.
        /// </summary>
        [SortField(1)]
        public static int RatingSourceId { get; set; }

    }

    [SnapshotDefinition("[Chess].[Player]")]
    public static class PlayerTable
    {
        /// <summary>
        /// we need to sort player data because [Chess].[Player] has a GUID for a primary key, randomising the order that rows are returned.
        /// </summary>
        [SortField]
        public static string Name { get; set; }
    }

    [SnapshotDefinition("[Chess].[AuditPlayer]")]
    public static class AuditPlayerTable
    {
        /// <summary>
        /// The audit table does not have the player ID defined as a foreign key (because the player might be deleted), so no relationship is
        /// defined automatically. We define it manually to make the keys consistent.
        /// </summary>
        [References("[Chess].[Player]", "PlayerId")]
        public static int PlayerId { get; set; }

        /// <summary>
        /// Date will match the update timestamp. They are set by a trigger, not a default, so it's not set as unpredictable automatically.
        /// It's also a local date, just to illustrate local handling.
        /// </summary>
        [Unpredictable]
        [Local]
        public static DateTime AuditStartDate { get; set; }

        /// <summary>
        /// Date will match the update timestamp. They are set by a trigger, not a default, so it's not set as unpredictable automatically
        /// It's also a local date, just to illustrate local handling.
        /// </summary>
        [Unpredictable]
        [Local]
        public static DateTime AuditEndDate { get; set; }
    }

    [SnapshotDefinition("[Chess].[AuditRating]")]
    public static class AuditRatingTable
    {
        [References("[Chess].[Player]", "PlayerId")]
        public static int PlayerId { get; set; }

        [References("[Chess].[Rating]", "RatingId")]
        public static int RatingId { get; set; }

        /// <summary>
        /// Date will match the update timestamp. They are set by a trigger, not a default, so it's not set as unpredictable automatically.
        /// It's also a local date, just to illustrate local handling.
        /// </summary>
        [Unpredictable]
        [Local]
        public static DateTime AuditStartDate { get; set; }

        /// <summary>
        /// Date will match the update timestamp. They are set by a trigger, not a default, so it's not set as unpredictable automatically
        /// It's also a local date, just to illustrate local handling.
        /// </summary>
        [Unpredictable]
        [Local]
        public static DateTime AuditEndDate { get; set; }

        /// <summary>
        /// Date will match the rating timestamp. Therefore we note it's unpredictable and local.
        /// </summary>
        [Unpredictable]
        [Local]
        public static DateTime RatingDate { get; set; }
    }
}
