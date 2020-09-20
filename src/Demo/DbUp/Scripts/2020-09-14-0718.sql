SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Chess].[AuditRating]
(
     AuditRatingId INT IDENTITY PRIMARY KEY NOT NULL
	,RatingId UNIQUEIDENTIFIER NOT NULL
	,PlayerId UNIQUEIDENTIFIER NOT NULL
	,RatingSourceId INT NOT NULL 
	,Rating BIGINT NOT NULL
	,RatingDate DATETIME NOT NULL
    ,[AuditStartDate] DATETIME NOT NULL
    ,[AuditEndDate] DATETIME NOT NULL
    ,[AuditStartOperation] CHAR(1) NOT NULL 
    ,[AuditEndOperation] CHAR(1) NULL 
)
GO
CREATE TRIGGER [Chess].[trAuditRating] ON [Chess].[Rating]
FOR INSERT, UPDATE, DELETE 
AS
    SET NOCOUNT ON -- Trigger cannot affect the "rows affected" counter, or else it would break Entity Framework
    
    DECLARE @Now datetime
    DECLARE @Action varchar(1)
    DECLARE @infinite DATETIME
    SET @infinite = '9999-12-31'

    -- Defining if it's an UPDATE (U), INSERT (I), or DELETE ('D')
    IF (SELECT COUNT(*) FROM inserted) > 0 BEGIN
        IF (SELECT COUNT(*) FROM deleted) > 0  
            SET @Action = 'U'
        ELSE
            SET @Action = 'I'
    END
    
    SET @Now = GETDATE()


    -- Closing the lifetime of the current revisions (EndDate=infinite) for records which were updated or deleted
    IF (@Action='D' OR @Action='U')
        UPDATE [Chess].[AuditRating]
        SET [AuditEndDate] = @Now, 
        [AuditEndOperation] = @Action 
        FROM [Chess].[AuditPlayer] aud
        INNER JOIN deleted tab
        ON [tab].[PlayerId] = [aud].[PlayerId]
        AND aud.[AuditEndDate] = @infinite

    -- Creating new revisions for records which were inserted or updated
    IF (@Action='I' OR @Action='U') BEGIN
        INSERT INTO [Chess].[AuditRating] ([RatingId], [PlayerId], [RatingSourceId], [Rating], [RatingDate], [AuditStartDate], [AuditEndDate], [AuditStartOperation])
        SELECT     [inserted].[RatingId], [inserted].[PlayerId], [inserted].[RatingSourceId], [inserted].[Rating], [inserted].[RatingDate], 
        @Now,
        @infinite, 
        @Action
        FROM inserted

    END
GO    
