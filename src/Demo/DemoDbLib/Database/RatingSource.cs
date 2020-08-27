using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDbLib.Database
{
    [Table("RatingSource", Schema = "Chess")]
    public class RatingSource
    {
        public int RatingSourceId { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }
    }
}