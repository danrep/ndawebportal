CREATE TABLE [dbo].[CredentialArea]
(
	[Id] INT NOT NULL PRIMARY KEY identity(1,1), 
	AreaName varchar(100) not null,
	CredentialId int not null, 
	IsDeleted bit not null
)
