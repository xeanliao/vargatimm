
alter table FiveZipAreas
Add IsEnabled bit
alter table FiveZipAreas
Add OTotal int
alter table FiveZipAreas
Add [Description] nvarchar(256)
go
update FiveZipAreas set IsEnabled = 1, OTotal=0
go

alter table FiveZipAreas
alter column IsEnabled bit not null


go
alter table Tracts
Add IsEnabled bit
alter table Tracts
Add OTotal int
alter table Tracts
Add [Description] nvarchar(256)
go
update Tracts set IsEnabled = 1, OTotal=0
go

alter table Tracts
alter column IsEnabled bit not null
go

alter table BlockGroups
Add IsEnabled bit
alter table BlockGroups
Add OTotal int
alter table BlockGroups
Add [Description] nvarchar(256)
go
update BlockGroups set IsEnabled = 1, OTotal=0
go

alter table BlockGroups
alter column IsEnabled bit not null
go

CREATE PROCEDURE [dbo].[SetFiveZipEnabled]
	@Code nvarchar(8),
	@Total int,
	@Description nvarchar(255),
	@IsEnabled bit
AS
BEGIN
	SET NOCOUNT ON
	update FiveZipAreas 
	set OTotal = @Total,
	[Description]=@Description,
	IsEnabled = @IsEnabled 
	where Code = @Code
	
	update Tracts
	set IsEnabled = @IsEnabled 
	where Id in (select distinct TractId from BlockGroupMappings
	where FiveZipAreaCode = @Code)
	
	update BlockGroups
	set IsEnabled = @IsEnabled 
	where Id in (select distinct BlockGroupId from BlockGroupMappings
	where FiveZipAreaCode = @Code)
	
END

GO

CREATE PROCEDURE [dbo].[SetTractEnabled]
	@StateCode nvarchar(8),
	@CountyCode nvarchar(8),
	@Code nvarchar(8),
	@Total int,
	@Description nvarchar(255),
	@IsEnabled bit
AS
BEGIN
	SET NOCOUNT ON
	update Tracts 
	set OTotal = @Total,
	[Description]=@Description,
	IsEnabled = @IsEnabled 
	where Code = @Code
	and StateCode = @StateCode
	and CountyCode = @CountyCode 
	
	update BlockGroups 
	set 
	[Description]=@Description,
	IsEnabled = @IsEnabled 
	where StateCode = @StateCode
	and CountyCode = @CountyCode 
	and TractCode = @Code
END
go

CREATE PROCEDURE [dbo].[SetBlockGroupEnabled]
	@StateCode nvarchar(8),
	@CountyCode nvarchar(8),
	@TractCode nvarchar(8),
	@Code nvarchar(8),
	@Total int,
	@Description nvarchar(255),
	@IsEnabled bit
AS
BEGIN
	SET NOCOUNT ON
	update BlockGroups 
	set OTotal = @Total,
	[Description]=@Description,
	IsEnabled = @IsEnabled 
	where Code = @Code
	and StateCode = @StateCode
	and CountyCode = @CountyCode 
	and TractCode = @TractCode
END
go

