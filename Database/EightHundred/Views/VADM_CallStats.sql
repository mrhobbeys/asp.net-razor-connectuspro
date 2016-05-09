-- =============================================
-- Script Template
-- =============================================
USE [DB_10668_calls]
GO

/****** Object:  View [dbo].[VADM_CallStats]    Script Date: 04/26/2012 17:19:39 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VADM_CallStats]'))
DROP VIEW [dbo].[VADM_CallStats]
GO

USE [DB_10668_calls]
GO

/****** Object:  View [dbo].[VADM_CallStats]    Script Date: 04/26/2012 17:19:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/******* callstats report *****/
CREATE VIEW [dbo].[VADM_CallStats]
AS
SELECT     TOP (100) PERCENT dbo.StatisticTracks.TrackId AS seqnr, dbo.StatisticTracks.StartDate AS calltime, dbo.StatisticTracks.CalledNumber, 
                      dbo.LookupScript.XMLCode AS CalledDescription, dbo.StatisticTracks.DialedNumber AS callerid, dbo.StatisticTracks.Duration, dbo.StatisticTracks.Jobid, u.UserName, 
                      dbo.EndCallOptions.OptionName
FROM         dbo.StatisticTracks LEFT OUTER JOIN
                      Membership.dbo.aspnet_Users AS u ON dbo.StatisticTracks.UserId = u.UserId LEFT OUTER JOIN
                      dbo.LookupScript ON dbo.StatisticTracks.CalledNumber = dbo.LookupScript.LookupPhoneNr LEFT OUTER JOIN
                      dbo.EndCallOptions ON dbo.StatisticTracks.OptionId = dbo.EndCallOptions.OptionId
WHERE     (dbo.StatisticTracks.CalledNumber <> '')
ORDER BY seqnr

GO