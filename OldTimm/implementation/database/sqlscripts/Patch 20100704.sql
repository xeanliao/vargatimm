
ALTER TABLE `timm`.`distributionmaps`
 ADD COLUMN `ColorR` int(11) NOT NULL default '0',
 ADD COLUMN `ColorG` int(11) NOT NULL default '0',
 ADD COLUMN `ColorB` int(11) NOT NULL default '0',
 ADD COLUMN `ColorString` varchar(16) NOT NULL default '000000',
 ADD COLUMN `Total` INTEGER NOT NULL DEFAULT 0 AFTER `ColorString`,
 ADD COLUMN `Penetration` INTEGER NOT NULL DEFAULT 0 AFTER `Total`,
 ADD COLUMN `Percentage` DOUBLE NOT NULL DEFAULT 0 AFTER `Penetration`,
 ADD COLUMN `TotalAdjustment` INTEGER NOT NULL DEFAULT 0 AFTER `Percentage`,
 ADD COLUMN `CountAdjustment` INTEGER NOT NULL DEFAULT 0 AFTER `TotalAdjustment`;

