
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [WordOfTheDay]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[WordOfTheDays]
           ([ActiveIn]
           ,[Phrase_Id])
           select getdate(),x.ID from (SELECT top 1 ID
  FROM [dbo].[Phrases] where Interesting = 1 order by newId()) x
END

GO
/****** Object:  Table [dbo].[PhraseMessages]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhraseMessages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Header] [nvarchar](50) NULL,
	[Content] [nvarchar](max) NULL,
	[Date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PhraseNotifications]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhraseNotifications](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[DateSent] [datetime] NOT NULL,
	[NotificationType] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PhraseNotificationUsers]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhraseNotificationUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChannelName] [nvarchar](max) NULL,
	[LastUpdate] [datetime] NOT NULL,
	[User_Id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Phrases]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Phrases](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[Source] [int] NOT NULL,
	[Interesting] [bit] NOT NULL,
	[AddDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PhraseUsers]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhraseUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DeviceId] [nvarchar](max) NULL,
	[AccountId] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[KeywordPhraseWeakReferences]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KeywordPhraseWeakReferences](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Convergence] [float] NOT NULL,
	[UserWeight] [float] NOT NULL,
	[Phrase_Id] [bigint] NULL,
	[Keyword_Id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Keywords]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Keywords](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](50) NULL,
	[Count] [bigint] NOT NULL,
	[Phrase_Id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NotificationHistories]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationHistories](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhraseNotifications_Id] [bigint] NULL,
	[PhraseUser_Id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProposedPhrases]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposedPhrases](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Right] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RequestInfoes]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestInfoes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DeviceId] [nvarchar](max) NULL,
	[AccountId] [nvarchar](max) NULL,
	[RequestType] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WordOfTheDays]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WordOfTheDays](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ActiveIn] [datetime] NOT NULL,
	[Phrase_Id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [WeakReferences]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [WeakReferences]
AS
SELECT        kg.Id, kg.Convergence, kg.UserWeight, kg.Phrase_Id, kg.Keyword_Id, k.Value kValue, g.Value AS gValue
FROM            dbo.KeywordPhraseWeakReferences AS kg LEFT OUTER JOIN
                         dbo.Phrases AS g ON g.Id = kg.Phrase_Id LEFT OUTER JOIN
                         dbo.Keywords AS k ON k.Id = kg.Keyword_Id


GO
/****** Object:  View [dbo].[UserRequestStatistics]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[UserRequestStatistics]
AS
SELECT Count(0) as UserRequestCount
  FROM [dbo].[PhraseUsers] gu
  union
  select SUM(count)
  from [dbo].KeyWords k



GO
/****** Object:  View [dbo].[UsersPerDay]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[UsersPerDay]
AS
select  x.updateTime,Count(x.Id) newUsers from (
SELECT Id, dateadd(dd, datediff(dd, 0, LastUpdate)+0, 0) updateTime
  FROM [dbo].[PhraseNotificationUsers]) x
  group by x.updateTime




GO
/****** Object:  View [dbo].[UsersPerWeek]    Script Date: 2014-11-20 11:53:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[UsersPerWeek]
AS

select  DateAdd(d,x.updateTime*7,CAST(GETDATE() AS DATE)) as UserCount ,Count(x.Id) newUsers from (
SELECT Id, DateDiff(d,GETDATE(),Cast(LastUpdate as Date))/7 updateTime
  FROM [dbo].[PhraseNotificationUsers]
  ) x
  group by x.updateTime
  --order by updateTime





GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Phrases_Value]    Script Date: 2014-11-20 11:53:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Phrases_Value] ON [dbo].[Phrases]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Keywords_Value]    Script Date: 2014-11-20 11:53:08 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Keywords_Value] ON [dbo].[Keywords]
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PhraseNotificationUsers]  WITH CHECK ADD  CONSTRAINT [PhraseNotificationUser_User] FOREIGN KEY([User_Id])
REFERENCES [dbo].[PhraseUsers] ([Id])
GO
ALTER TABLE [dbo].[PhraseNotificationUsers] CHECK CONSTRAINT [PhraseNotificationUser_User]
GO
ALTER TABLE [dbo].[KeywordPhraseWeakReferences]  WITH CHECK ADD  CONSTRAINT [KeywordPhraseWeakReference_Phrase] FOREIGN KEY([Phrase_Id])
REFERENCES [dbo].[Phrases] ([Id])
GO
ALTER TABLE [dbo].[KeywordPhraseWeakReferences] CHECK CONSTRAINT [KeywordPhraseWeakReference_Phrase]
GO
ALTER TABLE [dbo].[KeywordPhraseWeakReferences]  WITH CHECK ADD  CONSTRAINT [KeywordPhraseWeakReference_Keyword] FOREIGN KEY([Keyword_Id])
REFERENCES [dbo].[Keywords] ([Id])
GO
ALTER TABLE [dbo].[KeywordPhraseWeakReferences] CHECK CONSTRAINT [KeywordPhraseWeakReference_Keyword]
GO
ALTER TABLE [dbo].[Keywords]  WITH CHECK ADD  CONSTRAINT [Keyword_Phrase] FOREIGN KEY([Phrase_Id])
REFERENCES [dbo].[Phrases] ([Id])
GO
ALTER TABLE [dbo].[Keywords] CHECK CONSTRAINT [Keyword_Phrase]
GO
ALTER TABLE [dbo].[NotificationHistories]  WITH CHECK ADD  CONSTRAINT [NotificationHistory_PhraseNotifications] FOREIGN KEY([PhraseNotifications_Id])
REFERENCES [dbo].[PhraseNotifications] ([Id])
GO
ALTER TABLE [dbo].[NotificationHistories] CHECK CONSTRAINT [NotificationHistory_PhraseNotifications]
GO
ALTER TABLE [dbo].[NotificationHistories]  WITH CHECK ADD  CONSTRAINT [NotificationHistory_PhraseUser] FOREIGN KEY([PhraseUser_Id])
REFERENCES [dbo].[PhraseUsers] ([Id])
GO
ALTER TABLE [dbo].[NotificationHistories] CHECK CONSTRAINT [NotificationHistory_PhraseUser]
GO
ALTER TABLE [dbo].[WordOfTheDays]  WITH CHECK ADD  CONSTRAINT [WordOfTheDay_Phrase] FOREIGN KEY([Phrase_Id])
REFERENCES [dbo].[Phrases] ([Id])
GO
ALTER TABLE [dbo].[WordOfTheDays] CHECK CONSTRAINT [WordOfTheDay_Phrase]
GO


GO
ALTER DATABASE [$DatabaseName$] SET  READ_WRITE 
GO
