

ALTER TABLE gtuinfo ADD COLUMN `Status` INTEGER DEFAULT 0 

ALTER TABLE task MODIFY COLUMN `AuditorId` INTEGER


INSERT INTO groupprivilegemappings (`Id`,`GroupId`,`PrivilegeId`) VALUES
 (143,53,2),
 (144,53,8),
 (145,53,12),
 (146,53,17),
 (147,53,19),
 (148,53,20),
 (149,53,21)



ALTER TABLE task ADD COLUMN `Email` VARCHAR(45) 
AFTER `Status`,
 ADD COLUMN `Telephone` VARCHAR(45)  AFTER `Email`

ALTER TABLE `gtuinfo` ADD COLUMN `Distance` DOUBLE