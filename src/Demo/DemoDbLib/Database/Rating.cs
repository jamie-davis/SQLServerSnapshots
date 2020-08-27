using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDbLib.Database
{
    [Table("Rating", Schema = "Chess")]
    public class Rating
    {
        public Guid RatingId { get; set; }
        public Guid PlayerId { get; set; }
        public int RatingSourceId { get; set; }

        [Column("Rating")] //Not allowed to have a property with the class name
        public long PlayerRating { get; set; }
        public DateTime RatingDate { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public Player Player { get; set; }

        [ForeignKey(nameof(RatingSourceId))]
        public RatingSource RatingSource { get; set; }
    }
}