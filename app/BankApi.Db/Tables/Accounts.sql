CREATE TABLE [dbo].[Accounts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Balance] DECIMAL(19, 4) NOT NULL, 
    [OwnerId] INT NOT NULL, 
    CONSTRAINT [FK_Accounts_Customers_Owner] FOREIGN KEY (OwnerId) REFERENCES Customers([Id])
)
