CREATE TABLE [dbo].[Programme] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [ProgrammeName] VARCHAR (100) NOT NULL,
    [ProgrammeCode] VARCHAR (100) NOT NULL,
    [IsDeleted]     BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

