DECLARE @temp_table TABLE
(
	Id int,
	CustomerName nvarchar(100)
)

INSERT INTO @temp_table VALUES (1, 'Arisha Barron')
INSERT INTO @temp_table VALUES (2, 'Branden Gibson')
INSERT INTO @temp_table VALUES (3, 'Rhonda Church')
INSERT INTO @temp_table VALUES (4, 'Georgina Hazel')

SET IDENTITY_INSERT [dbo].[Customers] ON

MERGE INTO [dbo].[Customers] AS TARGET
USING @temp_table As SOURCE
ON TARGET.[Id] = SOURCE.[Id] AND TARGET.[Name] = SOURCE.[CustomerName]
WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, [Name]) VALUES (SOURCE.Id, SOURCE.[CustomerName]);


SET IDENTITY_INSERT [dbo].[Customers] OFF

GO