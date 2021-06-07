USE [timm_redesign]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetBoxIds]    Script Date: 08/29/2011 20:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Kyoung Seop Shin>
-- Create date: <7/25/2011>
-- Description:	<Get BoxIDs>
-- =============================================
CREATE FUNCTION [dbo].[fn_GetBoxIds] (
@MinLatitude FLOAT,
@MaxLatitude FLOAT,
@MinLongitude FLOAT,
@MaxLongitude FLOAT,
@MountLat INT,
@MountLon INT
)
RETURNS @t_boxmappings TABLE (BoxId INT)
AS
BEGIN
	--SELECT * FROM dbo.fn_GetBoxIds(18.12984, 18.20209, -66.75859, -66.70747, 3, 4);
	DECLARE @v_MinLat INT;
	DECLARE @v_MinLon INT;
	DECLARE @v_temp FLOAT;
	DECLARE @v_tempLat INT;
	DECLARE @v_tempLon INT;
	
	SET @v_MinLat = CAST(FLOOR(@MinLatitude * 100) AS INT);
	SET @v_MinLat = @v_MinLat - (@v_MinLat % @MountLat);
	
	IF @MinLatitude < 0
		SET @v_MinLat =  @v_MinLat - @MountLat;

	IF @MinLongitude < -170 AND @MaxLongitude > 170
	BEGIN
		SET @v_temp = @MinLongitude;
		SET @MinLongitude = @MaxLongitude;
		SET @MaxLongitude = @v_temp + 360;
	END

	SET @v_MinLon = CAST(FLOOR(@MinLongitude * 100) AS INT);
	SET @v_MinLon = @v_MinLon - (@v_MinLon % @MountLon);
	
	IF @MinLongitude < 0
		SET @v_MinLon = @v_MinLon - @MountLon;

	SET @v_tempLat = @v_MinLat;
	WHILE @v_tempLat < @MaxLatitude * 100
	BEGIN
		SET @v_tempLon = @v_MinLon;
		WHILE @v_tempLon < @MaxLongitude * 100
		BEGIN
			INSERT INTO @t_boxmappings (BoxID) VALUES (@v_tempLat * 100000 + @v_tempLon);
			SET @v_tempLon = @v_tempLon + @MountLon;
		END
        SET @v_tempLat = @v_tempLat + @MountLat;
	END
	RETURN
END
GO
/****** Object:  Table [dbo].[AreaType]    Script Date: 08/29/2011 20:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AreaType](
	[AreaTypeID] [tinyint] NOT NULL,
	[AreaTypeName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_AreaType] PRIMARY KEY CLUSTERED 
(
	[AreaTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PostalArea]    Script Date: 08/29/2011 20:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PostalArea](
	[Version] [smallint] NOT NULL,
	[AreaTypeID] [tinyint] NOT NULL,
	[AreaID] [int] NOT NULL,
	[AreaCode] [varchar](50) NOT NULL,
	[AreaCodeGroup] [varchar](50) NULL,
	[AreaFIPSST] [varchar](2) NOT NULL,
	[AreaFIPSCO] [varchar](3) NOT NULL,
	[AreaDescription] [varchar](255) NULL,
	[NumOfHome] [int] NULL,
	[NumOfBusiness] [int] NULL,
	[NumOfAPT] [int] NULL,
	[NumOfTotal] [int] NULL,
	[NumOfShape] [int] NOT NULL,
	[NumOfRing] [int] NOT NULL,
	[AreaGeography] [geography] NOT NULL,
	[AreaGeometry] [geometry] NOT NULL,
 CONSTRAINT [PK_PostalArea] PRIMARY KEY CLUSTERED 
(
	[Version] ASC,
	[AreaTypeID] ASC,
	[AreaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_PostalArea] UNIQUE NONCLUSTERED 
(
	[Version] ASC,
	[AreaTypeID] ASC,
	[AreaCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[usp_AggregateStats]    Script Date: 08/29/2011 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Kyoung Seop Shin>
-- Create date: <8/17/2011>
-- Description:	<Update Aggregated Numbers for ZIP>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AggregateStats]
@Version SMALLINT
AS
BEGIN
	--EXEC dbo.usp_AggregateStats 1107;
	SET NOCOUNT ON;

	UPDATE dbo.PostalArea
		SET NumOfHome = p.NumOfHome,
		NumOfBusiness = p.NumOfBusiness,
		NumOfAPT = p.NumOfAPT,
		NumOfTotal = p.NumOfTotal
		FROM dbo.PostalArea AS pa
		INNER JOIN (
			SELECT Version,
				2 AS AreaTypeID,
				AreaCodeGroup,
				SUM(NumOfHome) AS NumOfHome,
				SUM(NumOfBusiness) AS NumOfBusiness,
				SUM(NumOfAPT) AS NumOfAPT,
				SUM(NumOfTotal) AS NumOfTotal
            FROM dbo.PostalArea
            WHERE Version = @Version
            AND AreaTypeID = 3
			GROUP BY Version,
				AreaCodeGroup
		) AS p ON pa.Version = p.Version
		AND pa.AreaTypeID = p.AreaTypeID
		AND pa.AreaCode = p.AreaCodeGroup;
		
	UPDATE dbo.PostalArea
		SET NumOfHome = p.NumOfHome,
		NumOfBusiness = p.NumOfBusiness,
		NumOfAPT = p.NumOfAPT,
		NumOfTotal = p.NumOfTotal
		FROM dbo.PostalArea AS pa
		INNER JOIN (
			SELECT Version,
				1 AS AreaTypeID,
				AreaCodeGroup,
				SUM(NumOfHome) AS NumOfHome,
				SUM(NumOfBusiness) AS NumOfBusiness,
				SUM(NumOfAPT) AS NumOfAPT,
				SUM(NumOfTotal) AS NumOfTotal
            FROM dbo.PostalArea
            WHERE Version = @Version
            AND AreaTypeID = 2
			GROUP BY Version,
				AreaCodeGroup
		) AS p ON pa.Version = p.Version
		AND pa.AreaTypeID = p.AreaTypeID
		AND pa.AreaCode = p.AreaCodeGroup;
END
GO
/****** Object:  Table [dbo].[PostalShape]    Script Date: 08/29/2011 20:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostalShape](
	[Version] [smallint] NOT NULL,
	[AreaTypeID] [tinyint] NOT NULL,
	[AreaID] [int] NOT NULL,
	[ShapeID] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentShapeID] [bigint] NULL,
	[MinLatitude] [float] NOT NULL,
	[MaxLatitude] [float] NOT NULL,
	[MinLongitude] [float] NOT NULL,
	[MaxLongitude] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[SqMile] [float] NOT NULL,
	[IsInnerRing] [bit] NOT NULL,
	[ShapeGeography] [geography] NOT NULL,
	[ShapeGeometry] [geometry] NOT NULL,
 CONSTRAINT [PK_PostalShape] PRIMARY KEY CLUSTERED 
(
	[ShapeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PostalShape_Version_AreaTypeID] ON [dbo].[PostalShape] 
(
	[Version] ASC,
	[AreaTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PostalShapeCoordinate]    Script Date: 08/29/2011 20:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostalShapeCoordinate](
	[ShapeID] [bigint] NOT NULL,
	[ShapeOrder] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_PostalShapeCoordinate] PRIMARY KEY CLUSTERED 
(
	[ShapeID] ASC,
	[ShapeOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PostalShapeCooordinate_ShapeID_Lat_Lon] ON [dbo].[PostalShapeCoordinate] 
(
	[ShapeID] ASC,
	[Latitude] ASC,
	[Longitude] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PostalShapeBox]    Script Date: 08/29/2011 20:19:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostalShapeBox](
	[ShapeID] [bigint] NOT NULL,
	[BoxID] [int] NOT NULL,
 CONSTRAINT [PK_PostalShapeBox] PRIMARY KEY CLUSTERED 
(
	[ShapeID] ASC,
	[BoxID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[usp_ProcessCROUTE]    Script Date: 08/29/2011 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Kyoung Shin>
-- Create date: <08/14/2011>
-- Description:	<Process CROUTE From Staging To Production>
-- =============================================
CREATE PROCEDURE [dbo].[usp_ProcessCROUTE]
@Version SMALLINT
AS
BEGIN
	--EXEC dbo.usp_ProcessCROUTE 1107;
	SET NOCOUNT ON;

	DECLARE @v_ID INT;
	DECLARE @v_geom GEOGRAPHY;
	DECLARE @v_count INT;
	DECLARE @v_count_ring INT;
	DECLARE @v_count_total INT;
	DECLARE @v_g GEOMETRY;

	--Get 3 ZIP
	DECLARE c_CROUTE CURSOR FAST_FORWARD LOCAL FOR
		SELECT ID, geom FROM premium.dbo.CROUTE
			ORDER BY ID;
	OPEN c_CROUTE;
	FETCH NEXT FROM c_CROUTE INTO @v_ID, @v_geom;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			--Get Total Count
			SET @v_count = 1;
			SET @v_count_total = 0;
			WHILE @v_count <= @v_geom.STNumGeometries()
			BEGIN
				SET @v_count_ring = 1;
				WHILE @v_count_ring <= @v_geom.STGeometryN(@v_count).NumRings()
				BEGIN
					SET @v_count_total = @v_count_total + 1;
					SET @v_count_ring = @v_count_ring + 1;
				END
				SET @v_count = @v_count + 1;
			END

			--Get Area
			SET @v_g = geometry::STGeomFromText(@v_geom.ToString(), 4326);
			INSERT INTO dbo.PostalArea (
				Version, AreaTypeID, AreaID, AreaCode, AreaCodeGroup,
				AreaFIPSST, AreaFIPSCO, AreaDescription,
				NumOfHome, NumOfBusiness, NumOfAPT, NumOfTotal,
				NumOfShape, NumOfRing, AreaGeography, AreaGeometry
			) SELECT @Version, 3, @v_ID, GEOCODE, ZIP,
				LEFT(FIPSSTCO, 2), RIGHT(FIPSSTCO, 3), 'CROUTE',
				HOME_COUNT, BUSINESS_C, APT_COUNT, TOTAL_COUN,
				0, @v_count_total, @v_geom, @v_g
				FROM premium.dbo.CROUTE
				WHERE ID = @v_ID;
				
			--Get Geometry (Polygon, Ring)
			SET @v_count = 1;
			WHILE @v_count <= @v_geom.STNumGeometries()
			BEGIN
				SET @v_count_ring = 1;
				WHILE @v_count_ring <= @v_geom.STGeometryN(@v_count).NumRings()
				BEGIN
					SET @v_g = geometry::STGeomFromText(
						REPLACE(REPLACE(@v_geom.STGeometryN(@v_count).RingN(@v_count_ring).ToString(),
							'LINESTRING (', 'POLYGON (('), ')', '))'), 4326);
					IF @v_g.STIsValid() <> 1
						SET @v_g = @v_g.MakeValid();
				
					INSERT INTO dbo.PostalShape (
						Version, AreaTypeID, AreaID,
						MinLatitude, MaxLatitude, MinLongitude, MaxLongitude,
						Latitude, Longitude,
						SqMile, IsInnerRing,
						ShapeGeography, ShapeGeometry
					) SELECT @Version, 3, @v_ID,
						0, 0, 0, 0,
						ROUND(@v_g.STCentroid().STY, 5), ROUND(@v_g.STCentroid().STX, 5),
						@v_geom.STGeometryN(@v_count).STArea() * 3.86102159 * POWER(10.00000000, -7), (CASE WHEN @v_count_ring = 1 THEN 0 ELSE 1 END),
						@v_geom.STGeometryN(@v_count).RingN(@v_count_ring), @v_g;

					SET @v_count_ring = @v_count_ring + 1;
				END
				SET @v_count = @v_count + 1;
			END
		END TRY
		BEGIN CATCH
			PRINT 'Error ID: ' + CAST(@v_ID AS VARCHAR(50));
		END CATCH

		FETCH NEXT FROM c_CROUTE INTO @v_ID, @v_geom;
	END
	CLOSE c_CROUTE;
	DEALLOCATE c_CROUTE;

	--Get Coordinates (Point)
	DECLARE c_CROUTE CURSOR FAST_FORWARD LOCAL FOR
		SELECT ShapeID, ShapeGeography FROM dbo.PostalShape
			WHERE Version = @Version
			AND AreaTypeID = 3
			ORDER BY ShapeID;
	OPEN c_CROUTE;
	FETCH NEXT FROM c_CROUTE INTO @v_ID, @v_geom;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @v_count = 1;
		WHILE @v_count <= @v_geom.STNumPoints()
		BEGIN
			SET @v_g = geometry::STGeomFromText(@v_geom.STPointN(@v_geom.STNumPoints() - @v_count + 1).ToString(), 4326);
			INSERT INTO dbo.PostalShapeCoordinate (
				ShapeID, ShapeOrder, Latitude, Longitude
			) SELECT @v_ID, @v_count, ROUND(@v_g.STY, 5), ROUND(@v_g.STX, 5);
			
			SET @v_count = @v_count + 1;
		END
		FETCH NEXT FROM c_CROUTE INTO @v_ID, @v_geom;
	END
	CLOSE c_CROUTE;
	DEALLOCATE c_CROUTE;	

	--Update NumOfShape
	UPDATE PostalArea
		SET NumOfShape = pag.NumOfShape
		FROM PostalArea AS pa INNER JOIN (
			SELECT Version, AreaTypEID, AreaID, COUNT(*) AS NumOfShape
				FROM dbo.PostalShape
				WHERE IsInnerRing = 0
				AND Version = @Version
				AND AreaTypeID = 3
				GROUP BY Version, AreaTypEID, AreaID
		) AS pag
		ON pa.Version = pag.Version AND pa.AreaTypeID = pag.AreaTypeID AND pa.AreaID = pag.AreaID
		WHERE pa.Version = @Version
		AND pa.AreaTypeID = 3;

	--Update Bounding Box
	UPDATE dbo.PostalShape
		SET MinLatitude = ROUND(psc.MinLatitude, 5),
			MaxLatitude = ROUND(psc.MaxLatitude, 5),
			MinLongitude = ROUND(psc.MinLongitude, 5),
			MaxLongitude = ROUND(psc.MaxLongitude, 5)
		FROM dbo.PostalShape AS ps
		INNER JOIN (
			SELECT ShapeID,
				MIN(Latitude) AS MinLatitude,
				MAX(Latitude) AS MaxLatitude,
				MIN(Longitude) AS MinLongitude,
				MAX(Longitude) AS MaxLongitude
				FROM dbo.PostalShapeCoordinate
			GROUP BY ShapeID
		) AS psc
		ON ps.ShapeID = psc.ShapeID
		WHERE ps.Version = @Version
		AND ps.AreaTypeID = 3;

	--Get BoxID
	DECLARE @v_AreaID INT;
	DECLARE @v_MinLatitude FLOAT;
	DECLARE @v_MaxLatitude FLOAT;
	DECLARE @v_MinLongitude FLOAT;
	DECLARE @v_MaxLongitude FLOAT;
	DECLARE @v_MountLat INT;
	DECLARE @v_MountLon INT;

	SET @v_MountLat = 3;
	SET @v_MountLon = 4;

	DECLARE c_AREA CURSOR FAST_FORWARD LOCAL FOR
		SELECT ShapeID, MinLatitude, MaxLatitude, MinLongitude, MaxLongitude
			FROM dbo.PostalShape
			WHERE Version = @Version
			AND AreaTypeID = 3
			ORDER BY ShapeID;
	OPEN c_AREA;
	FETCH NEXT FROM c_AREA INTO @v_AreaID, @v_MinLatitude, @v_MaxLatitude, @v_MinLongitude, @v_MaxLongitude;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO dbo.PostalShapeBox (ShapeID, BoxID)
			SELECT @v_AreaID, BoxID
			FROM dbo.fn_GetBoxIds(
				@v_MinLatitude, @v_MaxLatitude,
				@v_MinLongitude, @v_MaxLongitude,
				@v_MountLat, @v_MountLon
			);
		FETCH NEXT FROM c_AREA INTO @v_AreaID, @v_MinLatitude, @v_MaxLatitude, @v_MinLongitude, @v_MaxLongitude;
	END
	CLOSE c_AREA;
	DEALLOCATE c_AREA;

	--Update Parent Polygon ID for Inner Ring
	CREATE SPATIAL INDEX IX_PostalShape_ShapeGeometry
	ON dbo.PostalShape (
		ShapeGeometry
	)USING  GEOMETRY_GRID 
	WITH (BOUNDING_BOX =(-180, -90, 180, 90), GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
		CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON
	);
	UPDATE dbo.PostalShape
		SET ParentShapeID = psp.ShapeID
		FROM dbo.PostalShape
		INNER JOIN dbo.PostalShape AS psp
		ON psp.ShapeGeometry.STContains(PostalShape.ShapeGeometry) = 1
		AND PostalShape.Version = psp.Version
		AND PostalShape.AreaTypeID = psp.AreaTypeID
		AND PostalShape.ShapeId <> psp.ShapeID
		WHERE PostalShape.Version = @Version
		AND PostalShape.AreaTypeID = 3;
	DROP INDEX IX_PostalShape_ShapeGeometry ON dbo.PostalShape;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Process5ZIP]    Script Date: 08/29/2011 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Kyoung Shin>
-- Create date: <08/14/2011>
-- Description:	<Process 3 ZIP Data From Staging To Production>
-- =============================================
CREATE PROCEDURE [dbo].[usp_Process5ZIP]
@Version SMALLINT
AS
BEGIN
	--EXEC dbo.usp_Process5ZIP 1107;
	SET NOCOUNT ON;

	DECLARE @v_ID INT;
	DECLARE @v_geom GEOGRAPHY;
	DECLARE @v_count INT;
	DECLARE @v_count_ring INT;
	DECLARE @v_count_total INT;
	DECLARE @v_g GEOMETRY;

	--Get 3 ZIP
	DECLARE c_5ZIP CURSOR FAST_FORWARD LOCAL FOR
		SELECT ID, geom FROM premium.dbo.ZIP
			ORDER BY ID;
	OPEN c_5ZIP;
	FETCH NEXT FROM c_5ZIP INTO @v_ID, @v_geom;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			--Get Total Count
			SET @v_count = 1;
			SET @v_count_total = 0;
			WHILE @v_count <= @v_geom.STNumGeometries()
			BEGIN
				SET @v_count_ring = 1;
				WHILE @v_count_ring <= @v_geom.STGeometryN(@v_count).NumRings()
				BEGIN
					SET @v_count_total = @v_count_total + 1;
					SET @v_count_ring = @v_count_ring + 1;
				END
				SET @v_count = @v_count + 1;
			END

			--Get Area
			SET @v_g = geometry::STGeomFromText(@v_geom.ToString(), 4326);
			INSERT INTO dbo.PostalArea (
				Version, AreaTypeID, AreaID, AreaCode, AreaCodeGroup,
				AreaFIPSST, AreaFIPSCO, AreaDescription,
				NumOfHome, NumOfBusiness, NumOfAPT, NumOfTotal,
				NumOfShape, NumOfRing, AreaGeography, AreaGeometry
			) SELECT @Version, 2, @v_ID, ZIP, LEFT(ZIP, 3),
				LEFT(FIPSSTCO, 2), RIGHT(FIPSSTCO, 3), '5ZIP',
				0, 0, 0, 0,
				0, @v_count_total, @v_geom, @v_g
				FROM premium.dbo.ZIP
				WHERE ID = @v_ID;
				
			--Get Geometry (Polygon, Ring)
			SET @v_count = 1;
			WHILE @v_count <= @v_geom.STNumGeometries()
			BEGIN
				SET @v_count_ring = 1;
				WHILE @v_count_ring <= @v_geom.STGeometryN(@v_count).NumRings()
				BEGIN
					SET @v_g = geometry::STGeomFromText(
						REPLACE(REPLACE(@v_geom.STGeometryN(@v_count).RingN(@v_count_ring).ToString(),
							'LINESTRING (', 'POLYGON (('), ')', '))'), 4326);
					IF @v_g.STIsValid() <> 1
						SET @v_g = @v_g.MakeValid();
				
					INSERT INTO dbo.PostalShape (
						Version, AreaTypeID, AreaID,
						MinLatitude, MaxLatitude, MinLongitude, MaxLongitude,
						Latitude, Longitude,
						SqMile, IsInnerRing,
						ShapeGeography, ShapeGeometry
					) SELECT @Version, 2, @v_ID,
						0, 0, 0, 0,
						ROUND(@v_g.STCentroid().STY, 5), ROUND(@v_g.STCentroid().STX, 5),
						@v_geom.STGeometryN(@v_count).STArea() * 3.86102159 * POWER(10.00000000, -7), (CASE WHEN @v_count_ring = 1 THEN 0 ELSE 1 END),
						@v_geom.STGeometryN(@v_count).RingN(@v_count_ring), @v_g;

					SET @v_count_ring = @v_count_ring + 1;
				END
				SET @v_count = @v_count + 1;
			END
		END TRY
		BEGIN CATCH
			PRINT 'Error ID: ' + CAST(@v_ID AS VARCHAR(50));
		END CATCH

		FETCH NEXT FROM c_5ZIP INTO @v_ID, @v_geom;
	END
	CLOSE c_5ZIP;
	DEALLOCATE c_5ZIP;

	--Get Coordinates (Point)
	DECLARE c_5ZIP CURSOR FAST_FORWARD LOCAL FOR
		SELECT ShapeID, ShapeGeography FROM dbo.PostalShape
			WHERE Version = @Version
			AND AreaTypeID = 2
			ORDER BY ShapeID;
	OPEN c_5ZIP;
	FETCH NEXT FROM c_5ZIP INTO @v_ID, @v_geom;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @v_count = 1;
		WHILE @v_count <= @v_geom.STNumPoints()
		BEGIN
			SET @v_g = geometry::STGeomFromText(@v_geom.STPointN(@v_geom.STNumPoints() - @v_count + 1).ToString(), 4326);
			INSERT INTO dbo.PostalShapeCoordinate (
				ShapeID, ShapeOrder, Latitude, Longitude
			) SELECT @v_ID, @v_count, ROUND(@v_g.STY, 5), ROUND(@v_g.STX, 5);
			
			SET @v_count = @v_count + 1;
		END
		FETCH NEXT FROM c_5ZIP INTO @v_ID, @v_geom;
	END
	CLOSE c_5ZIP;
	DEALLOCATE c_5ZIP;	

	--Update NumOfShape
	UPDATE PostalArea
		SET NumOfShape = pag.NumOfShape
		FROM PostalArea AS pa INNER JOIN (
			SELECT Version, AreaTypEID, AreaID, COUNT(*) AS NumOfShape
				FROM dbo.PostalShape
				WHERE IsInnerRing = 0
				AND Version = @Version
				AND AreaTypeID = 2
				GROUP BY Version, AreaTypEID, AreaID
		) AS pag
		ON pa.Version = pag.Version AND pa.AreaTypeID = pag.AreaTypeID AND pa.AreaID = pag.AreaID
		WHERE pa.Version = @Version
		AND pa.AreaTypeID = 2;
		
	--Update Bounding Box
	UPDATE dbo.PostalShape
		SET MinLatitude = ROUND(psc.MinLatitude, 5),
			MaxLatitude = ROUND(psc.MaxLatitude, 5),
			MinLongitude = ROUND(psc.MinLongitude, 5),
			MaxLongitude = ROUND(psc.MaxLongitude, 5)
		FROM dbo.PostalShape AS ps
		INNER JOIN (
			SELECT ShapeID,
				MIN(Latitude) AS MinLatitude,
				MAX(Latitude) AS MaxLatitude,
				MIN(Longitude) AS MinLongitude,
				MAX(Longitude) AS MaxLongitude
				FROM dbo.PostalShapeCoordinate
			GROUP BY ShapeID
		) AS psc
		ON ps.ShapeID = psc.ShapeID
		WHERE ps.Version = @Version
		AND ps.AreaTypeID = 2;

	--Get BoxID
	DECLARE @v_AreaID INT;
	DECLARE @v_MinLatitude FLOAT;
	DECLARE @v_MaxLatitude FLOAT;
	DECLARE @v_MinLongitude FLOAT;
	DECLARE @v_MaxLongitude FLOAT;
	DECLARE @v_MountLat INT;
	DECLARE @v_MountLon INT;

	SET @v_MountLat = 10;
	SET @v_MountLon = 15;

	DECLARE c_AREA CURSOR FAST_FORWARD LOCAL FOR
		SELECT ShapeID, MinLatitude, MaxLatitude, MinLongitude, MaxLongitude
			FROM dbo.PostalShape
			WHERE Version = @Version
			AND AreaTypeID = 2
			ORDER BY ShapeID;
	OPEN c_AREA;
	FETCH NEXT FROM c_AREA INTO @v_AreaID, @v_MinLatitude, @v_MaxLatitude, @v_MinLongitude, @v_MaxLongitude;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO dbo.PostalShapeBox (ShapeID, BoxID)
			SELECT @v_AreaID, BoxID
			FROM dbo.fn_GetBoxIds(
				@v_MinLatitude, @v_MaxLatitude,
				@v_MinLongitude, @v_MaxLongitude,
				@v_MountLat, @v_MountLon
			);
		FETCH NEXT FROM c_AREA INTO @v_AreaID, @v_MinLatitude, @v_MaxLatitude, @v_MinLongitude, @v_MaxLongitude;
	END
	CLOSE c_AREA;
	DEALLOCATE c_AREA;
	
	--Update Parent Polygon ID for Inner Ring
	CREATE SPATIAL INDEX IX_PostalShape_ShapeGeometry
	ON dbo.PostalShape (
		ShapeGeometry
	)USING  GEOMETRY_GRID 
	WITH (BOUNDING_BOX =(-180, -90, 180, 90), GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
		CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON
	);
	UPDATE dbo.PostalShape
		SET ParentShapeID = psp.ShapeID
		FROM dbo.PostalShape
		INNER JOIN dbo.PostalShape AS psp
		ON psp.ShapeGeometry.STContains(PostalShape.ShapeGeometry) = 1
		AND PostalShape.Version = psp.Version
		AND PostalShape.AreaTypeID = psp.AreaTypeID
		AND PostalShape.ShapeId <> psp.ShapeID
		WHERE PostalShape.Version = @Version
		AND PostalShape.AreaTypeID = 2;
	DROP INDEX IX_PostalShape_ShapeGeometry ON dbo.PostalShape;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Process3ZIP]    Script Date: 08/29/2011 20:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Kyoung Shin>
-- Create date: <08/14/2011>
-- Description:	<Process 3 ZIP Data From Staging To Production>
-- =============================================
CREATE PROCEDURE [dbo].[usp_Process3ZIP]
@Version SMALLINT
AS
BEGIN
	--EXEC dbo.usp_Process3ZIP 1107;
	SET NOCOUNT ON;

	DECLARE @v_ID INT;
	DECLARE @v_geom GEOGRAPHY;
	DECLARE @v_count INT;
	DECLARE @v_count_ring INT;
	DECLARE @v_count_total INT;
	DECLARE @v_g GEOMETRY;

	--Get 3 ZIP
	DECLARE c_3ZIP CURSOR FAST_FORWARD LOCAL FOR
		SELECT ID, geom FROM premium.dbo.ZIP_3
			ORDER BY ID;
	OPEN c_3ZIP;
	FETCH NEXT FROM c_3ZIP INTO @v_ID, @v_geom;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			--Get Total Count
			SET @v_count = 1;
			SET @v_count_total = 0;
			WHILE @v_count <= @v_geom.STNumGeometries()
			BEGIN
				SET @v_count_ring = 1;
				WHILE @v_count_ring <= @v_geom.STGeometryN(@v_count).NumRings()
				BEGIN
					SET @v_count_total = @v_count_total + 1;
					SET @v_count_ring = @v_count_ring + 1;
				END
				SET @v_count = @v_count + 1;
			END

			--Get Area
			SET @v_g = geometry::STGeomFromText(@v_geom.ToString(), 4326);
			INSERT INTO dbo.PostalArea (
				Version, AreaTypeID, AreaID, AreaCode, AreaCodeGroup,
				AreaFIPSST, AreaFIPSCO, AreaDescription,
				NumOfHome, NumOfBusiness, NumOfAPT, NumOfTotal,
				NumOfShape, NumOfRing, AreaGeography, AreaGeometry
			) SELECT @Version, 1, @v_ID, ZIP3, NULL,
				'', '', '3ZIP',
				0, 0, 0, 0,
				0, @v_count_total, @v_geom, @v_g
				FROM premium.dbo.ZIP_3
				WHERE ID = @v_ID;
				
			--Get Geometry (Polygon, Ring)
			SET @v_count = 1;
			WHILE @v_count <= @v_geom.STNumGeometries()
			BEGIN
				SET @v_count_ring = 1;
				WHILE @v_count_ring <= @v_geom.STGeometryN(@v_count).NumRings()
				BEGIN
					SET @v_g = geometry::STGeomFromText(
						REPLACE(REPLACE(@v_geom.STGeometryN(@v_count).RingN(@v_count_ring).ToString(),
							'LINESTRING (', 'POLYGON (('), ')', '))'), 4326);
					IF @v_g.STIsValid() <> 1
						SET @v_g = @v_g.MakeValid();
				
					INSERT INTO dbo.PostalShape (
						Version, AreaTypeID, AreaID,
						MinLatitude, MaxLatitude, MinLongitude, MaxLongitude,
						Latitude, Longitude,
						SqMile, IsInnerRing,
						ShapeGeography, ShapeGeometry
					) SELECT @Version, 1, @v_ID,
						0, 0, 0, 0,
						ROUND(@v_g.STCentroid().STY, 5), ROUND(@v_g.STCentroid().STX, 5),
						@v_geom.STGeometryN(@v_count).STArea() * 3.86102159 * POWER(10.00000000, -7), (CASE WHEN @v_count_ring = 1 THEN 0 ELSE 1 END),
						@v_geom.STGeometryN(@v_count).RingN(@v_count_ring), @v_g;

					SET @v_count_ring = @v_count_ring + 1;
				END
				SET @v_count = @v_count + 1;
			END
		END TRY
		BEGIN CATCH
			PRINT 'Error ID: ' + CAST(@v_ID AS VARCHAR(50));
		END CATCH

		FETCH NEXT FROM c_3ZIP INTO @v_ID, @v_geom;
	END
	CLOSE c_3ZIP;
	DEALLOCATE c_3ZIP;
	
	--Get Coordinates (Point)
	DECLARE c_3ZIP CURSOR FAST_FORWARD LOCAL FOR
		SELECT ShapeID, ShapeGeography FROM dbo.PostalShape
			WHERE Version = @Version
			AND AreaTypeID = 1
			ORDER BY ShapeID;
	OPEN c_3ZIP;
	FETCH NEXT FROM c_3ZIP INTO @v_ID, @v_geom;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @v_count = 1;
		WHILE @v_count <= @v_geom.STNumPoints()
		BEGIN
			SET @v_g = geometry::STGeomFromText(@v_geom.STPointN(@v_geom.STNumPoints() - @v_count + 1).ToString(), 4326);
			INSERT INTO dbo.PostalShapeCoordinate (
				ShapeID, ShapeOrder, Latitude, Longitude
			) SELECT @v_ID, @v_count, ROUND(@v_g.STY, 5), ROUND(@v_g.STX, 5);
			
			SET @v_count = @v_count + 1;
		END
		FETCH NEXT FROM c_3ZIP INTO @v_ID, @v_geom;
	END
	CLOSE c_3ZIP;
	DEALLOCATE c_3ZIP;	

	--Update NumOfShape
	UPDATE PostalArea
		SET NumOfShape = pag.NumOfShape
		FROM PostalArea AS pa INNER JOIN (
			SELECT Version, AreaTypEID, AreaID, COUNT(*) AS NumOfShape
				FROM dbo.PostalShape
				WHERE IsInnerRing = 0
				AND Version = @Version
				AND AreaTypeID = 1
				GROUP BY Version, AreaTypEID, AreaID
		) AS pag
		ON pa.Version = pag.Version AND pa.AreaTypeID = pag.AreaTypeID AND pa.AreaID = pag.AreaID
		WHERE pa.Version = @Version
		AND pa.AreaTypeID = 1;

	--Update Bounding Box
	UPDATE dbo.PostalShape
		SET MinLatitude = ROUND(psc.MinLatitude, 5),
			MaxLatitude = ROUND(psc.MaxLatitude, 5),
			MinLongitude = ROUND(psc.MinLongitude, 5),
			MaxLongitude = ROUND(psc.MaxLongitude, 5)
		FROM dbo.PostalShape AS ps
		INNER JOIN (
			SELECT ShapeID,
				MIN(Latitude) AS MinLatitude,
				MAX(Latitude) AS MaxLatitude,
				MIN(Longitude) AS MinLongitude,
				MAX(Longitude) AS MaxLongitude
				FROM dbo.PostalShapeCoordinate
			GROUP BY ShapeID
		) AS psc
		ON ps.ShapeID = psc.ShapeID
		WHERE ps.Version = @Version
		AND ps.AreaTypeID = 1;

	--Get BoxID
	DECLARE @v_AreaID INT;
	DECLARE @v_MinLatitude FLOAT;
	DECLARE @v_MaxLatitude FLOAT;
	DECLARE @v_MinLongitude FLOAT;
	DECLARE @v_MaxLongitude FLOAT;
	DECLARE @v_MountLat INT;
	DECLARE @v_MountLon INT;

	SET @v_MountLat = 25;
	SET @v_MountLon = 40;

	DECLARE c_AREA CURSOR FAST_FORWARD LOCAL FOR
		SELECT ShapeID, MinLatitude, MaxLatitude, MinLongitude, MaxLongitude
			FROM dbo.PostalShape
			WHERE Version = @Version
			AND AreaTypeID = 1
			ORDER BY ShapeID;
	OPEN c_AREA;
	FETCH NEXT FROM c_AREA INTO @v_AreaID, @v_MinLatitude, @v_MaxLatitude, @v_MinLongitude, @v_MaxLongitude;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO dbo.PostalShapeBox (ShapeID, BoxID)
			SELECT @v_AreaID, BoxID
			FROM dbo.fn_GetBoxIds(
				@v_MinLatitude, @v_MaxLatitude,
				@v_MinLongitude, @v_MaxLongitude,
				@v_MountLat, @v_MountLon
			);
		FETCH NEXT FROM c_AREA INTO @v_AreaID, @v_MinLatitude, @v_MaxLatitude, @v_MinLongitude, @v_MaxLongitude;
	END
	CLOSE c_AREA;
	DEALLOCATE c_AREA;

	--Update Parent Polygon ID for Inner Ring
	CREATE SPATIAL INDEX IX_PostalShape_ShapeGeometry
	ON dbo.PostalShape (
		ShapeGeometry
	)USING  GEOMETRY_GRID 
	WITH (BOUNDING_BOX =(-180, -90, 180, 90), GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
		CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON
	);
	UPDATE dbo.PostalShape
		SET ParentShapeID = psp.ShapeID
		FROM dbo.PostalShape
		INNER JOIN dbo.PostalShape AS psp
		ON psp.ShapeGeometry.STContains(PostalShape.ShapeGeometry) = 1
		AND PostalShape.Version = psp.Version
		AND PostalShape.AreaTypeID = psp.AreaTypeID
		AND PostalShape.ShapeId <> psp.ShapeID
		WHERE PostalShape.Version = @Version
		AND PostalShape.AreaTypeID = 1;
	DROP INDEX IX_PostalShape_ShapeGeometry ON dbo.PostalShape;
END
GO
/****** Object:  ForeignKey [FK_PostalArea_AreaType]    Script Date: 08/29/2011 20:19:49 ******/
ALTER TABLE [dbo].[PostalArea]  WITH CHECK ADD  CONSTRAINT [FK_PostalArea_AreaType] FOREIGN KEY([AreaTypeID])
REFERENCES [dbo].[AreaType] ([AreaTypeID])
GO
ALTER TABLE [dbo].[PostalArea] CHECK CONSTRAINT [FK_PostalArea_AreaType]
GO
/****** Object:  ForeignKey [FK_PostalShape_PostalArea]    Script Date: 08/29/2011 20:19:49 ******/
ALTER TABLE [dbo].[PostalShape]  WITH CHECK ADD  CONSTRAINT [FK_PostalShape_PostalArea] FOREIGN KEY([Version], [AreaTypeID], [AreaID])
REFERENCES [dbo].[PostalArea] ([Version], [AreaTypeID], [AreaID])
GO
ALTER TABLE [dbo].[PostalShape] CHECK CONSTRAINT [FK_PostalShape_PostalArea]
GO
/****** Object:  ForeignKey [FK_PostalShape_PostalShape]    Script Date: 08/29/2011 20:19:49 ******/
ALTER TABLE [dbo].[PostalShape]  WITH CHECK ADD  CONSTRAINT [FK_PostalShape_PostalShape] FOREIGN KEY([ParentShapeID])
REFERENCES [dbo].[PostalShape] ([ShapeID])
GO
ALTER TABLE [dbo].[PostalShape] CHECK CONSTRAINT [FK_PostalShape_PostalShape]
GO
/****** Object:  ForeignKey [FK_PostalShapeBox_PostalShape]    Script Date: 08/29/2011 20:19:49 ******/
ALTER TABLE [dbo].[PostalShapeBox]  WITH CHECK ADD  CONSTRAINT [FK_PostalShapeBox_PostalShape] FOREIGN KEY([ShapeID])
REFERENCES [dbo].[PostalShape] ([ShapeID])
GO
ALTER TABLE [dbo].[PostalShapeBox] CHECK CONSTRAINT [FK_PostalShapeBox_PostalShape]
GO
/****** Object:  ForeignKey [FK_PostalShapeCoordinate_PostalShape]    Script Date: 08/29/2011 20:19:49 ******/
ALTER TABLE [dbo].[PostalShapeCoordinate]  WITH CHECK ADD  CONSTRAINT [FK_PostalShapeCoordinate_PostalShape] FOREIGN KEY([ShapeID])
REFERENCES [dbo].[PostalShape] ([ShapeID])
GO
ALTER TABLE [dbo].[PostalShapeCoordinate] CHECK CONSTRAINT [FK_PostalShapeCoordinate_PostalShape]
GO
