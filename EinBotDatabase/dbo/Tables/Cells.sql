CREATE TABLE [dbo].[Cells]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[TableDefinitionsId] INT NOT NULL,
	[ColumnDefinitionsId] INT NOT NULL,
	[RowNum] INT NOT NULL,
	[Data] NVARCHAR(50),
	[RowKey] NVARCHAR(50), 
    CONSTRAINT [FK_Cells_TableDefinitionsId_ToTable] FOREIGN KEY ([TableDefinitionsId]) REFERENCES [TableDefinitions]([Id]), 
    CONSTRAINT [FK_Cells_ColumnDefinitionsId_ToTable] FOREIGN KEY ([ColumnDefinitionsId]) REFERENCES [ColumnDefinitions]([Id]),
)
