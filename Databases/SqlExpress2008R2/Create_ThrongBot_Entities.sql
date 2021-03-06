USE [ThrongBot]
GO
/****** Object:  Table [dbo].[UserAgentString]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAgentString](
	[Id] [uniqueidentifier] NOT NULL,
	[UserAgent] [nvarchar](1024) NULL,
 CONSTRAINT [PK_UserAgentString] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SessionConfiguration]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SessionConfiguration](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionId] [int] NOT NULL,
	[MaxConcurrentCrawls] [int] NOT NULL,
 CONSTRAINT [PK_SessionConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessedPage]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessedPage](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionId] [int] NOT NULL,
	[CrawlerId] [int] NOT NULL,
	[PageUrl] [nvarchar](1024) NULL,
	[Title] [nvarchar](1024) NULL,
	[Description] [nvarchar](1024) NULL,
	[KeyWords] [nvarchar](1024) NULL,
	[StatusCode] [int] NULL,
 CONSTRAINT [PK_ProcessedPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LinkToCrawl]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LinkToCrawl](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionId] [int] NOT NULL,
	[InProgress] [bit] NULL,
	[TargetBaseDomain] [nvarchar](256) NULL,
	[SourceUrl] [nvarchar](1024) NULL,
	[TargetUrl] [nvarchar](1024) NULL,
	[CrawlDepth] [int] NULL,
	[IsRoot] [bit] NULL,
	[IsInternal] [bit] NULL,
 CONSTRAINT [PK_LinkToCrawl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CrawlerRun]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CrawlerRun](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionId] [int] NOT NULL,
	[Crawlerid] [int] NOT NULL,
	[SeedUrl] [nvarchar](1024) NULL,
	[BaseDomain] [nvarchar](256) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Depth] [int] NULL,
	[CrawledCount] [int] NULL,
	[InProgress] [bit] NULL,
	[ErrorOccurred] [bit] NULL,
 CONSTRAINT [PK_CrawlerRun] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CrawledLink]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CrawledLink](
	[Id] [uniqueidentifier] NOT NULL,
	[SessionId] [int] NOT NULL,
	[CrawlerId] [int] NOT NULL,
	[SourceUrl] [nvarchar](1024) NULL,
	[TargetUrl] [nvarchar](1024) NULL,
	[StatusCode] [int] NULL,
	[ErrorOccurred] [bit] NULL,
	[Exception] [nvarchar](2048) NULL,
	[IsRoot] [bit] NULL,
	[Bypassed] [bit] NULL,
	[CrawlDepth] [int] NULL,
 CONSTRAINT [PK_CrawledLink] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlackList]    Script Date: 09/21/2014 16:42:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlackList](
	[Url] [nvarchar](512) NULL
) ON [PRIMARY]
GO
