USE [timm_gtu]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](45) NOT NULL,
 CONSTRAINT [PK_roles_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[companies]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[companies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_companies_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[campaigns]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[campaigns](
	[Id] [int] NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Description] [varchar](1024) NOT NULL,
	[UserName] [varchar](64) NOT NULL,
	[Date] [datetime2](0) NOT NULL,
	[CustemerName] [varchar](64) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[ZoomLevel] [int] NOT NULL,
	[Sequence] [int] NULL,
	[ClientName] [varchar](64) NOT NULL,
	[ContactName] [varchar](64) NOT NULL,
	[ClientCode] [varchar](64) NOT NULL,
	[Logo] [varchar](64) NULL,
	[AreaDescription] [varchar](128) NOT NULL,
	[CreatorName] [varchar](64) NOT NULL,
 CONSTRAINT [PK_campaigns_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[users]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[users](
	[Id] [int] NOT NULL,
	[UserName] [varchar](64) NOT NULL,
	[Password] [varchar](64) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[FullName] [varchar](128) NOT NULL,
	[UserCode] [varchar](64) NOT NULL,
	[Role] [int] NOT NULL,
 CONSTRAINT [PK_users_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [users$UserName_UNIQUE] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[userrolemappings]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[userrolemappings](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Id] [int] IDENTITY(56,1) NOT NULL,
 CONSTRAINT [PK_userrolemappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_USERROLEMAPPINGS_ROLEID] ON [dbo].[userrolemappings] 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_USERROLEMAPPINGS_USERID] ON [dbo].[userrolemappings] 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaignrecords]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaignrecords](
	[Id] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Classification] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[Value] [bit] NOT NULL,
 CONSTRAINT [PK_campaignrecords_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CampaignRecords_Campaign] ON [dbo].[campaignrecords] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaignpercentagecolors]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaignpercentagecolors](
	[Id] [int] IDENTITY(2130853639,1) NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Min] [float] NOT NULL,
	[Max] [float] NOT NULL,
	[ColorId] [int] NOT NULL,
 CONSTRAINT [PK_campaignpercentagecolors_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CampaignPercentageColors_Campaign] ON [dbo].[campaignpercentagecolors] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaignclassifications]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaignclassifications](
	[Id] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Classification] [int] NOT NULL,
 CONSTRAINT [PK_campaignclassifications_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CampaignClassifications_Campaign] ON [dbo].[campaignclassifications] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[addresses]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[addresses](
	[Id] [int] NOT NULL,
	[Address] [varchar](256) NOT NULL,
	[ZipCode] [varchar](8) NOT NULL,
	[OriginalLatitude] [real] NOT NULL,
	[OriginalLongitude] [real] NOT NULL,
	[Longitude] [real] NOT NULL,
	[Latitude] [real] NOT NULL,
	[Color] [varchar](8) NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Picture] [varchar](64) NULL,
 CONSTRAINT [PK_addresses_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_Addresses_Campaign] ON [dbo].[addresses] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[employees]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_employees_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[distributionjobs]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[distributionjobs](
	[Id] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[CampaignId] [int] NOT NULL,
 CONSTRAINT [PK_distributionjobs_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_DistributionJobs_Campaign] ON [dbo].[distributionjobs] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[gtus]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[gtus](
	[Id] [int] NOT NULL,
	[UniqueID] [varchar](50) NULL,
	[Model] [varchar](50) NULL,
	[IsEnabled] [bit] NOT NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_gtus_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [gtus$UniqueID_UNIQUE] UNIQUE NONCLUSTERED 
(
	[UniqueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_Gtus_Users] ON [dbo].[gtus] 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[submaps]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[submaps](
	[Id] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Total] [int] NOT NULL,
	[Penetration] [int] NOT NULL,
	[Percentage] [float] NOT NULL,
	[ColorR] [int] NOT NULL,
	[ColorG] [int] NOT NULL,
	[ColorB] [int] NOT NULL,
	[ColorString] [varchar](16) NOT NULL,
	[CampaignId] [int] NOT NULL,
	[TotalAdjustment] [int] NOT NULL,
	[CountAdjustment] [int] NOT NULL,
 CONSTRAINT [PK_submaps_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_SubMaps_Campaign] ON [dbo].[submaps] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[submaprecords]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[submaprecords](
	[Id] [int] NOT NULL,
	[SubMapId] [int] NOT NULL,
	[Classification] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[Value] [bit] NOT NULL,
 CONSTRAINT [PK_submaprecords_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_SubMapRecords_SubMap] ON [dbo].[submaprecords] 
(
	[SubMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[submapcoordinates]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[submapcoordinates](
	[Id] [int] IDENTITY(1840015,1) NOT NULL,
	[SubMapId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_submapcoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_SubMapCoordinates_SubMap] ON [dbo].[submapcoordinates] 
(
	[SubMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[radiuses]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[radiuses](
	[Id] [int] NOT NULL,
	[Length] [float] NOT NULL,
	[LengthMeasuresId] [int] NOT NULL,
	[AddressId] [int] NOT NULL,
	[IsDisplay] [bit] NOT NULL,
 CONSTRAINT [PK_radiuses_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_Radiuses_Address] ON [dbo].[radiuses] 
(
	[AddressId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[distributionmaps]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[distributionmaps](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SubMapId] [int] NOT NULL,
	[ColorR] [int] NOT NULL,
	[ColorG] [int] NOT NULL,
	[ColorB] [int] NOT NULL,
	[ColorString] [varchar](16) NOT NULL,
	[Total] [int] NOT NULL,
	[Penetration] [int] NOT NULL,
	[Percentage] [float] NOT NULL,
	[TotalAdjustment] [int] NOT NULL,
	[CountAdjustment] [int] NOT NULL,
 CONSTRAINT [PK_distributionmaps_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_DistributionMaps_SubMap] ON [dbo].[distributionmaps] 
(
	[SubMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[distributionmaprecords]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[distributionmaprecords](
	[Id] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[DistributionMapId] [int] NOT NULL,
	[Classification] [int] NOT NULL,
	[Value] [bit] NOT NULL,
 CONSTRAINT [PK_distributionmaprecords_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_DistributionMapRecords_DistributionMap] ON [dbo].[distributionmaprecords] 
(
	[DistributionMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[distributionmapcoordinates]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[distributionmapcoordinates](
	[Id] [int] IDENTITY(194974,1) NOT NULL,
	[DistributionMapId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_distributionmapcoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_distributionmapcoordinates_1] ON [dbo].[distributionmapcoordinates] 
(
	[DistributionMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[monitoraddresses]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[monitoraddresses](
	[Id] [int] NOT NULL,
	[Address] [varchar](256) NOT NULL,
	[ZipCode] [varchar](8) NOT NULL,
	[OriginalLatitude] [real] NOT NULL,
	[OriginalLongitude] [real] NOT NULL,
	[Longitude] [real] NOT NULL,
	[Latitude] [real] NOT NULL,
	[DmId] [int] NOT NULL,
	[Picture] [varchar](64) NULL,
 CONSTRAINT [PK_monitoraddresses_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_monitoraddresses_1] ON [dbo].[monitoraddresses] 
(
	[DmId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[distributionjobmaps]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[distributionjobmaps](
	[Id] [int] NOT NULL,
	[DistributionJobId] [int] NULL,
	[DistributionMapId] [int] NULL,
 CONSTRAINT [PK_distributionjobmaps_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_DistributionJobMaps_DistributionJobs] ON [dbo].[distributionjobmaps] 
(
	[DistributionJobId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_DistributionJobMaps_DistributionMaps] ON [dbo].[distributionjobmaps] 
(
	[DistributionMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](45) NOT NULL,
	[Date] [date] NULL,
	[DmId] [int] NOT NULL,
	[AuditorId] [int] NULL,
	[Status] [int] NULL,
	[Email] [varchar](45) NULL,
	[Telephone] [varchar](45) NULL,
 CONSTRAINT [PK_task_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_monitors_1] ON [dbo].[task] 
(
	[DmId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_monitors_2] ON [dbo].[task] 
(
	[AuditorId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tasktime]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tasktime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NULL,
	[Time] [datetime2](0) NULL,
	[TimeType] [int] NULL,
 CONSTRAINT [PK_tasktime_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_tasktime_1] ON [dbo].[tasktime] 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[taskgtuinfomapping]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[taskgtuinfomapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NOT NULL,
	[GTUId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[DistributionJobId] [int] NOT NULL,
	[Color] [varchar](45) NULL,
	[TimeInserted] [datetime] NULL,
 CONSTRAINT [PK_taskgtuinfomapping_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[gtuinfo]    Script Date: 09/26/2011 21:56:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[gtuinfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dwSpeed] [float] NULL,
	[nHeading] [int] NULL,
	[dtSendTime] [datetime2](0) NULL,
	[dtReceivedTime] [datetime2](0) NULL,
	[sIPAddress] [varchar](45) NULL,
	[nAreaCode] [int] NULL,
	[nNetworkCode] [int] NULL,
	[nCellID] [int] NULL,
	[nGPSFix] [int] NULL,
	[nAccuracy] [int] NULL,
	[nCount] [int] NULL,
	[nLocationID] [int] NULL,
	[sVersion] [varchar](45) NULL,
	[dwAltitude] [float] NULL,
	[dwLatitude] [float] NULL,
	[dwLongitude] [float] NULL,
	[PowerInfo] [int] NULL,
	[TaskgtuinfoId] [int] NULL,
	[Code] [varchar](100) NULL,
	[Status] [int] NULL,
	[Distance] [float] NULL,
 CONSTRAINT [PK_gtuinfo_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [FK_gtuinfo_1] ON [dbo].[gtuinfo] 
(
	[TaskgtuinfoId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF__addresses__Pictu__34C8D9D1]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[addresses] ADD  DEFAULT (NULL) FOR [Picture]
GO
/****** Object:  Default [DF__campaigns__Seque__38996AB5]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[campaigns] ADD  DEFAULT (NULL) FOR [Sequence]
GO
/****** Object:  Default [DF__campaigns__Logo__398D8EEE]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[campaigns] ADD  DEFAULT (NULL) FOR [Logo]
GO
/****** Object:  Default [DF__campaigns__Creat__3A81B327]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[campaigns] ADD  DEFAULT ('') FOR [CreatorName]
GO
/****** Object:  Default [DF__distribut__Distr__3B75D760]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionjobmaps] ADD  DEFAULT (NULL) FOR [DistributionJobId]
GO
/****** Object:  Default [DF__distribut__Distr__3C69FB99]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionjobmaps] ADD  DEFAULT (NULL) FOR [DistributionMapId]
GO
/****** Object:  Default [DF__distribut__Color__3D5E1FD2]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [ColorR]
GO
/****** Object:  Default [DF__distribut__Color__3E52440B]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [ColorG]
GO
/****** Object:  Default [DF__distribut__Color__3F466844]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [ColorB]
GO
/****** Object:  Default [DF__distribut__Color__403A8C7D]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT (N'000000') FOR [ColorString]
GO
/****** Object:  Default [DF__distribut__Total__412EB0B6]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [Total]
GO
/****** Object:  Default [DF__distribut__Penet__4222D4EF]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [Penetration]
GO
/****** Object:  Default [DF__distribut__Perce__4316F928]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [Percentage]
GO
/****** Object:  Default [DF__distribut__Total__440B1D61]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [TotalAdjustment]
GO
/****** Object:  Default [DF__distribut__Count__44FF419A]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps] ADD  DEFAULT ((0)) FOR [CountAdjustment]
GO
/****** Object:  Default [DF__gtuinfo__dwSpeed__236943A5]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [dwSpeed]
GO
/****** Object:  Default [DF__gtuinfo__nHeadin__245D67DE]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nHeading]
GO
/****** Object:  Default [DF__gtuinfo__dtSendT__25518C17]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [dtSendTime]
GO
/****** Object:  Default [DF__gtuinfo__dtRecei__2645B050]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [dtReceivedTime]
GO
/****** Object:  Default [DF__gtuinfo__sIPAddr__2739D489]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [sIPAddress]
GO
/****** Object:  Default [DF__gtuinfo__nAreaCo__282DF8C2]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nAreaCode]
GO
/****** Object:  Default [DF__gtuinfo__nNetwor__29221CFB]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nNetworkCode]
GO
/****** Object:  Default [DF__gtuinfo__nCellID__2A164134]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nCellID]
GO
/****** Object:  Default [DF__gtuinfo__nGPSFix__2B0A656D]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nGPSFix]
GO
/****** Object:  Default [DF__gtuinfo__nAccura__2BFE89A6]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nAccuracy]
GO
/****** Object:  Default [DF__gtuinfo__nCount__2CF2ADDF]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nCount]
GO
/****** Object:  Default [DF__gtuinfo__nLocati__2DE6D218]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [nLocationID]
GO
/****** Object:  Default [DF__gtuinfo__sVersio__2EDAF651]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [sVersion]
GO
/****** Object:  Default [DF__gtuinfo__dwAltit__2FCF1A8A]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [dwAltitude]
GO
/****** Object:  Default [DF__gtuinfo__dwLatit__30C33EC3]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [dwLatitude]
GO
/****** Object:  Default [DF__gtuinfo__dwLongi__31B762FC]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [dwLongitude]
GO
/****** Object:  Default [DF__gtuinfo__PowerIn__32AB8735]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [PowerInfo]
GO
/****** Object:  Default [DF__gtuinfo__Taskgtu__339FAB6E]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [TaskgtuinfoId]
GO
/****** Object:  Default [DF__gtuinfo__Code__3493CFA7]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [Code]
GO
/****** Object:  Default [DF__gtuinfo__Status__3587F3E0]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT ((0)) FOR [Status]
GO
/****** Object:  Default [DF__gtuinfo__Distanc__367C1819]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo] ADD  DEFAULT (NULL) FOR [Distance]
GO
/****** Object:  Default [DF__gtus__UniqueID__5CD6CB2B]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtus] ADD  DEFAULT (NULL) FOR [UniqueID]
GO
/****** Object:  Default [DF__gtus__Model__5DCAEF64]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtus] ADD  DEFAULT (NULL) FOR [Model]
GO
/****** Object:  Default [DF__gtus__UserId__5EBF139D]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtus] ADD  DEFAULT (NULL) FOR [UserId]
GO
/****** Object:  Default [DF__monitorad__Pictu__5FB337D6]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[monitoraddresses] ADD  DEFAULT (NULL) FOR [Picture]
GO
/****** Object:  Default [DF__submaps__TotalAd__619B8048]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[submaps] ADD  DEFAULT ((0)) FOR [TotalAdjustment]
GO
/****** Object:  Default [DF__submaps__CountAd__628FA481]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[submaps] ADD  DEFAULT ((0)) FOR [CountAdjustment]
GO
/****** Object:  Default [DF__task__Date__6383C8BA]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task] ADD  DEFAULT (NULL) FOR [Date]
GO
/****** Object:  Default [DF__task__AuditorId__6477ECF3]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task] ADD  DEFAULT (NULL) FOR [AuditorId]
GO
/****** Object:  Default [DF__task__Status__656C112C]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task] ADD  DEFAULT (NULL) FOR [Status]
GO
/****** Object:  Default [DF__task__Email__66603565]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task] ADD  DEFAULT (NULL) FOR [Email]
GO
/****** Object:  Default [DF__task__Telephone__6754599E]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task] ADD  DEFAULT (NULL) FOR [Telephone]
GO
/****** Object:  Default [DF__tasktime__TaskId__6B24EA82]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[tasktime] ADD  DEFAULT (NULL) FOR [TaskId]
GO
/****** Object:  Default [DF__tasktime__Time__6C190EBB]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[tasktime] ADD  DEFAULT (NULL) FOR [Time]
GO
/****** Object:  Default [DF__tasktime__TimeTy__6D0D32F4]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[tasktime] ADD  DEFAULT (NULL) FOR [TimeType]
GO
/****** Object:  ForeignKey [addresses$FK_Addresses_Campaign]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[addresses]  WITH NOCHECK ADD  CONSTRAINT [addresses$FK_Addresses_Campaign] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[campaigns] ([Id])
GO
ALTER TABLE [dbo].[addresses] CHECK CONSTRAINT [addresses$FK_Addresses_Campaign]
GO
/****** Object:  ForeignKey [campaignclassifications$FK_CampaignClassifications_Campaign]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[campaignclassifications]  WITH NOCHECK ADD  CONSTRAINT [campaignclassifications$FK_CampaignClassifications_Campaign] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[campaigns] ([Id])
GO
ALTER TABLE [dbo].[campaignclassifications] CHECK CONSTRAINT [campaignclassifications$FK_CampaignClassifications_Campaign]
GO
/****** Object:  ForeignKey [campaignpercentagecolors$FK_CampaignPercentageColors_Campaign]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[campaignpercentagecolors]  WITH NOCHECK ADD  CONSTRAINT [campaignpercentagecolors$FK_CampaignPercentageColors_Campaign] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[campaigns] ([Id])
GO
ALTER TABLE [dbo].[campaignpercentagecolors] CHECK CONSTRAINT [campaignpercentagecolors$FK_CampaignPercentageColors_Campaign]
GO
/****** Object:  ForeignKey [campaignrecords$FK_CampaignRecords_Campaign]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[campaignrecords]  WITH NOCHECK ADD  CONSTRAINT [campaignrecords$FK_CampaignRecords_Campaign] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[campaigns] ([Id])
GO
ALTER TABLE [dbo].[campaignrecords] CHECK CONSTRAINT [campaignrecords$FK_CampaignRecords_Campaign]
GO
/****** Object:  ForeignKey [distributionjobmaps$FK_DistributionJobMaps_DistributionJobs]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionjobmaps]  WITH NOCHECK ADD  CONSTRAINT [distributionjobmaps$FK_DistributionJobMaps_DistributionJobs] FOREIGN KEY([DistributionJobId])
REFERENCES [dbo].[distributionjobs] ([Id])
GO
ALTER TABLE [dbo].[distributionjobmaps] CHECK CONSTRAINT [distributionjobmaps$FK_DistributionJobMaps_DistributionJobs]
GO
/****** Object:  ForeignKey [distributionjobmaps$FK_DistributionJobMaps_DistributionMaps]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionjobmaps]  WITH NOCHECK ADD  CONSTRAINT [distributionjobmaps$FK_DistributionJobMaps_DistributionMaps] FOREIGN KEY([DistributionMapId])
REFERENCES [dbo].[distributionmaps] ([Id])
GO
ALTER TABLE [dbo].[distributionjobmaps] CHECK CONSTRAINT [distributionjobmaps$FK_DistributionJobMaps_DistributionMaps]
GO
/****** Object:  ForeignKey [distributionjobs$FK_DistributionJobs_Campaign]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionjobs]  WITH NOCHECK ADD  CONSTRAINT [distributionjobs$FK_DistributionJobs_Campaign] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[campaigns] ([Id])
GO
ALTER TABLE [dbo].[distributionjobs] CHECK CONSTRAINT [distributionjobs$FK_DistributionJobs_Campaign]
GO
/****** Object:  ForeignKey [distributionmapcoordinates$FK_distributionmapcoordinates_1]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmapcoordinates]  WITH NOCHECK ADD  CONSTRAINT [distributionmapcoordinates$FK_distributionmapcoordinates_1] FOREIGN KEY([DistributionMapId])
REFERENCES [dbo].[distributionmaps] ([Id])
GO
ALTER TABLE [dbo].[distributionmapcoordinates] CHECK CONSTRAINT [distributionmapcoordinates$FK_distributionmapcoordinates_1]
GO
/****** Object:  ForeignKey [distributionmaprecords$FK_DistributionMapRecords_DistributionMap]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaprecords]  WITH NOCHECK ADD  CONSTRAINT [distributionmaprecords$FK_DistributionMapRecords_DistributionMap] FOREIGN KEY([DistributionMapId])
REFERENCES [dbo].[distributionmaps] ([Id])
GO
ALTER TABLE [dbo].[distributionmaprecords] CHECK CONSTRAINT [distributionmaprecords$FK_DistributionMapRecords_DistributionMap]
GO
/****** Object:  ForeignKey [distributionmaps$FK_DistributionMaps_SubMap]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[distributionmaps]  WITH NOCHECK ADD  CONSTRAINT [distributionmaps$FK_DistributionMaps_SubMap] FOREIGN KEY([SubMapId])
REFERENCES [dbo].[submaps] ([Id])
GO
ALTER TABLE [dbo].[distributionmaps] CHECK CONSTRAINT [distributionmaps$FK_DistributionMaps_SubMap]
GO
/****** Object:  ForeignKey [FK_employees_companies]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[employees]  WITH CHECK ADD  CONSTRAINT [FK_employees_companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[companies] ([Id])
GO
ALTER TABLE [dbo].[employees] CHECK CONSTRAINT [FK_employees_companies]
GO
/****** Object:  ForeignKey [FK_gtuinfo_taskgtuinfomapping]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtuinfo]  WITH CHECK ADD  CONSTRAINT [FK_gtuinfo_taskgtuinfomapping] FOREIGN KEY([TaskgtuinfoId])
REFERENCES [dbo].[taskgtuinfomapping] ([Id])
GO
ALTER TABLE [dbo].[gtuinfo] CHECK CONSTRAINT [FK_gtuinfo_taskgtuinfomapping]
GO
/****** Object:  ForeignKey [gtus$FK_Gtus_Users]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[gtus]  WITH NOCHECK ADD  CONSTRAINT [gtus$FK_Gtus_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[gtus] CHECK CONSTRAINT [gtus$FK_Gtus_Users]
GO
/****** Object:  ForeignKey [monitoraddresses$FK_monitoraddresses_1]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[monitoraddresses]  WITH NOCHECK ADD  CONSTRAINT [monitoraddresses$FK_monitoraddresses_1] FOREIGN KEY([DmId])
REFERENCES [dbo].[distributionmaps] ([Id])
GO
ALTER TABLE [dbo].[monitoraddresses] CHECK CONSTRAINT [monitoraddresses$FK_monitoraddresses_1]
GO
/****** Object:  ForeignKey [radiuses$FK_Radiuses_Address]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[radiuses]  WITH NOCHECK ADD  CONSTRAINT [radiuses$FK_Radiuses_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[addresses] ([Id])
GO
ALTER TABLE [dbo].[radiuses] CHECK CONSTRAINT [radiuses$FK_Radiuses_Address]
GO
/****** Object:  ForeignKey [submapcoordinates$FK_SubMapCoordinates_SubMap]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[submapcoordinates]  WITH NOCHECK ADD  CONSTRAINT [submapcoordinates$FK_SubMapCoordinates_SubMap] FOREIGN KEY([SubMapId])
REFERENCES [dbo].[submaps] ([Id])
GO
ALTER TABLE [dbo].[submapcoordinates] CHECK CONSTRAINT [submapcoordinates$FK_SubMapCoordinates_SubMap]
GO
/****** Object:  ForeignKey [submaprecords$FK_SubMapRecords_SubMap]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[submaprecords]  WITH NOCHECK ADD  CONSTRAINT [submaprecords$FK_SubMapRecords_SubMap] FOREIGN KEY([SubMapId])
REFERENCES [dbo].[submaps] ([Id])
GO
ALTER TABLE [dbo].[submaprecords] CHECK CONSTRAINT [submaprecords$FK_SubMapRecords_SubMap]
GO
/****** Object:  ForeignKey [submaps$FK_SubMaps_Campaign]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[submaps]  WITH NOCHECK ADD  CONSTRAINT [submaps$FK_SubMaps_Campaign] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[campaigns] ([Id])
GO
ALTER TABLE [dbo].[submaps] CHECK CONSTRAINT [submaps$FK_SubMaps_Campaign]
GO
/****** Object:  ForeignKey [task$FK_monitors_1]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task]  WITH NOCHECK ADD  CONSTRAINT [task$FK_monitors_1] FOREIGN KEY([DmId])
REFERENCES [dbo].[distributionmaps] ([Id])
GO
ALTER TABLE [dbo].[task] CHECK CONSTRAINT [task$FK_monitors_1]
GO
/****** Object:  ForeignKey [task$FK_monitors_2]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[task]  WITH NOCHECK ADD  CONSTRAINT [task$FK_monitors_2] FOREIGN KEY([AuditorId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[task] CHECK CONSTRAINT [task$FK_monitors_2]
GO
/****** Object:  ForeignKey [FK_taskgtuinfomapping_distributionjobs]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[taskgtuinfomapping]  WITH CHECK ADD  CONSTRAINT [FK_taskgtuinfomapping_distributionjobs] FOREIGN KEY([DistributionJobId])
REFERENCES [dbo].[distributionjobs] ([Id])
GO
ALTER TABLE [dbo].[taskgtuinfomapping] CHECK CONSTRAINT [FK_taskgtuinfomapping_distributionjobs]
GO
/****** Object:  ForeignKey [FK_taskgtuinfomapping_employees]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[taskgtuinfomapping]  WITH CHECK ADD  CONSTRAINT [FK_taskgtuinfomapping_employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([Id])
GO
ALTER TABLE [dbo].[taskgtuinfomapping] CHECK CONSTRAINT [FK_taskgtuinfomapping_employees]
GO
/****** Object:  ForeignKey [FK_taskgtuinfomapping_gtus]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[taskgtuinfomapping]  WITH CHECK ADD  CONSTRAINT [FK_taskgtuinfomapping_gtus] FOREIGN KEY([GTUId])
REFERENCES [dbo].[gtus] ([Id])
GO
ALTER TABLE [dbo].[taskgtuinfomapping] CHECK CONSTRAINT [FK_taskgtuinfomapping_gtus]
GO
/****** Object:  ForeignKey [FK_taskgtuinfomapping_roles]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[taskgtuinfomapping]  WITH CHECK ADD  CONSTRAINT [FK_taskgtuinfomapping_roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[roles] ([Id])
GO
ALTER TABLE [dbo].[taskgtuinfomapping] CHECK CONSTRAINT [FK_taskgtuinfomapping_roles]
GO
/****** Object:  ForeignKey [FK_taskgtuinfomapping_task]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[taskgtuinfomapping]  WITH CHECK ADD  CONSTRAINT [FK_taskgtuinfomapping_task] FOREIGN KEY([TaskId])
REFERENCES [dbo].[task] ([Id])
GO
ALTER TABLE [dbo].[taskgtuinfomapping] CHECK CONSTRAINT [FK_taskgtuinfomapping_task]
GO
/****** Object:  ForeignKey [tasktime$FK_tasktime_1]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[tasktime]  WITH NOCHECK ADD  CONSTRAINT [tasktime$FK_tasktime_1] FOREIGN KEY([TaskId])
REFERENCES [dbo].[task] ([Id])
GO
ALTER TABLE [dbo].[tasktime] CHECK CONSTRAINT [tasktime$FK_tasktime_1]
GO
/****** Object:  ForeignKey [FK_userrolemappings_roles]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[userrolemappings]  WITH CHECK ADD  CONSTRAINT [FK_userrolemappings_roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[roles] ([Id])
GO
ALTER TABLE [dbo].[userrolemappings] CHECK CONSTRAINT [FK_userrolemappings_roles]
GO
/****** Object:  ForeignKey [FK_userrolemappings_users]    Script Date: 09/26/2011 21:56:00 ******/
ALTER TABLE [dbo].[userrolemappings]  WITH CHECK ADD  CONSTRAINT [FK_userrolemappings_users] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[userrolemappings] CHECK CONSTRAINT [FK_userrolemappings_users]
GO
