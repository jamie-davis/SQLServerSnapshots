ALTER TABLE [Chess].[Rating]
ADD CONSTRAINT FK_RatingPlayerId FOREIGN KEY (PlayerId)
REFERENCES [Chess].[Player] (PlayerId)
GO
ALTER TABLE [Chess].[Rating]
ADD CONSTRAINT FK_RatingSourceId FOREIGN KEY (RatingSourceId)
REFERENCES [Chess].[RatingSource] (RatingSourceId)
GO
CREATE INDEX IX_Rating_PlayerId ON [Chess].[Rating] (PlayerId)
GO
