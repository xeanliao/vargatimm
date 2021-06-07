

DROP TABLE IF EXISTS `distributionmaprecords`;
CREATE TABLE `distributionmaprecords` (
  `Id` int(11) NOT NULL,
  `AreaId` int(11) NOT NULL,
  `DistributionMapId` int(11) NOT NULL,
  `Classification` int(11) NOT NULL,
  `Value` bit(1) NOT NULL,
  PRIMARY KEY  (`Id`),
  KEY `FK_DistributionMapRecords_DistributionMap` (`DistributionMapId`),
  CONSTRAINT `FK_DistributionMapRecords_DistributionMap` FOREIGN KEY (`DistributionMapId`) REFERENCES `distributionmaps` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

