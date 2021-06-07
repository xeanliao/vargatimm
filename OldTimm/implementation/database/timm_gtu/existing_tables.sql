USE [timm_gtu]
GO
/****** Object:  Table [dbo].[elementaryschoolareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[elementaryschoolareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_elementaryschoolareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[elementaryschoolareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[elementaryschoolareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ElementarySchoolAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_elementaryschoolareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_ELEMENTARYSCHOOLAREACOORDINATES_ELEMENTARYSCHOOLAREAID] ON [dbo].[elementaryschoolareacoordinates] 
(
	[ElementarySchoolAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[elementaryschoolareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[elementaryschoolareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[ElementarySchoolAreaId] [int] NOT NULL,
 CONSTRAINT [PK_elementaryschoolareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_ELEMENTARYSCHOOLAREABOXMAPPINGS_ELEMENTARYSCHOOLAREAID] ON [dbo].[elementaryschoolareaboxmappings] 
(
	[ElementarySchoolAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_ELEMENTARYSCHOOLAREABOXMAPPINGS_BOXID] ON [dbo].[elementaryschoolareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[metropolitancoreareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[metropolitancoreareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[Memi] [varchar](8) NOT NULL,
	[MtFCC] [varchar](8) NOT NULL,
	[Type] [varchar](8) NOT NULL,
	[Status] [varchar](8) NOT NULL,
	[GEOCODE] [varchar](8) NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_metropolitancoreareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[metropolitancoreareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[metropolitancoreareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MetropolitanCoreAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_metropolitancoreareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_MetropolitanCoreAreaCoordinates_MetropolitanCoreAreaId] ON [dbo].[metropolitancoreareacoordinates] 
(
	[MetropolitanCoreAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[metropolitancoreareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[metropolitancoreareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[MetropolitanCoreAreaId] [int] NOT NULL,
 CONSTRAINT [PK_metropolitancoreareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_MetropolitanCoreAreaBoxMappings_MetropolitanCoreAreaId] ON [dbo].[metropolitancoreareaboxmappings] 
(
	[MetropolitanCoreAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_MetropolitanCoreAreaBoxMappings_BoxId] ON [dbo].[metropolitancoreareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[lowerhouseareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[lowerhouseareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[GEO_ID] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_lowerhouseareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[lowerhouseareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lowerhouseareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LowerHouseAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_lowerhouseareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_LowerHouseAreaCoordinates_LowerHouseAreaId] ON [dbo].[lowerhouseareacoordinates] 
(
	[LowerHouseAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[lowerhouseareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lowerhouseareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[LowerHouseAreaId] [int] NOT NULL,
 CONSTRAINT [PK_lowerhouseareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_LowerHouseAreaBoxMappings_LowerHouseAreaId] ON [dbo].[lowerhouseareaboxmappings] 
(
	[LowerHouseAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_LowerHouseAreaBoxMappings_BoxId] ON [dbo].[lowerhouseareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[countyareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[countyareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_countyareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[countyareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[countyareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountyAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_countyareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_COUNTYAREACOORDINATES_COUNTYAREAID] ON [dbo].[countyareacoordinates] 
(
	[CountyAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[countyareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[countyareaboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[CountyAreaId] [int] NOT NULL,
 CONSTRAINT [PK_countyareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_COUNTYAREABOXMAPPINGS_COUNTYAREAID] ON [dbo].[countyareaboxmappings] 
(
	[CountyAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_COUNTYAREABOXMAPPINGS_BOXID] ON [dbo].[countyareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaignusermappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaignusermappings](
	[UserId] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Id] [int] IDENTITY(58,1) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_campaignusermappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_campaignusermappings_1] ON [dbo].[campaignusermappings] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_submapusermappings_1] ON [dbo].[campaignusermappings] 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaigntractimported]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaigntractimported](
	[Id] [int] NOT NULL,
	[TractId] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Total] [int] NOT NULL,
	[Penetration] [int] NOT NULL,
 CONSTRAINT [PK_campaigntractimported_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CampaignTractImported_CampaignId] ON [dbo].[campaigntractimported] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CampaignTractImported_TractId] ON [dbo].[campaigntractimported] 
(
	[TractId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaigns_backup]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[campaigns_backup](
	[Id] [int] NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Description] [varchar](1024) NOT NULL,
	[UserName] [varchar](64) NOT NULL,
	[Date] [datetime2](0) NOT NULL,
	[CustemerName] [varchar](64) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[ZoomLevel] [bigint] NOT NULL,
	[Sequence] [bigint] NULL,
	[ClientName] [varchar](64) NOT NULL,
	[ContactName] [varchar](64) NOT NULL,
	[ClientCode] [varchar](64) NOT NULL,
	[Logo] [varchar](64) NULL,
	[AreaDescription] [varchar](128) NOT NULL,
	[IPAddress] [varchar](100) NULL,
	[OperationTime] [datetime2](0) NULL,
	[OperationUser] [varchar](64) NULL,
 CONSTRAINT [PK_campaigns_backup_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[campaignblockgroupimported]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaignblockgroupimported](
	[Id] [int] NOT NULL,
	[BlockGroupId] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Total] [int] NOT NULL,
	[Penetration] [int] NOT NULL,
 CONSTRAINT [PK_campaignblockgroupimported_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CampaignBlockGroupImported_BlockGroupId] ON [dbo].[campaignblockgroupimported] 
(
	[BlockGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CampaignBlockGroupImported_CampaignId] ON [dbo].[campaignblockgroupimported] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[blockgroupselectmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[blockgroupselectmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThreeZipAreaId] [int] NOT NULL,
	[FiveZipAreaId] [int] NOT NULL,
	[TractId] [int] NOT NULL,
	[BlockGroupId] [int] NOT NULL,
 CONSTRAINT [PK_blockgroupselectmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_BlockGroupSelectMappings_BlockGroup] ON [dbo].[blockgroupselectmappings] 
(
	[BlockGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_BlockGroupSelectMappings_FiveZipArea] ON [dbo].[blockgroupselectmappings] 
(
	[FiveZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_BlockGroupSelectMappings_ThreeZipArea] ON [dbo].[blockgroupselectmappings] 
(
	[ThreeZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_BlockGroupSelectMappings_Tract] ON [dbo].[blockgroupselectmappings] 
(
	[TractId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[blockgroups]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[blockgroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[CountyCode] [varchar](8) NOT NULL,
	[TractCode] [varchar](8) NOT NULL,
	[ArbitraryUniqueCode] [varchar](20) NOT NULL,
	[LSAD] [varchar](8) NULL,
	[LSADTrans] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_blockgroups_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[blockgroupcoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[blockgroupcoordinates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BlockGroupId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_blockgroupcoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[blockgroupboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[blockgroupboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[BlockGroupId] [int] NOT NULL,
 CONSTRAINT [PK_blockgroupboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_BLOCKGROUPBOXMAPPINGS_BLOCKGROUPID] ON [dbo].[blockgroupboxmappings] 
(
	[BlockGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_BLOCKGROUPBOXMAPPINGS_BOXID] ON [dbo].[blockgroupboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[customareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](32) NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[total] [int] NOT NULL,
	[Description] [varchar](256) NOT NULL,
 CONSTRAINT [PK_customareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[urbanareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[urbanareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_urbanareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[urbanareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[urbanareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UrbanAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_urbanareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_UrbanAreaCoordinates_UrbanAreaId] ON [dbo].[urbanareacoordinates] 
(
	[UrbanAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[urbanareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[urbanareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[UrbanAreaId] [int] NOT NULL,
 CONSTRAINT [PK_urbanareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_UrbanAreaBoxMappings_UrbanAreaId] ON [dbo].[urbanareaboxmappings] 
(
	[UrbanAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_UrbanAreaBoxMappings_BoxId] ON [dbo].[urbanareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[uppersenateareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[uppersenateareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[GEO_ID] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_uppersenateareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[uppersenateareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uppersenateareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UpperSenateAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_uppersenateareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_UpperSenateAreaCoordinates_UpperSenateAreaId] ON [dbo].[uppersenateareacoordinates] 
(
	[UpperSenateAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[uppersenateareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uppersenateareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[UpperSenateAreaId] [int] NOT NULL,
 CONSTRAINT [PK_uppersenateareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_UpperSenateAreaBoxMappings_UpperSenateAreaId] ON [dbo].[uppersenateareaboxmappings] 
(
	[UpperSenateAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_UpperSenateAreaBoxMappings_BoxId] ON [dbo].[uppersenateareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[unifiedschoolareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[unifiedschoolareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_unifiedschoolareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[unifiedschoolareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[unifiedschoolareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UnifiedSchoolAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_unifiedschoolareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_UnifiedSchoolAreaCoordinates_UnifiedSchoolAreaId] ON [dbo].[unifiedschoolareacoordinates] 
(
	[UnifiedSchoolAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[unifiedschoolareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[unifiedschoolareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[UnifiedSchoolAreaId] [int] NOT NULL,
 CONSTRAINT [PK_unifiedschoolareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_UnifiedSchoolAreaBoxMappings_UnifiedSchoolAreaId] ON [dbo].[unifiedschoolareaboxmappings] 
(
	[UnifiedSchoolAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_UnifiedSchoolAreaBoxMappings_BoxId] ON [dbo].[unifiedschoolareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tracts]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tracts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[CountyCode] [varchar](8) NOT NULL,
	[ArbitraryUniqueCode] [varchar](20) NOT NULL,
	[LSAD] [varchar](8) NULL,
	[LSADTrans] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_tracts_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_Tracts_ArbitraryUniqueCode] ON [dbo].[tracts] 
(
	[ArbitraryUniqueCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tractcoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tractcoordinates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TractId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_tractcoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_TractCoordinates_TractId] ON [dbo].[tractcoordinates] 
(
	[TractId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tractboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tractboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[TractId] [int] NOT NULL,
 CONSTRAINT [PK_tractboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_TractBoxMappings_TractId] ON [dbo].[tractboxmappings] 
(
	[TractId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_TractBoxMappings_BoxId] ON [dbo].[tractboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimmLog]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimmLog](
	[logtype] [varchar](50) NULL,
	[info] [varchar](max) NULL,
	[logTime] [datetime] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[threezipboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[threezipboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[ThreeZipAreaId] [int] NOT NULL,
 CONSTRAINT [PK_threezipboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_ThreeZipBoxMappings_ThreeZipAreaId] ON [dbo].[threezipboxmappings] 
(
	[ThreeZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_ThreeZipBoxMappings_BoxId] ON [dbo].[threezipboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[threezipareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[threezipareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[LSAD] [varchar](8) NULL,
	[LSADTrans] [varchar](64) NULL,
	[HOME_COUNT] [int] NOT NULL,
	[BUSINESS_COUNT] [int] NOT NULL,
	[APT_COUNT] [int] NOT NULL,
	[TOTAL_COUNT] [int] NOT NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_threezipareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[threezipareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[threezipareacoordinates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ThreeZipAreaId] [int] NOT NULL,
	[ShapeId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_threezipareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_ThreeZipAreaCoordinates_ThreeZipAreaId] ON [dbo].[threezipareacoordinates] 
(
	[ThreeZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[stateareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[stateareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[GEO_ID] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[StateUSPS] [varchar](8) NOT NULL,
	[Region] [varchar](8) NOT NULL,
	[Division] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
 CONSTRAINT [PK_stateareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[stateareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stateareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_stateareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_StateAreaCoordinates_StateAreaAreaId] ON [dbo].[stateareacoordinates] 
(
	[StateAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sequencecounters]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sequencecounters](
	[BlockGroup] [int] NOT NULL,
	[BlockGroupBoxMapping] [int] NOT NULL,
	[BlockGroupCoordinate] [int] NOT NULL,
	[BlockGroupSelectMapping] [int] NOT NULL,
	[CustomArea] [int] NOT NULL,
	[CustomAreaBoxMapping] [int] NOT NULL,
	[CustomAreaCoordinate] [int] NOT NULL,
	[NdAddress] [int] NOT NULL,
	[NdAddressBoxMapping] [int] NOT NULL,
	[NdAddressCoordinate] [int] NOT NULL,
	[SubMapCoordinate] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[secondaryschoolareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[secondaryschoolareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_secondaryschoolareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[secondaryschoolareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[secondaryschoolareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SecondarySchoolAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_secondaryschoolareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_SecondarySchoolAreaCoordinates_SecondarySchoolAreaId] ON [dbo].[secondaryschoolareacoordinates] 
(
	[SecondarySchoolAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[secondaryschoolareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[secondaryschoolareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[SecondarySchoolAreaId] [int] NOT NULL,
 CONSTRAINT [PK_secondaryschoolareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_SecondarySchoolAreaBoxMappings_SecondarySchoolAreaId] ON [dbo].[secondaryschoolareaboxmappings] 
(
	[SecondarySchoolAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_SecondarySchoolAreaBoxMappings_BoxId] ON [dbo].[secondaryschoolareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[radiusrecords]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[radiusrecords](
	[Id] [int] NOT NULL,
	[RadiusId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[Classification] [int] NOT NULL,
 CONSTRAINT [PK_radiusrecords_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_RadiusRecords_RadiusId] ON [dbo].[radiusrecords] 
(
	[RadiusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ndaddresses]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ndaddresses](
	[Id] [int] NOT NULL,
	[Street] [varchar](128) NOT NULL,
	[ZipCode] [varchar](32) NOT NULL,
	[Geofence] [int] NOT NULL,
	[Description] [varchar](256) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_ndaddresses_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[fivezipareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[fivezipareas](
	[Id] [int] NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSADTrans] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[HOME_COUNT] [int] NOT NULL,
	[BUSINESS_COUNT] [int] NOT NULL,
	[APT_COUNT] [int] NOT NULL,
	[TOTAL_COUNT] [int] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[HispanicSFDU] [int] NOT NULL,
	[HispanicMFDU] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_fivezipareas_new_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_FiveZipAreas_New_Code] ON [dbo].[fivezipareas] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[premiumcroutes]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[premiumcroutes](
	[Id] [int] NOT NULL,
	[FIPSSTCO] [varchar](8) NOT NULL,
	[GEOCODE] [varchar](16) NOT NULL,
	[ZIP] [varchar](8) NOT NULL,
	[CROUTE] [varchar](8) NOT NULL,
	[STATE_FIPS] [varchar](8) NOT NULL,
	[STATE] [varchar](8) NOT NULL,
	[COUNTY] [varchar](32) NOT NULL,
	[ZIP_NAME] [varchar](32) NOT NULL,
	[HOME_COUNT] [int] NULL,
	[BUSINESS_COUNT] [int] NULL,
	[APT_COUNT] [int] NULL,
	[TOTAL_COUNT] [int] NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[HispanicSFDU] [int] NOT NULL,
	[HispanicMFDU] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_premiumcroutes_new_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_PremiumCRoutes_GEOCODE_new] ON [dbo].[premiumcroutes] 
(
	[GEOCODE] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[votingdistrictareas]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[votingdistrictareas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](64) NOT NULL,
	[Code] [varchar](8) NOT NULL,
	[VTD] [varchar](8) NOT NULL,
	[StateCode] [varchar](8) NOT NULL,
	[CountyCode] [varchar](8) NOT NULL,
	[ALand] [bigint] NOT NULL,
	[AWater] [bigint] NOT NULL,
	[LSAD] [varchar](8) NOT NULL,
	[LSAD_TRAN] [varchar](64) NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsEnabled] [int] NOT NULL,
	[OTotal] [int] NULL,
	[Description] [varchar](256) NULL,
	[PartCount] [int] NOT NULL,
	[IsInnerShape] [int] NOT NULL,
	[IsInnerRing] [tinyint] NOT NULL,
	[SqMile] [float] NULL,
	[Area] [float] NULL,
	[geom] [geography] NULL,
	[polygon] [geometry] NULL,
	[NumOfRings] [int] NOT NULL,
 CONSTRAINT [PK_votingdistrictareas_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[votingdistrictareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[votingdistrictareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VotingDistrictAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_votingdistrictareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_VotingDistrictAreaCoordinates_VotingDistrictAreaId] ON [dbo].[votingdistrictareacoordinates] 
(
	[VotingDistrictAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[votingdistrictareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[votingdistrictareaboxmappings](
	[Id] [int] NOT NULL,
	[BoxId] [int] NOT NULL,
	[VotingDistrictAreaId] [int] NOT NULL,
 CONSTRAINT [PK_votingdistrictareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_VotingDistrictAreaBoxMappings_VotingDistrictAreaId] ON [dbo].[votingdistrictareaboxmappings] 
(
	[VotingDistrictAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_VotingDistrictAreaBoxMappings_BoxId] ON [dbo].[votingdistrictareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaignfivezipimported]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaignfivezipimported](
	[Id] [int] NOT NULL,
	[FiveZipAreaId] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Total] [int] NOT NULL,
	[Penetration] [int] NOT NULL,
	[PartPercentage] [real] NULL,
	[IsPartModified] [smallint] NULL,
 CONSTRAINT [PK_campaignfivezipimported_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CampaignFiveZipImported_Campaign] ON [dbo].[campaignfivezipimported] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CampaignFiveZipImported_FiveZipArea] ON [dbo].[campaignfivezipimported] 
(
	[FiveZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[campaigncrouteimported]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[campaigncrouteimported](
	[Id] [int] NOT NULL,
	[PremiumCRouteId] [int] NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Total] [int] NOT NULL,
	[Penetration] [int] NOT NULL,
	[PartPercentage] [real] NULL,
	[IsPartModified] [smallint] NULL,
 CONSTRAINT [PK_campaigncrouteimported_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CampaignCRouteImported_CampaignId] ON [dbo].[campaigncrouteimported] 
(
	[CampaignId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CampaignCRouteImported_PremiumCRouteId] ON [dbo].[campaigncrouteimported] 
(
	[PremiumCRouteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[premiumcrouteselectmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[premiumcrouteselectmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThreeZipAreaId] [int] NOT NULL,
	[FiveZipAreaId] [int] NOT NULL,
	[PremiumCRouteId] [int] NOT NULL,
 CONSTRAINT [PK_premiumcrouteselectmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[premiumcroutecoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[premiumcroutecoordinates](
	[Id] [bigint] NOT NULL,
	[PreminumCRouteId] [int] NOT NULL,
	[ShapeId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_premiumcroutecoordinates_new_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_PremiumCRouteCoordinates_PremiumCRoutes_new] ON [dbo].[premiumcroutecoordinates] 
(
	[PreminumCRouteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[premiumcrouteboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[premiumcrouteboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[PreminumCRouteId] [int] NOT NULL,
 CONSTRAINT [PK_premiumcrouteboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[fivezipareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fivezipareacoordinates](
	[Id] [bigint] NOT NULL,
	[FiveZipAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[ShapeId] [int] NOT NULL,
 CONSTRAINT [PK_fivezipareacoordinates_new_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_FiveZipAreaCoordinates_New_FiveZipAreaId] ON [dbo].[fivezipareacoordinates] 
(
	[FiveZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ndaddresscoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ndaddresscoordinates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NdAddressId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_ndaddresscoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_NdAddressCoordinates_NdAddressId] ON [dbo].[ndaddresscoordinates] 
(
	[NdAddressId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ndaddressboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ndaddressboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[NdAddressId] [int] NOT NULL,
 CONSTRAINT [PK_ndaddressboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_NdAddressBoxMappings_NdAddressId] ON [dbo].[ndaddressboxmappings] 
(
	[NdAddressId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_NdAddressBoxMappings_BoxId] ON [dbo].[ndaddressboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[customareacoordinates]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customareacoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_customareacoordinates_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CUSTOMAREACOORDINATES_CUSTOMAREAID] ON [dbo].[customareacoordinates] 
(
	[CustomAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[customareaboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customareaboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[CustomAreaId] [int] NOT NULL,
 CONSTRAINT [PK_customareaboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_CUSTOMAREABOXMAPPINGS_CUSTOMAREAID] ON [dbo].[customareaboxmappings] 
(
	[CustomAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CUSTOMAREABOXMAPPINGS_BOXID] ON [dbo].[customareaboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[fivezipboxmappings]    Script Date: 09/26/2011 21:00:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fivezipboxmappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[FiveZipAreaId] [int] NOT NULL,
 CONSTRAINT [PK_fivezipboxmappings_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_FiveZipBoxMappings_FiveZipAreaId] ON [dbo].[fivezipboxmappings] 
(
	[FiveZipAreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_FiveZipBoxMappings_BoxId] ON [dbo].[fivezipboxmappings] 
(
	[BoxId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF__campaignc__PartP__0DAF0CB0]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigncrouteimported] ADD  DEFAULT (NULL) FOR [PartPercentage]
GO
/****** Object:  Default [DF__campaignc__IsPar__0EA330E9]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigncrouteimported] ADD  DEFAULT (NULL) FOR [IsPartModified]
GO
/****** Object:  Default [DF__campaignf__PartP__108B795B]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaignfivezipimported] ADD  DEFAULT (NULL) FOR [PartPercentage]
GO
/****** Object:  Default [DF__campaignf__IsPar__117F9D94]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaignfivezipimported] ADD  DEFAULT (NULL) FOR [IsPartModified]
GO
/****** Object:  Default [DF__campaigns__Seque__182C9B23]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigns_backup] ADD  DEFAULT (NULL) FOR [Sequence]
GO
/****** Object:  Default [DF__campaigns___Logo__1920BF5C]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigns_backup] ADD  DEFAULT (NULL) FOR [Logo]
GO
/****** Object:  Default [DF__campaigns__IPAdd__1A14E395]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigns_backup] ADD  DEFAULT (NULL) FOR [IPAddress]
GO
/****** Object:  Default [DF__campaigns__Opera__1B0907CE]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigns_backup] ADD  DEFAULT (NULL) FOR [OperationTime]
GO
/****** Object:  Default [DF__campaigns__Opera__1BFD2C07]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[campaigns_backup] ADD  DEFAULT (NULL) FOR [OperationUser]
GO
/****** Object:  Default [DF__countyare__ALand__341F99B2]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[countyareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__countyare__AWate__3513BDEB]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[countyareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__elementar__ALand__563FA78C]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[elementaryschoolareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__elementar__AWate__5733CBC5]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[elementaryschoolareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__elementar__LSAD___2A61254E]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[elementaryschoolareas] ADD  DEFAULT (NULL) FOR [LSAD_TRAN]
GO
/****** Object:  Default [DF__fivezipar__LSADT__60E75331]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT (NULL) FOR [LSADTrans]
GO
/****** Object:  Default [DF__fivezipar__OTota__61DB776A]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT (NULL) FOR [OTotal]
GO
/****** Object:  Default [DF__fivezipar__Descr__62CF9BA3]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT (NULL) FOR [Description]
GO
/****** Object:  Default [DF__fivezipar__PartC__63C3BFDC]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [PartCount]
GO
/****** Object:  Default [DF__fivezipar__IsInn__64B7E415]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [IsInnerShape]
GO
/****** Object:  Default [DF__fivezipar__HOME___65AC084E]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [HOME_COUNT]
GO
/****** Object:  Default [DF__fivezipar__BUSIN__66A02C87]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [BUSINESS_COUNT]
GO
/****** Object:  Default [DF__fivezipar__APT_C__679450C0]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [APT_COUNT]
GO
/****** Object:  Default [DF__fivezipar__TOTAL__688874F9]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [TOTAL_COUNT]
GO
/****** Object:  Default [DF__fivezipar__Hispa__56F3D4A3]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [HispanicSFDU]
GO
/****** Object:  Default [DF__fivezipar__Hispa__57E7F8DC]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [HispanicMFDU]
GO
/****** Object:  Default [DF__fivezipar__IsInn__735B0927]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareas] ADD  DEFAULT ((0)) FOR [IsInnerRing]
GO
/****** Object:  Default [DF__lowerhous__ALand__5BF880E2]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[lowerhouseareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__lowerhous__AWate__5CECA51B]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[lowerhouseareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__metropoli__ALand__61B15A38]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[metropolitancoreareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__metropoli__AWate__62A57E71]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[metropolitancoreareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__ndaddress__Descr__6B24EA82]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[ndaddresses] ADD  DEFAULT (NULL) FOR [Description]
GO
/****** Object:  Default [DF__premiumcr__PartC__5B2E79DB]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcroutes] ADD  DEFAULT ((0)) FOR [PartCount]
GO
/****** Object:  Default [DF__premiumcr__IsInn__5C229E14]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcroutes] ADD  DEFAULT ((0)) FOR [IsInnerShape]
GO
/****** Object:  Default [DF__premiumcr__Hispa__5AC46587]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcroutes] ADD  DEFAULT ((0)) FOR [HispanicSFDU]
GO
/****** Object:  Default [DF__premiumcr__Hispa__5BB889C0]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcroutes] ADD  DEFAULT ((0)) FOR [HispanicMFDU]
GO
/****** Object:  Default [DF__premiumcr__IsInn__7266E4EE]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcroutes] ADD  DEFAULT ((0)) FOR [IsInnerRing]
GO
/****** Object:  Default [DF__secondary__ALand__676A338E]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[secondaryschoolareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__secondary__AWate__685E57C7]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[secondaryschoolareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__statearea__ALand__6D230CE4]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[stateareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__statearea__AWate__6E17311D]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[stateareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__unifiedsc__ALand__72DBE63A]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[unifiedschoolareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__unifiedsc__AWate__73D00A73]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[unifiedschoolareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__uppersena__ALand__7894BF90]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[uppersenateareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__uppersena__AWate__7988E3C9]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[uppersenateareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__urbanarea__ALand__7E4D98E6]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[urbanareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__urbanarea__AWate__7F41BD1F]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[urbanareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  Default [DF__votingdis__ALand__0406723C]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[votingdistrictareas] ADD  DEFAULT ((0)) FOR [ALand]
GO
/****** Object:  Default [DF__votingdis__AWate__04FA9675]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[votingdistrictareas] ADD  DEFAULT ((0)) FOR [AWater]
GO
/****** Object:  ForeignKey [customareaboxmappings$FK_CUSTOMAREABOXMAPPINGS_CUSTOMAREAID]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[customareaboxmappings]  WITH NOCHECK ADD  CONSTRAINT [customareaboxmappings$FK_CUSTOMAREABOXMAPPINGS_CUSTOMAREAID] FOREIGN KEY([CustomAreaId])
REFERENCES [dbo].[customareas] ([Id])
GO
ALTER TABLE [dbo].[customareaboxmappings] CHECK CONSTRAINT [customareaboxmappings$FK_CUSTOMAREABOXMAPPINGS_CUSTOMAREAID]
GO
/****** Object:  ForeignKey [customareacoordinates$FK_CUSTOMAREACOORDINATES_CUSTOMAREAID]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[customareacoordinates]  WITH NOCHECK ADD  CONSTRAINT [customareacoordinates$FK_CUSTOMAREACOORDINATES_CUSTOMAREAID] FOREIGN KEY([CustomAreaId])
REFERENCES [dbo].[customareas] ([Id])
GO
ALTER TABLE [dbo].[customareacoordinates] CHECK CONSTRAINT [customareacoordinates$FK_CUSTOMAREACOORDINATES_CUSTOMAREAID]
GO
/****** Object:  ForeignKey [FK_fivezipareacoordinates_fivezipareas]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipareacoordinates]  WITH CHECK ADD  CONSTRAINT [FK_fivezipareacoordinates_fivezipareas] FOREIGN KEY([FiveZipAreaId])
REFERENCES [dbo].[fivezipareas] ([Id])
GO
ALTER TABLE [dbo].[fivezipareacoordinates] CHECK CONSTRAINT [FK_fivezipareacoordinates_fivezipareas]
GO
/****** Object:  ForeignKey [FK_fivezipboxmappings_fivezipareas]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[fivezipboxmappings]  WITH CHECK ADD  CONSTRAINT [FK_fivezipboxmappings_fivezipareas] FOREIGN KEY([FiveZipAreaId])
REFERENCES [dbo].[fivezipareas] ([Id])
GO
ALTER TABLE [dbo].[fivezipboxmappings] CHECK CONSTRAINT [FK_fivezipboxmappings_fivezipareas]
GO
/****** Object:  ForeignKey [ndaddressboxmappings$FK_NdAddressBoxMappings_NdAddressId]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[ndaddressboxmappings]  WITH NOCHECK ADD  CONSTRAINT [ndaddressboxmappings$FK_NdAddressBoxMappings_NdAddressId] FOREIGN KEY([NdAddressId])
REFERENCES [dbo].[ndaddresses] ([Id])
GO
ALTER TABLE [dbo].[ndaddressboxmappings] CHECK CONSTRAINT [ndaddressboxmappings$FK_NdAddressBoxMappings_NdAddressId]
GO
/****** Object:  ForeignKey [ndaddresscoordinates$FK_NdAddressCoordinates_NdAddressId]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[ndaddresscoordinates]  WITH NOCHECK ADD  CONSTRAINT [ndaddresscoordinates$FK_NdAddressCoordinates_NdAddressId] FOREIGN KEY([NdAddressId])
REFERENCES [dbo].[ndaddresses] ([Id])
GO
ALTER TABLE [dbo].[ndaddresscoordinates] CHECK CONSTRAINT [ndaddresscoordinates$FK_NdAddressCoordinates_NdAddressId]
GO
/****** Object:  ForeignKey [FK_premiumcrouteboxmappings_premiumcroutes]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcrouteboxmappings]  WITH CHECK ADD  CONSTRAINT [FK_premiumcrouteboxmappings_premiumcroutes] FOREIGN KEY([PreminumCRouteId])
REFERENCES [dbo].[premiumcroutes] ([Id])
GO
ALTER TABLE [dbo].[premiumcrouteboxmappings] CHECK CONSTRAINT [FK_premiumcrouteboxmappings_premiumcroutes]
GO
/****** Object:  ForeignKey [FK_premiumcroutecoordinates_premiumcroutes]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcroutecoordinates]  WITH CHECK ADD  CONSTRAINT [FK_premiumcroutecoordinates_premiumcroutes] FOREIGN KEY([PreminumCRouteId])
REFERENCES [dbo].[premiumcroutes] ([Id])
GO
ALTER TABLE [dbo].[premiumcroutecoordinates] CHECK CONSTRAINT [FK_premiumcroutecoordinates_premiumcroutes]
GO
/****** Object:  ForeignKey [FK_premiumcrouteselectmappings_fivezipareas]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcrouteselectmappings]  WITH CHECK ADD  CONSTRAINT [FK_premiumcrouteselectmappings_fivezipareas] FOREIGN KEY([FiveZipAreaId])
REFERENCES [dbo].[fivezipareas] ([Id])
GO
ALTER TABLE [dbo].[premiumcrouteselectmappings] CHECK CONSTRAINT [FK_premiumcrouteselectmappings_fivezipareas]
GO
/****** Object:  ForeignKey [FK_premiumcrouteselectmappings_premiumcroutes]    Script Date: 09/26/2011 21:00:48 ******/
ALTER TABLE [dbo].[premiumcrouteselectmappings]  WITH CHECK ADD  CONSTRAINT [FK_premiumcrouteselectmappings_premiumcroutes] FOREIGN KEY([PremiumCRouteId])
REFERENCES [dbo].[premiumcroutes] ([Id])
GO
ALTER TABLE [dbo].[premiumcrouteselectmappings] CHECK CONSTRAINT [FK_premiumcrouteselectmappings_premiumcroutes]
GO

