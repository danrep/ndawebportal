CREATE TABLE [dbo].[PinMap]
(
	[Id] INT NOT NULL PRIMARY KEY identity(1,1), 
	CredentialId int not null,
	PinData varchar(1000) not null, 
	PinUse int not null, 
	CanExpire bit not null, 
	InitiationDate datetime null, 
	ExpiryDate datetime null, 
	CredentialAssigned int not null, 
	HasUseCounts bit not null, 
	UseCount int null, 
	UseCountMax int null, 
	IsDeleted bit not null
)
