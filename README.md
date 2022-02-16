# Get MSSQL public IP
1. [open `Amazon Elastic Container Service` page](https://us-west-1.console.aws.amazon.com/ecs/v2/clusters/TIMM/services?region=us-west-1)
   ![service task](../TIMM/document_img/ecs_task.jpg)
2. find the running task for the service e.g. timm202202 then you can get the current instance public IP
  ![service task](../TIMM/document_img/ecs_task_public_ip.jpg)

# Get MSSQL private dns address
1. [open the AWS Cloud Map](https://us-west-1.console.aws.amazon.com/cloudmap/home/namespaces?region=us-west-1)
2. find the domain name `timm`
   ![cloud map](../TIMM/document_img/cloud_map.jpg)
3. find the register name. normally the register name is same as ECS service name
   ![cloud map name](../TIMM/document_img/cloud_map_name.jpg)
4. you can find this domain name running instance id
   ![running instance id](../TIMM/document_img/cloud_map_name_instance.jpg)
5. go to ECS find the running task
6. make sure your running task id is same as cloud map service instance id
   ![cloud map instance id vs ecs task id](../TIMM/document_img/cloud_map_name_task.jpg)
7. So you can used timm202202.timm in EC2 to access this ECS instance
   
# How to access mssql ECS instance
1. the mssql instance already enabled ssh remote login. the username is `root` and private key is varga.pem.
2. you can used any ssh client to login this server
   e.g. `ssh root@<replace to your mssql server ip> -i ~/.ssh/varga.pem`
   Or you can use ssh client with private domain name in EC2 instance
   e.g. `ssh root@timm2022.timm -i ~/.ssh/varga.pem`

# Database restore
1. scp database back file to mssql server
scp -i <replace to you private key file path> <replace to local bak file> root@<replace to mssql server ip>:/var/opt/mssql/data/ 
Or
run scp in EC2 instance. BTW. windows can install openssh client
e.g. `scp -i .ssh/varga.pem "D:\MSSQL\Backup\timm202202.bak" root@timm202202.timm:/var/opt/mssql/data/`
Or you can use any sftp tools e.g. FileZilla to upload to mssql server
![sftp](../TIMM/document_img/sftp.jpg)

2. ssh remote to MSSQL server
`ssh root@13.57.187.203 -i ~/.ssh/varga.pem`
4. run restore sql command
```sql
/opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P '<repace to password>' \
   -Q 'RESTORE DATABASE Timm202202 FROM DISK = "/var/opt/mssql/data/timm202202.bak" WITH MOVE "timm" TO "/var/opt/mssql/data/timm202202.mdf", MOVE "timm_log" TO "/var/opt/mssql/data/timm202202.ldf"'
```

see [Microsoft document here](https://docs.microsoft.com/en-us/sql/linux/tutorial-restore-backup-in-sql-server-container?view=sql-server-ver15#restore-the-database)

5. Tips. the restore really take very long time. you can run this sql command to check the restore progress
```sql
SELECT session_id as SPID, command, a.text AS Query, start_time, percent_complete, dateadd(second,estimated_completion_time/1000, getdate()) as estimated_completion_time 
FROM sys.dm_exec_requests r CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) a 
WHERE r.command in ('BACKUP DATABASE','RESTORE DATABASE')
```
The full command run in linux is
```bash
/opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P '<repace to password>' \
  -Q '
SELECT session_id as SPID, command, a.text AS Query, start_time, percent_complete, dateadd(second,estimated_completion_time/1000, getdate()) as estimated_completion_time
FROM sys.dm_exec_requests r CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) a
WHERE r.command in ("BACKUP DATABASE","RESTORE DATABASE")
'
```
5. After success run the restore command you can see something like this
```bash
Processed 1386416 pages for database 'Timm202202', file 'timm' on file 1.
Processed 3 pages for database 'Timm202202', file 'timm_log' on file 1.
Converting database 'Timm202202' from version 706 to the current version 904.
Database 'Timm202202' running the upgrade step from version 706 to version 770.
......
Database 'Timm202202' running the upgrade step from version 903 to version 904.
RESTORE DATABASE successfully processed 1386419 pages in 796.762 seconds (13.594 MB/sec)
```
# Run Database 3zip/5zip/croute build sql
Tips: the linux mssql 2019 you can used new `Azure Data Studio` to connect to and run sql command. you can download [here](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver15) BTW. there have mac version
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
