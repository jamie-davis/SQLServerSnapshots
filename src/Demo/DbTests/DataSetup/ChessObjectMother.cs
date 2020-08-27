using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DbTests.TestConfiguration;
using DemoDbLib.Database;
using Respawn;

namespace DbTests.DataSetup
{
    internal class ChessObjectMother
    {
        public const string ChessDotCom = "Chess.com";
        public const string ECF = "ECF";
        public const string FIDE = "FIDE";
        public const string Lichess = "Lichess.org";

        private static Checkpoint checkpoint = new Checkpoint
        {
            TablesToIgnore = new []
            {
                "RatingSource"
            },
            SchemasToInclude = new []
            {
                "Chess"
            }
        };

        public ChessObjectMother()
        {
            
        }

        public async Task ResetAsync()
        {
            await checkpoint.Reset(Config.ConnectionString);

            await AddPlayerAsync("Janet", (ChessDotCom, 1401), (Lichess, 1642));
            await AddPlayerAsync("John", (Lichess, 1216));
        }

        public async Task AddPlayerAsync(string name, params (string Source, long Rating)[] ratings)
        {
            var entities = new ChessContext(Config.ConnectionString);

            var player = new Player
            {
                PlayerId = Guid.NewGuid(),
                Name = name
            };
            entities.Players.Add(player);

            player.Ratings = ratings.Select(rating =>
                {
                    var ratingEntity = new Rating
                    {
                        RatingId = Guid.NewGuid(),
                        Player = player,
                        PlayerRating = rating.Rating,
                        RatingSource = entities.RatingSources.First(s => s.Name == rating.Source),
                        RatingDate = DateTime.Now
                    };

                    entities.Ratings.Add(ratingEntity);

                    return ratingEntity;
                })
                .ToList();

            await entities.SaveChangesAsync();
        }
    }
}
