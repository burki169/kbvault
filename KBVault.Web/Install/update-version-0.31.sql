ALTER TABLE dbo.Settings ADD
	ArticlePrefix nvarchar(50) NULL
GO
ALTER TABLE dbo.Settings ADD CONSTRAINT
	DF_Settings_ArticlePrefix DEFAULT N'KB' FOR ArticlePrefix
GO