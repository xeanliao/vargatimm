ALTER TABLE `timm`.`task` ADD COLUMN `Status` INTEGER AFTER `AuditorId`;
update `timm`.`task` set status=0
INSERT INTO `groupprivilegemappings` (`Id`,`GroupId`,`PrivilegeId`) VALUES 
 (141,46,11),
 (142,47,11);