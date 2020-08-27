INSERT INTO [Chess].[RatingSource] (Name)
SELECT 'ECF'
UNION
SELECT 'Chess.com'
UNION
SELECT 'Lichess.org'
UNION
SELECT 'FIDE'
GO