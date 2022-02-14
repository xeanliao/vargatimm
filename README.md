# Database restore
1. scp database back file to mssql server
scp -i ~/.ssh/varga.pem <replace to local bak file> root@13.57.187.203:/var/opt/mssql/data/

2. the mssql IP you can get from EC


3. ssh remote to MSSQL server
ssh root@13.57.187.203 -i ~/.ssh/varga.pem

4. run MSSQL restore command




# Database Update

1. 2021-07-24

```sql
-- 3zip

DROP INDEX IF EXISTS [threezipareas_spatial_index] ON [dbo].[threezipareas]

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

CREATE INDEX [threezipareas_IsInnerRing_index] ON [dbo].[threezipareas]
(
	[IsInnerRing] ASC
)

CREATE SPATIAL INDEX [threezipareas_spatial_index]
  ON [dbo].[threezipareas](Geom)
  USING GEOMETRY_AUTO_GRID
  WITH ( 
    BOUNDING_BOX= (xmin=-180, ymin=-90, xmax=180, ymax=90) 
  );

-- 5zip

DROP INDEX IF EXISTS [fivezipareas_spatial_index] ON [dbo].[fivezipareas]

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

CREATE INDEX [fivezipareas_IsInnerRing_index] ON [dbo].[fivezipareas]
(
	[IsInnerRing] ASC
)

CREATE SPATIAL INDEX [fivezipareas_spatial_index]
  ON [dbo].[fivezipareas](Geom)
  USING GEOMETRY_AUTO_GRID
  WITH ( 
    BOUNDING_BOX= (xmin=-180, ymin=-90, xmax=180, ymax=90) 
  );


-- croute

DROP INDEX IF EXISTS [premiumcroutes_spatial_index] ON [dbo].[premiumcroutes]

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

CREATE INDEX [premiumcroutes_IsInnerRing_index] ON [dbo].[premiumcroutes]
(
	[IsInnerRing] ASC
)

CREATE SPATIAL INDEX [premiumcroutes_spatial_index]
  ON [dbo].[premiumcroutes](Geom)
  USING GEOMETRY_AUTO_GRID
  WITH ( 
    BOUNDING_BOX= (xmin=-180, ymin=-90, xmax=180, ymax=90) 
  );
```
