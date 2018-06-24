CREATE TABLE [dbo].[State] (
    [StateId]   INT           IDENTITY (1, 1) NOT NULL,
    [StateName] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([StateId] ASC)
);

