
GO
/****** Object:  Table [dbo].[Colors]    Script Date: 02/08/2010 11:06:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Colors](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[HtmlValue] [nvarchar](8) NOT NULL,
	[R] [float] NOT NULL,
	[G] [float] NOT NULL,
	[B] [float] NOT NULL,
	[A] [float] NOT NULL,
 CONSTRAINT [PK_Colors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CampaignPercentageColors]    Script Date: 02/08/2010 11:06:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CampaignPercentageColors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CampaignId] [int] NOT NULL,
	[Min] [float] NOT NULL,
	[Max] [float] NOT NULL,
	[ColorId] [int] NOT NULL,
 CONSTRAINT [PK_CampaignPercentageColors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_CampaignPercentageColors_Campaigns]    Script Date: 02/08/2010 11:06:19 ******/
ALTER TABLE [dbo].[CampaignPercentageColors]  WITH CHECK ADD  CONSTRAINT [FK_CampaignPercentageColors_Campaigns] FOREIGN KEY([CampaignId])
REFERENCES [dbo].[Campaigns] ([Id])
GO
ALTER TABLE [dbo].[CampaignPercentageColors] CHECK CONSTRAINT [FK_CampaignPercentageColors_Campaigns]
GO
/****** Object:  ForeignKey [FK_CampaignPercentageColors_Colors]    Script Date: 02/08/2010 11:06:19 ******/
ALTER TABLE [dbo].[CampaignPercentageColors]  WITH CHECK ADD  CONSTRAINT [FK_CampaignPercentageColors_Colors] FOREIGN KEY([ColorId])
REFERENCES [dbo].[Colors] ([Id])
GO
ALTER TABLE [dbo].[CampaignPercentageColors] CHECK CONSTRAINT [FK_CampaignPercentageColors_Colors]
GO
INSERT INTO [Colors] VALUES (1,'Blue','0000FF',0,0,255,0.3)
INSERT INTO [Colors] VALUES (2,'Green','008000',0,128,0,0.3)
INSERT INTO [Colors] VALUES (3,'Yellow','FFFF00',255,255,0,0.3)
INSERT INTO [Colors] VALUES (4,'Orange','F75600',247,86,0,0.3)
INSERT INTO [Colors] VALUES (5,'Red','BB0000',187,0,0,0.3)
go