DROP TABLE if exists [dbo].[TGSetup];
GO

CREATE TABLE [dbo].[TGSetup](
	[CompanyID] [int] NOT NULL,
	[FaceApiSubscriptionKey] [varchar](50) NOT NULL,
	[FaceApiEndpoint] [varchar](50) NOT NULL,
	[FaceApiGroupID] [varchar](36) NOT NULL,
	[FaceApiConfidenceThreshold] [decimal](10, 6) NOT NULL,
	[MapQuestApiKey] [varchar](100) NOT NULL,
	[tstamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_TGSetup] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
