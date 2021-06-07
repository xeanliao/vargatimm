
GO

CREATE TABLE [dbo].[NdAddresses](
	[Id] [int] NOT NULL,
	[Street] [nvarchar](128) NOT NULL,
	[ZipCode] [nvarchar](32) NOT NULL,
	[Geofence] [int] NOT NULL,
	[Description] [nvarchar](256) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_NdAddresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[NdAddressCoordinates](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NdAddressId] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_NdAddressCoordinates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NdAddressCoordinates]  WITH CHECK ADD  CONSTRAINT [FK_NdAddressCoordinates_NdAddresses] FOREIGN KEY([NdAddressId])
REFERENCES [dbo].[NdAddresses] ([Id])
GO

ALTER TABLE [dbo].[NdAddressCoordinates] CHECK CONSTRAINT [FK_NdAddressCoordinates_NdAddresses]
GO

CREATE TABLE [dbo].[NdAddressBoxMappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoxId] [int] NOT NULL,
	[NdAddressId] [int] NOT NULL,
 CONSTRAINT [PK_NdAddressBoxMappings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NdAddressBoxMappings]  WITH CHECK ADD  CONSTRAINT [FK_NdAddressBoxMappings_NdAddresses] FOREIGN KEY([NdAddressId])
REFERENCES [dbo].[NdAddresses] ([Id])
GO

ALTER TABLE [dbo].[NdAddressBoxMappings] CHECK CONSTRAINT [FK_NdAddressBoxMappings_NdAddresses]
GO

