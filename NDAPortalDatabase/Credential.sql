CREATE TABLE [dbo].[Credential] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Username]     VARCHAR (100)  NOT NULL,
    [PhoneNumber]  VARCHAR (100)  NOT NULL,
    [Password]     VARCHAR (1000) NOT NULL,
    [PasswordSalt] VARCHAR (100)  NOT NULL,
    [Surname]      VARCHAR (100)  NULL,
    [FirstName]    VARCHAR (100)  NULL,
    [OtherNames]   VARCHAR (100)  NULL,
    [IsDeleted]    BIT            NOT NULL,
    [LasLogIn]     DATETIME       NOT NULL,
    [UserState]    INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


