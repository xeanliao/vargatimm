# Database Update

1. 2021-07-24

```sql
-- 3zip
IF EXISTS (SELECT 1
               FROM   INFORMATION_SCHEMA.COLUMNS
               WHERE  TABLE_NAME = 'threezipareas'
                      AND COLUMN_NAME = 'Geom'
                      AND TABLE_SCHEMA='DBO')
  BEGIN
      ALTER TABLE [dbo].[threezipareas]
        DROP COLUMN Geom
  END
GO

ALTER TABLE [dbo].[threezipareas]
ADD Geom geometry, IsMaster bit;

UPDATE [dbo].[threezipareas]
SET [IsMaster] = 1
WHERE Id IN (
  SELECT B.Id
  FROM
    (
      SELECT Code, Max(SqMile) AS SqMile
      FROM [dbo].[threezipareas]
      GROUP by Code
    ) AS A INNER JOIN [dbo].[threezipareas] B ON A.Code = B.Code AND A.SqMile = B.SqMile
)

CREATE SPATIAL INDEX threezipareas_spatial_index
   ON [dbo].[threezipareas](Geom)
   USING GEOMETRY_GRID
   WITH (
    BOUNDING_BOX = ( xmin=0, ymin=0, xmax=500, ymax=200 ),
    GRIDS = (LOW, LOW, MEDIUM, HIGH),
    CELLS_PER_OBJECT = 64,
    PAD_INDEX  = ON );

-- 5zip
IF EXISTS (SELECT 1
               FROM   INFORMATION_SCHEMA.COLUMNS
               WHERE  TABLE_NAME = 'fivezipareas'
                      AND COLUMN_NAME = 'Geom'
                      AND TABLE_SCHEMA='DBO')
  BEGIN
      ALTER TABLE [dbo].[fivezipareas]
        DROP COLUMN Geom
  END
GO


ALTER TABLE [dbo].[fivezipareas]
ADD Geom geometry, IsMaster bit;

UPDATE [dbo].[fivezipareas]
SET [IsMaster] = 1
WHERE Id IN (
  SELECT B.Id
  FROM
    (
      SELECT Code, Max(SqMile) AS SqMile
      FROM [dbo].[fivezipareas]
      GROUP by Code
    ) AS A INNER JOIN [dbo].[fivezipareas] B ON A.Code = B.Code AND A.SqMile = B.SqMile
)

CREATE SPATIAL INDEX fivezipareas_spatial_index
   ON [dbo].[fivezipareas](Geom)
   USING GEOMETRY_GRID
   WITH (
    BOUNDING_BOX = ( xmin=0, ymin=0, xmax=500, ymax=200 ),
    GRIDS = (LOW, LOW, MEDIUM, HIGH),
    CELLS_PER_OBJECT = 64,
    PAD_INDEX  = ON );


-- croute
IF EXISTS (SELECT 1
               FROM   INFORMATION_SCHEMA.COLUMNS
               WHERE  TABLE_NAME = 'premiumcroutes'
                      AND COLUMN_NAME = 'Geom'
                      AND TABLE_SCHEMA='DBO')
  BEGIN
      ALTER TABLE [dbo].[premiumcroutes]
        DROP COLUMN Geom
  END
GO

ALTER TABLE [dbo].[premiumcroutes]
ADD Geom geometry, IsMaster bit;

UPDATE [dbo].[premiumcroutes]
SET [IsMaster] = 1
WHERE Id IN (
  SELECT B.Id
  FROM
    (
      SELECT GEOCODE, Max(SqMile) AS SqMile
      FROM [dbo].[premiumcroutes]
      GROUP by GEOCODE
    ) AS A INNER JOIN [dbo].[premiumcroutes] B ON A.GEOCODE = B.GEOCODE AND A.SqMile = B.SqMile
)

CREATE SPATIAL INDEX premiumcroutes_spatial_index
   ON [dbo].[premiumcroutes](Geom)
   USING GEOMETRY_GRID
   WITH (
    BOUNDING_BOX = ( xmin=0, ymin=0, xmax=500, ymax=200 ),
    GRIDS = (LOW, LOW, MEDIUM, HIGH),
    CELLS_PER_OBJECT = 64,
    PAD_INDEX  = ON );
```
