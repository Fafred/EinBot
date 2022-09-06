﻿CREATE TABLE [dbo].[TableDefinitions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[TableName] NVARCHAR(50) NOT NULL,
	[CollectionTypesId] INT NOT NULL,
	[RoleID] INT, 
    CONSTRAINT [FK_TableDefinitions_ToTable] FOREIGN KEY ([CollectionTypesId]) REFERENCES [CollectionTypes]([Id]),
)