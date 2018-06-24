CREATE TABLE [dbo].[SubProgramme] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [ProgrammeId]      INT           NOT NULL,
    [SubProgrammeCode] VARCHAR (100) NOT NULL,
    [SubProgrammeName] VARCHAR (100) NOT NULL,
    [IsDeleted]        BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

