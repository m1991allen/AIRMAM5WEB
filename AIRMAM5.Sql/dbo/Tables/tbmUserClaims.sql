CREATE TABLE [dbo].[tbmUserClaims] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [UserId]          NVARCHAR (MAX) NULL,
    [ClaimType]       NVARCHAR (MAX) NULL,
    [ClaimValue]      NVARCHAR (MAX) NULL,
    [IdentityUser_Id] NVARCHAR (128) NULL,
    CONSTRAINT [PK_dbo.tbmUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.tbmUserClaims_dbo.tbmUSERS_IdentityUser_Id] FOREIGN KEY ([IdentityUser_Id]) REFERENCES [dbo].[tbmUSERS] ([fsUSER_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_IdentityUser_Id]
    ON [dbo].[tbmUserClaims]([IdentityUser_Id] ASC);

