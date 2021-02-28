CREATE TABLE [dbo].[Word]
(
	[wordId] INT NOT NULL PRIMARY KEY, 
    [category] NVARCHAR(50) NULL, 
    [engWord] NVARCHAR(50) NULL, 
    [rusWord] NVARCHAR(50) NULL, 
    [chatId] INT NOT NULL
)
