CREATE TABLE [dbo].[UploadedParticular] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [AdmissionId]     VARCHAR (100)  NOT NULL,
    [RegistationGuid] VARCHAR (100)  NOT NULL,
    [ActiveSessionId] INT            NOT NULL,
    [CredentialId]    INT            NOT NULL,
    [DocumentTypeId]  INT            NOT NULL,
    [FileNameData]    VARCHAR (1000) NOT NULL,
    [IsDeleted]       BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

