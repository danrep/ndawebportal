CREATE TABLE [dbo].[Admission] (
    [Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [SessionId]              INT           DEFAULT ((0)) NOT NULL,
    [CredentialId]           INT           NOT NULL,
    [AdmissionId]            VARCHAR (100) NOT NULL,
    [AccessPin]              VARCHAR (100) NOT NULL,
    [DateAdmissionRequested] DATETIME      NOT NULL,
    [DatePinApplied]         DATETIME      NOT NULL,
    [AdmissionStatus]        INT           NOT NULL,
    [IsDeleted]              BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);





