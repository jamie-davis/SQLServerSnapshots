using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDbLib.Database
{
    [Table("Player", Schema="Chess")]
    public class Player
    {
        public Guid PlayerId { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }

        public List<Rating> Ratings { get; set; }
    }
}