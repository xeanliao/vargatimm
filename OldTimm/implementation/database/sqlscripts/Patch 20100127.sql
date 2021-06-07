alter table Campaigns
add ClientName nvarchar(64)

alter table Campaigns
add	ContactName nvarchar(64)

alter table Campaigns
add	ClientCode nvarchar(64)

alter table Campaigns
add	Logo nvarchar(64)

alter table Campaigns
add	AreaDescription nvarchar(128)
go
update Campaigns
set ClientName=CustemerName,ContactName=CustemerName,ClientCode=CustemerName,Logo='',AreaDescription=[Description]
go
--select * from Campaigns
alter table Campaigns
ALTER COLUMN ClientName nvarchar(64) not null

alter table Campaigns
ALTER COLUMN ContactName nvarchar(64) not null

alter table Campaigns
ALTER COLUMN ClientCode nvarchar(64) not null

alter table Campaigns
ALTER COLUMN Logo nvarchar(64) not null

alter table Campaigns
ALTER COLUMN AreaDescription nvarchar(128) not null
go
alter table Users
add FullName nvarchar(128) 

alter table Users
add UserCode nvarchar(64)
go

update Users set FullName = UserName, UserCode = UserName
--select * from Users 
go

ALTER TABLE Users
ALTER COLUMN FullName nvarchar(128) not null

ALTER TABLE Users
ALTER COLUMN UserCode nvarchar(64) not null
