
CREATE TABLE `privilege` (
  `Id` int(11) NOT NULL auto_increment,
  `Name` varchar(45) NOT NULL,
  `Value` int(11) default NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


INSERT INTO `privilege` (`Id`,`Name`,`Value`) VALUES 
 (1,'Submap',1),
 (2,'Distributionmap',2),
 (3,'Administration',3),
 (4,'GPSMonitor',4),
 (5,'ProductivityReport',5);



CREATE TABLE `timm`.`groupprivilegemappings` (
  `Id` INTEGER NOT NULL AUTO_INCREMENT,
  `GroupId` INTEGER NOT NULL,
  `PrivilegeId` INTEGER NOT NULL,
  PRIMARY KEY(`Id`),
  CONSTRAINT `FK_groupprivilegemappings_1` FOREIGN KEY `FK_groupprivilegemappings_1` (`GroupId`)
    REFERENCES `groups` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_groupprivilegemappings_2` FOREIGN KEY `FK_groupprivilegemappings_2` (`PrivilegeId`)
    REFERENCES `privilege` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
)
ENGINE = InnoDB;
