
CREATE TABLE `task` (
  `Id` int(11) NOT NULL auto_increment,
  `Name` varchar(45) NOT NULL,
  `Date` date default '0000-00-00',
  `DmId` int(11) NOT NULL,
  `AuditorId` int(11) NOT NULL,
  PRIMARY KEY  (`Id`),
  KEY `FK_monitors_1` (`DmId`),
  KEY `FK_monitors_2` (`AuditorId`),
  CONSTRAINT `FK_monitors_1` FOREIGN KEY (`DmId`) REFERENCES `distributionmaps` (`Id`),
  CONSTRAINT `FK_monitors_2` FOREIGN KEY (`AuditorId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `tasktime` (
  `Id` int(11) NOT NULL auto_increment,
  `TaskId` int(11) default NULL,
  `Time` datetime default NULL,
  `TimeType` int(11) default NULL,
  PRIMARY KEY  (`Id`),
  KEY `FK_tasktime_1` (`TaskId`),
  CONSTRAINT `FK_tasktime_1` FOREIGN KEY (`TaskId`) REFERENCES `task` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `taskgtuinfomapping` (
  `Id` int(11) NOT NULL auto_increment,
  `TaskId` int(11) NOT NULL,
  `GTUId` int(11) default NULL,
  `UserId` int(11) default NULL,
  `UserColor` varchar(45) default NULL,
  PRIMARY KEY  (`Id`),
  KEY `FK_monitorsettings_1` (`GTUId`),
  KEY `FK_monitorsettings_2` (`UserId`),
  KEY `FK_taskgtuinfomapping_3` (`TaskId`),
  CONSTRAINT `FK_monitorsettings_1` FOREIGN KEY (`GTUId`) REFERENCES `gtus` (`Id`),
  CONSTRAINT `FK_monitorsettings_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`),
  CONSTRAINT `FK_taskgtuinfomapping_3` FOREIGN KEY (`TaskId`) REFERENCES `task` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE `gtuinfo` (
  `Id` int(11) NOT NULL auto_increment,
  `dwSpeed` double default NULL,
  `nHeading` int(11) default NULL,
  `dtSendTime` datetime default NULL,
  `dtReceivedTime` datetime default NULL,
  `sIPAddress` varchar(45) default NULL,
  `nAreaCode` int(11) default NULL,
  `nNetworkCode` int(11) default NULL,
  `nCellID` int(11) default NULL,
  `nGPSFix` int(11) default NULL,
  `nAccuracy` int(11) default NULL,
  `nCount` int(11) default NULL,
  `nLocationID` int(11) default NULL,
  `sVersion` varchar(45) default NULL,
  `dwAltitude` double default NULL,
  `dwLatitude` double default NULL,
  `dwLongitude` double default NULL,
  `PowerInfo` int(11) default NULL,
  `TaskgtuinfoId` int(11) default NULL,
  `Code` varchar(100) default NULL,
  PRIMARY KEY  (`Id`),
  KEY `FK_gtuinfo_1` (`TaskgtuinfoId`),
  CONSTRAINT `FK_gtuinfo_1` FOREIGN KEY (`TaskgtuinfoId`) REFERENCES `taskgtuinfomapping` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;