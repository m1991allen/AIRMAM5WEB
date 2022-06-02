CREATE TABLE [dbo].[tbmUserLogins] (
    [UserId]          NVARCHAR (128) NOT NULL,
    [LoginProvider]   NVARCHAR (128) NOT NULL,
    [ProviderKey]     NVARCHAR (128) NOT NULL,
    [IdentityUser_Id] NVARCHAR (128) NULL,
    CONSTRAINT [PK_dbo.tbmUserLogins] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_dbo.tbmUserLogins_dbo.tbmUSERS_IdentityUser_Id] FOREIGN KEY ([IdentityUser_Id]) REFERENCES [dbo].[tbmUSERS] ([fsUSER_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_IdentityUser_Id]
    ON [dbo].[tbmUserLogins]([IdentityUser_Id] ASC);

