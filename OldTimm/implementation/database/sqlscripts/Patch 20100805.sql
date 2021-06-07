
CREATE TABLE `timm`.`groups` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(45) NOT NULL,
  PRIMARY KEY(`Id`)
)
ENGINE=InnoDB DEFAULT CHARSET=latin1
AUTO_INCREMENT = 1;


CREATE TABLE `timm`.`usergroupmappings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `GroupId` int(11) NOT NULL,  
  PRIMARY KEY  (`Id`),
  KEY `IDX_USERGROUPMAPPINGS_USERID` (`UserId`),
  KEY `IDX_USERGROUPMAPPINGS_GROUPID` (`GroupId`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1
AUTO_INCREMENT = 1;

insert into `timm`.`groups` (Name) values ('SubMapGroup')
insert into `timm`.`groups` (Name) values ('DistributionMapGroup')
insert into `timm`.`groups` (Name) values ('UserManagerGroup')
insert into `timm`.`usergroupmappings`  (userid,groupid) values (1,3)