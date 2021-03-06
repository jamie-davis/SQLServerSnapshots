﻿CREATE SCHEMA [Chess]
GO
CREATE TABLE [Chess].[Player]
(
	 PlayerId UNIQUEIDENTIFIER PRIMARY KEY NOT NULL
	,[Name] varchar(120)
)
GO 
CREATE TABLE [Chess].[RatingSource]
(
	 RatingSourceId INT PRIMARY KEY IDENTITY
	,[Name] varchar(120)
)
GO
CREATE TABLE [Chess].[Rating]
(
	 RatingId UNIQUEIDENTIFIER PRIMARY KEY NOT NULL
	,PlayerId UNIQUEIDENTIFIER NOT NULL
	,RatingSourceId INT NOT NULL 
	,Rating BIGINT NOT NULL
	,RatingDate DATETIME NOT NULL)
GO