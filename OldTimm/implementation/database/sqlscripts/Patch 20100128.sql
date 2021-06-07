ALTER TABLE [UpperSenateAreas]
ALTER COLUMN Name nvarchar(64)
go
alter table Radiuses 
add IsDisplay bit
go
update Radiuses set IsDisplay = 1 
go

alter table Radiuses 
ALTER COLUMN IsDisplay bit not null
go

alter table Campaigns
ALTER COLUMN Logo nvarchar(64) null