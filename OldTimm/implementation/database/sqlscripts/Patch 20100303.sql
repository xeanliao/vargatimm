
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CustomAreas](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[total] [int] NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_CustomArea] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[CustomAreaCoordinates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomAreaId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_CustomAreaCoordinates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CustomAreaCoordinates]  WITH CHECK ADD  CONSTRAINT [FK_CustomAreaCoordinates_CustomAreas] FOREIGN KEY([CustomAreaId])
REFERENCES [dbo].[CustomAreas] ([Id])
GO

ALTER TABLE [dbo].[CustomAreaCoordinates] CHECK CONSTRAINT [FK_CustomAreaCoordinates_CustomAreas]
GO
GO

CREATE TABLE [dbo].[CustomAreaBoxMappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[CustomAreaId] [int] NOT NULL,
 CONSTRAINT [PK_CustomAreaBoxMappings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CustomAreaBoxMappings]  WITH CHECK ADD  CONSTRAINT [FK_CustomAreaBoxMappings_CustomAreas] FOREIGN KEY([CustomAreaId])
REFERENCES [dbo].[CustomAreas] ([Id])
GO

ALTER TABLE [dbo].[CustomAreaBoxMappings] CHECK CONSTRAINT [FK_CustomAreaBoxMappings_CustomAreas]
GO





