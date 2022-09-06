CREATE TABLE [dbo].[ColumnDefinitions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[TableDefinitionsId] INT NOT NULL,
	[DataTypesId] INT NOT NULL,
	[ColumnName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_ColumnDefinitions_DataTypeId_ToTable] FOREIGN KEY ([DataTypesId]) REFERENCES [DataTypes]([Id]),
	CONSTRAINT [FK_ColumnDefinitions_TableDefinitionsId_ToTable] FOREIGN KEY ([TableDefinitionsId]) REFERENCES [TableDefinitions]([Id])
)