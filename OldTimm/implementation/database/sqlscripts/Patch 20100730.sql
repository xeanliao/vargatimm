insert into  campaignusermappings (UserId,CampaignId,Status)
select 1,Id,0 from campaigns


update campaignusermappings set status=1 where campaignid in
(select Id from Campaigns where Id in  (select campaignId from submaps where Id in  (select submapid from distributionmaps)))