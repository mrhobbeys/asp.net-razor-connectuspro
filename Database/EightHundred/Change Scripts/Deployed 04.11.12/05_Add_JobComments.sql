USE [EightHundred]
GO

BEGIN TRAN

BEGIN TRY

	IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tbl_Job_Comment')
	BEGIN
		CREATE TABLE dbo.tbl_Job_Comment
		(
			JobCommentId uniqueidentifier NOT NULL,
			JobId int NOT NULL,
			CommentTypeId int NOT NULL,
			CreateDate datetime NOT NULL,
			Comment nvarchar(MAX) NOT NULL
		)  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
		ALTER TABLE dbo.tbl_Job_Comment ADD CONSTRAINT
		PK_tbl_Job_Comment PRIMARY KEY CLUSTERED 
		(
		JobCommentId
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END

	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'tbl_Job_CommentType')
	BEGIN
		CREATE TABLE dbo.tbl_Job_CommentType
		(
		JobCommentTypeId int NOT NULL,
		CommentType nvarchar(50) NOT NULL
		)  ON [PRIMARY]
	ALTER TABLE dbo.tbl_Job_CommentType ADD CONSTRAINT
		PK_tbl_Job_CommentType PRIMARY KEY CLUSTERED 
		(
		JobCommentTypeId
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	END
	
	IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Job_Comment_JobCommentId]') AND type = 'D')
		ALTER TABLE [dbo].[tbl_Job_Comment] DROP CONSTRAINT [DF_tbl_Job_Comment_JobCommentId]

	ALTER TABLE [dbo].[tbl_Job_Comment] ADD CONSTRAINT [DF_tbl_Job_Comment_JobCommentId]  DEFAULT (NEWSEQUENTIALID()) FOR [JobCommentId]
	
	IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tbl_Job_Comment_CreateDate]') AND type = 'D')
		ALTER TABLE [dbo].[tbl_Job_Comment] DROP CONSTRAINT [DF_tbl_Job_Comment_CreateDate]

	ALTER TABLE [dbo].[tbl_Job_Comment] ADD  CONSTRAINT [DF_tbl_Job_Comment_CreateDate]  DEFAULT (GETDATE()) FOR [CreateDate]

	IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_Job_Comments_tbl_Job_CommentType]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_Job_Comment]'))
	ALTER TABLE [dbo].[tbl_Job_Comment] DROP CONSTRAINT [FK_tbl_Job_Comments_tbl_Job_CommentType]

	ALTER TABLE [dbo].[tbl_Job_Comment]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Job_Comments_tbl_Job_CommentType] FOREIGN KEY([CommentTypeId])
	REFERENCES [dbo].[tbl_Job_CommentType] ([JobCommentTypeId])

	ALTER TABLE [dbo].[tbl_Job_Comment] CHECK CONSTRAINT [FK_tbl_Job_Comments_tbl_Job_CommentType]
	
	IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tbl_Job_Comments_tbl_Job]') AND parent_object_id = OBJECT_ID(N'[dbo].[tbl_Job_Comment]'))
	ALTER TABLE [dbo].[tbl_Job_Comment] DROP CONSTRAINT [FK_tbl_Job_Comments_tbl_Job]

	ALTER TABLE [dbo].[tbl_Job_Comment]  WITH CHECK ADD  CONSTRAINT [FK_tbl_Job_Comments_tbl_Job] FOREIGN KEY([JobId])
	REFERENCES [dbo].[tbl_Job] ([JobId])

	ALTER TABLE [dbo].[tbl_Job_Comment] CHECK CONSTRAINT [FK_tbl_Job_Comments_tbl_Job]

	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	PRINT 'Error: ' + ERROR_MESSAGE()
END CATCH