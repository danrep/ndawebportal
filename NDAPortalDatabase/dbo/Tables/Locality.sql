CREATE TABLE [dbo].[Locality] (
    [LocalityId]   INT           IDENTITY (1, 1) NOT NULL,
    [LocalityName] NVARCHAR (50) NOT NULL,
    [StateId]      INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([LocalityId] ASC)
);

