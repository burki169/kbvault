ALTER TABLE [dbo].[Settings] ADD [ShowTotalArticleCountOnFrontPage] [bit] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[Category] ADD [Icon] [nvarchar](200)
ALTER TABLE [dbo].[Settings] ADD [SelectedTheme] [nvarchar](max)