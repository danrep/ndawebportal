CREATE TABLE [dbo].[Session] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [SessionName]      VARCHAR (100) NOT NULL,
    [IsCurrentSession] BIT           NOT NULL,
    [IsDeleted]        BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

