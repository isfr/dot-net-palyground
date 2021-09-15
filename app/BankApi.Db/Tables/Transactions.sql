CREATE TABLE [dbo].[Transactions]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OriginAccountId] INT NOT NULL, 
    [DestinationAccountId] INT NOT NULL, 
    [TransactionAmount] DECIMAL(19, 4) NOT NULL, 
    [Timestamp] DATETIME2 NOT NULL,
    CONSTRAINT [FK_Transactions_Accounts_Origin] FOREIGN KEY ([OriginAccountId]) REFERENCES [Accounts]([Id]),
    CONSTRAINT [FK_Transactions_Accounts_Destination] FOREIGN KEY ([DestinationAccountId]) REFERENCES [Accounts]([Id])
)
