

DROP TABLE IF EXISTS `distributionmapcoordinates`;
CREATE TABLE `distributionmapcoordinates` (
  `Id` int(11) NOT NULL auto_increment,
  `DistributionMapId` int(11) NOT NULL,
  `Latitude` double NOT NULL,
  `Longitude` double NOT NULL,
  PRIMARY KEY  (`Id`),
  KEY `FK_distributionmapcoordinates_1` (`DistributionMapId`),
  CONSTRAINT `FK_distributionmapcoordinates_1` FOREIGN KEY (`DistributionMapId`) REFERENCES `distributionmaps` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

