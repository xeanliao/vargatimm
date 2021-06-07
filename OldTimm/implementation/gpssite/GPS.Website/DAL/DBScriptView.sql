
Create View [dbo].[ViewCustomAreaRange]
as
select CustomAreaId, 
	max(latitude) as MaxLatitude, 
	min(latitude) as MinLatitude, 
	max(longitude) as MaxLongitude,
	min(longitude) as MinLongitude
	from customareacoordinates
group by CustomAreaId

GO


create View [dbo].[ViewDistributionRange]
as
select DistributionMapId,
	MAX(latitude) as MaxLatitude, 
	MIN(latitude) as MinLatitude,
	MAX(longitude) as MaxLongitude, 
	MIN(longitude) as MinLongitude
from distributionmapcoordinates
group by DistributionMapId

GO


CREATE view [dbo].[ViewGtuInTask]
as
-- Jimmy on 11/25/2011
select gtus.Id as GtuID, 
	right(gtus.UniqueID, 6) as UniqueID,
	users.Id as UserID,
	users.FullName,
	users.CompanyId, 
	users.[Role] as UserRoleID,
	m.TaskId,
	m.Id as TaskGtuID,
	m.UserColor
from taskgtuinfomapping (nolock) m
	join gtus (nolock) on gtus.Id = m.GTUId
	join users (nolock) on users.Id = m.UserId
where m.Id in
	(
		select Max(Id) from taskgtuinfomapping
		group by GTUId
	)

GO


create view [dbo].[ViewGtuLocation]
as
select Id, dwLatitude, dwLongitude from gtuinfo

GO


create View [dbo].[ViewNdAddressRange]
as
select NdAddressId, 
	max(latitude) as MaxLatitude, 
	min(latitude) as MinLatitude, 
	max(longitude) as MaxLongitude,
	min(longitude) as MinLongitude
	from ndaddresscoordinates
group by NdAddressId

GO
