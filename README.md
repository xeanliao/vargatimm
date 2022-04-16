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

3. run sql command to check the backup files
```sql
/opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P '<repace to password>' \
   -Q 'RESTORE FILELISTONLY FROM DISK = "/var/opt/mssql/backup/timm202202.bak"' | tr -s ' ' | cut -d ' ' -f 1-2
```
You can get some mssql data filename and log filename
```bash
timm D:\MSSQL\Data\timm202202.mdf
timm_log D:\MSSQL\Data\timm202202.ldf
```
You will use these file name in the next sql command `MOVE` part
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

6. Import shape file into database. you need upload all shape files into MSSQl server. use scp or sftp.
```bash
scp Varga_Final.dbf Varga_Final.prj  Varga_Final.shp Varga_Final.shx root@<Replace to MSSQL server ip>:/root/ -i <Replace to Varga SSH private pem file>
```
Then run these command on the remote mssql server
```bash
ogr2ogr -overwrite -lco "GEOM_TYPE=geometry" -a_srs "EPSG:4326" -f MSSQLSpatial "MSSQL:server=127.0.0.1;database=Timm202202;driver=ODBC Driver 17 for SQL Server;UID=SA;PWD=<Replace to MSSQL Password>" /root/Varga_Final.shx
```

Then fix the geom data
```sql
ALTER TABLE [dbo].[varga_final]
ADD [geom] GEOMETRY

GO

UPDATE [dbo].[varga_final]
SET [geom] = [ogr_geometry].STBuffer(0).MakeValid()
```

7. build croute
```sql
CREATE TABLE [dbo].[premiumcroutes_all](
	[Id] INT PRIMARY KEY,
	[Name] NVARCHAR(64) NOT NULL,
	[Code] NVARCHAR(64) NOT NULL,
  [Zip] NVARCHAR(64) NOT NULL,
	[HOME_COUNT] INT NOT NULL,
	[BUSINESS_COUNT] INT NOT NULL,
	[APT_COUNT] INT NOT NULL,
	[TOTAL_COUNT] INT NOT NULL,
	[Geom] GEOMETRY NULL
)
GO

CREATE INDEX [IX_premiumcroutes_all_Code] ON [dbo].[premiumcroutes_all]
(
	[Code] ASC
)
GO

CREATE SPATIAL INDEX [IX_premiumcroutes_all_Geom]
  ON [dbo].[premiumcroutes_all](Geom)
  USING GEOMETRY_AUTO_GRID
  WITH ( 
    BOUNDING_BOX= (xmin=-180, ymin=-90, xmax=180, ymax=90) 
  );
GO
```

From import shape table to croute table
```sql
INSERT INTO [dbo].[premiumcroutes_all]
    ([id],
    [name],
    [code],
    [zip],
    [home_count],
    [business_count],
    [apt_count],
    [total_count],
    [geom])
SELECT Row_number() OVER(ORDER BY geocode ASC) Id,
    geocode                   NAME,
    geocode                   Code,
    zip,
    home_count                HOME_COUNT,
    business_c                BUSINESS_COUNT,
    apt_count                 APT_COUNT,
    total_coun                TOTAL_COUNT,
    geom
FROM (SELECT geocode,
        Max(zip)                                 zip,
        Max(home_count)                          home_count,
        Max(business_c)                          business_c,
        Max(apt_count)                           apt_count,
        Max(total_coun)                          total_coun,
        geometry::UnionAggregate([geom]).STBuffer(0).MakeValid() Geom
    FROM varga_final
    GROUP  BY geocode) AS T
ORDER  BY T.geocode 
```
8. build 5zip
```sql
CREATE TABLE [dbo].[fivezipareas_all](
	[Id] INT PRIMARY KEY,
	[Name] NVARCHAR(64) NOT NULL,
	[Code] NVARCHAR(64) NOT NULL,
  [Zip] NVARCHAR(64) NOT NULL,
	[HOME_COUNT] INT NOT NULL,
	[BUSINESS_COUNT] INT NOT NULL,
	[APT_COUNT] INT NOT NULL,
	[TOTAL_COUNT] INT NOT NULL,
	[Geom] GEOMETRY NULL
)
GO

CREATE INDEX [IX_fivezipareas_all_Code] ON [dbo].[fivezipareas_all]
(
	[Code] ASC
)
GO

CREATE SPATIAL INDEX [IX_fivezipareas_all_Geom]
  ON [dbo].[fivezipareas_all](Geom)
  USING GEOMETRY_AUTO_GRID
  WITH ( 
    BOUNDING_BOX= (xmin=-180, ymin=-90, xmax=180, ymax=90) 
  );
GO
```

From import shape table to 5zip table
```sql
INSERT INTO [dbo].[fivezipareas_all]
    ([id],
    [name],
    [code],
    [zip],
    [home_count],
    [business_count],
    [apt_count],
    [total_count],
    [geom])
SELECT Row_number() OVER( ORDER BY ZIP ASC) Id,
    ZIP                       NAME,
    ZIP                       Code,
    ZIP,
    HOME_COUNT,
    BUSINESS_COUNT,
    APT_COUNT,
    TOTAL_COUNT,
    geom
FROM (SELECT ZIP,
        SUM(HOME_COUNT)                          HOME_COUNT,
        SUM(BUSINESS_COUNT)                      BUSINESS_COUNT,
        SUM(APT_COUNT)                           APT_COUNT,
        SUM(TOTAL_COUNT)                         TOTAL_COUNT,
        geometry::UnionAggregate([geom]).MakeValid() Geom
    FROM premiumcroutes_all
    GROUP  BY ZIP) AS T
ORDER  BY T.ZIP 
```

9. build 3zip
```sql
CREATE TABLE [dbo].[threezipareas_all](
	[Id] INT PRIMARY KEY,
	[Name] NVARCHAR(64) NOT NULL,
	[Code] NVARCHAR(64) NOT NULL,
  [Zip] NVARCHAR(64) NOT NULL,
	[HOME_COUNT] INT NOT NULL,
	[BUSINESS_COUNT] INT NOT NULL,
	[APT_COUNT] INT NOT NULL,
	[TOTAL_COUNT] INT NOT NULL,
	[Geom] GEOMETRY NULL
)
GO

CREATE INDEX [IX_threezipareas_all_Code] ON [dbo].[threezipareas_all]
(
	[Code] ASC
)
GO

CREATE SPATIAL INDEX [IX_threezipareas_all_Geom]
  ON [dbo].[threezipareas_all](Geom)
  USING GEOMETRY_AUTO_GRID
  WITH ( 
    BOUNDING_BOX= (xmin=-180, ymin=-90, xmax=180, ymax=90) 
  );
GO
```

From import shape table to 3zip table
```sql
INSERT INTO [dbo].[threezipareas_all]
    ([id],
    [name],
    [code],
    [zip],
    [home_count],
    [business_count],
    [apt_count],
    [total_count],
    [geom])
SELECT Row_number() OVER( ORDER BY ZIP ASC) Id,
    ZIP                       NAME,
    ZIP                       Code,
    ZIP,
    HOME_COUNT,
    BUSINESS_COUNT,
    APT_COUNT,
    TOTAL_COUNT,
    geom
FROM (SELECT SUBSTRING(ZIP, 3,3)                      Zip,
        SUM(HOME_COUNT)                          HOME_COUNT,
        SUM(BUSINESS_COUNT)                      BUSINESS_COUNT,
        SUM(APT_COUNT)                           APT_COUNT,
        SUM(TOTAL_COUNT)                         TOTAL_COUNT,
        geometry::UnionAggregate([geom]).MakeValid() Geom
    FROM premiumcroutes_all
    GROUP  BY SUBSTRING(ZIP, 3,3)) AS T
ORDER  BY T.Zip 
```

# Update SQL Only need run once!!!

1. 2022-03-01 already apply to timm202202
```sql
ALTER TABLE [dbo].[submaprecords]
ADD [Code] NVARCHAR(10);

ALTER TABLE [dbo].[distributionmaprecords]
ADD [Code] NVARCHAR(10);

ALTER TABLE [dbo].[campaignfivezipimported]
ADD [Code] NVARCHAR(10);

ALTER TABLE [dbo].[campaigncrouteimported]
ADD [Code] NVARCHAR(10);

ALTER TABLE [dbo].[campaignblockgroupimported]
ADD [Code] NVARCHAR(10);

ALTER TABLE [dbo].[campaigntractimported]
ADD [Code] NVARCHAR(10);
```