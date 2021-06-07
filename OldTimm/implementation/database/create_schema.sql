SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

CREATE SCHEMA IF NOT EXISTS `Timm` ;
USE `Timm` ;

-- -----------------------------------------------------
-- Table `Timm`.`CampaignBlockGroupImported`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignBlockGroupImported` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignBlockGroupImported` (
  `Id` INT NOT NULL ,
  `BlockGroupId` INT NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `Total` INT NOT NULL ,
  `Penetration` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_CampaignBlockGroupImported_BlockGroupId` (`BlockGroupId` ASC) ,
  INDEX `IDX_CampaignBlockGroupImported_CampaignId` (`CampaignId` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`Campaigns`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Campaigns` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Campaigns` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Description` VARCHAR(1024) NOT NULL ,
  `UserName` VARCHAR(64) NOT NULL ,
  `Date` DATETIME NOT NULL ,
  `CustemerName` VARCHAR(64) NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `ZoomLevel` INT NOT NULL ,
  `Sequence` INT NULL DEFAULT NULL ,
  `ClientName` VARCHAR(64) NOT NULL ,
  `ContactName` VARCHAR(64) NOT NULL ,
  `ClientCode` VARCHAR(64) NOT NULL ,
  `Logo` VARCHAR(64) NULL DEFAULT NULL ,
  `AreaDescription` VARCHAR(128) NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CampaignClassifications`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignClassifications` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignClassifications` (
  `Id` INT NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `Classification` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_CampaignClassifications_Campaign` (`CampaignId` ASC) ,
  CONSTRAINT `FK_CampaignClassifications_Campaign`
    FOREIGN KEY (`CampaignId` )
    REFERENCES `Timm`.`Campaigns` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CampaignFiveZipImported`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignFiveZipImported` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignFiveZipImported` (
  `Id` INT NOT NULL ,
  `FiveZipAreaId` INT NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `Total` INT NOT NULL ,
  `Penetration` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_CampaignFiveZipImported_FiveZipAreaId` (`FiveZipAreaId` ASC) ,
  INDEX `IDX_CampaignFiveZipImported_CampaignId` (`CampaignId` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CampaignRecords`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignRecords` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignRecords` (
  `Id` INT NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `Classification` INT NOT NULL ,
  `AreaId` INT NOT NULL ,
  `Value` BIT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_CampaignRecords_Campaign` (`CampaignId` ASC) ,
  CONSTRAINT `FK_CampaignRecords_Campaign`
    FOREIGN KEY (`CampaignId` )
    REFERENCES `Timm`.`Campaigns` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CampaignTractImported`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignTractImported` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignTractImported` (
  `Id` INT NOT NULL ,
  `TractId` INT NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `Total` INT NOT NULL ,
  `Penetration` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_CampaignTractImported_TractId` (`TractId` ASC) ,
  INDEX `IDX_CampaignTractImported_CampaignId` (`CampaignId` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CountyAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CountyAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CountyAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`CountyAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CountyAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CountyAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `CountyAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_COUNTYAREABOXMAPPINGS_COUNTYAREAID` (`CountyAreaId` ASC) ,
  INDEX `IDX_COUNTYAREABOXMAPPINGS_BOXID` (`BoxId` ASC) ,
  CONSTRAINT `FK_COUNTYAREABOXMAPPINGS_COUNTYAREAID`
    FOREIGN KEY (`CountyAreaId` )
    REFERENCES `Timm`.`CountyAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`CountyAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CountyAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CountyAreaCoordinates` (
  `Id` INT NOT NULL ,
  `CountyAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_COUNTYAREACOORDINATES_COUNTYAREAID` (`CountyAreaId` ASC) ,
  CONSTRAINT `FK_COUNTYAREACOORDINATES_COUNTYAREAID`
    FOREIGN KEY (`CountyAreaId` )
    REFERENCES `Timm`.`CountyAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`ElementarySchoolAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`ElementarySchoolAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`ElementarySchoolAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`ElementarySchoolAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`ElementarySchoolAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`ElementarySchoolAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `ElementarySchoolAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_ELEMENTARYSCHOOLAREABOXMAPPINGS_ELEMENTARYSCHOOLAREAID` (`ElementarySchoolAreaId` ASC) ,
  INDEX `IDX_ELEMENTARYSCHOOLAREABOXMAPPINGS_BOXID` (`BoxId` ASC) ,
  CONSTRAINT `FK_ELEMENTARYSCHOOLAREABOXMAPPINGS_ELEMENTARYSCHOOLAREAID`
    FOREIGN KEY (`ElementarySchoolAreaId` )
    REFERENCES `Timm`.`ElementarySchoolAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`ElementarySchoolAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`ElementarySchoolAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`ElementarySchoolAreaCoordinates` (
  `Id` INT NOT NULL ,
  `ElementarySchoolAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_ELEMENTARYSCHOOLAREACOORDINATES_ELEMENTARYSCHOOLAREAID` (`ElementarySchoolAreaId` ASC) ,
  CONSTRAINT `FK_ELEMENTARYSCHOOLAREACOORDINATES_ELEMENTARYSCHOOLAREAID`
    FOREIGN KEY (`ElementarySchoolAreaId` )
    REFERENCES `Timm`.`ElementarySchoolAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`FiveZipAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`FiveZipAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`FiveZipAreas` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSADTrans` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `IsEnabled` INT NOT NULL ,
  `OTotal` INT NULL DEFAULT NULL ,
  `Description` VARCHAR(256) NULL DEFAULT NULL ,
  `PartCount` INT NOT NULL DEFAULT 0 ,
  `IsInnerShape` INT NOT NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_FiveZipAreas_Code` (`Code` ASC) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`FiveZipAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`FiveZipAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`FiveZipAreaCoordinates` (
  `Id` BIGINT NOT NULL ,
  `FiveZipAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `ShapeId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_FiveZipAreaCoordinates_FiveZipAreaId` (`FiveZipAreaId` ASC) ,
  CONSTRAINT `FK_FiveZipAreaCoordinates_FiveZipAreaId`
    FOREIGN KEY (`FiveZipAreaId` )
    REFERENCES `Timm`.`FiveZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`FiveZipBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`FiveZipBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`FiveZipBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `FiveZipAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_FiveZipBoxMappings_FiveZipAreaId` (`FiveZipAreaId` ASC) ,
  INDEX `IDX_FiveZipBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_FiveZipBoxMappings_FiveZipAreaId`
    FOREIGN KEY (`FiveZipAreaId` )
    REFERENCES `Timm`.`FiveZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`LowerHouseAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`LowerHouseAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`LowerHouseAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(8) NOT NULL ,
  `GEO_ID` VARCHAR(8) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`LowerHouseAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`LowerHouseAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`LowerHouseAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `LowerHouseAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_LowerHouseAreaBoxMappings_LowerHouseAreaId` (`LowerHouseAreaId` ASC) ,
  INDEX `IDX_LowerHouseAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_LowerHouseAreaBoxMappings_LowerHouseAreaId`
    FOREIGN KEY (`LowerHouseAreaId` )
    REFERENCES `Timm`.`LowerHouseAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`LowerHouseAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`LowerHouseAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`LowerHouseAreaCoordinates` (
  `Id` INT NOT NULL ,
  `LowerHouseAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_LowerHouseAreaCoordinates_LowerHouseAreaId` (`LowerHouseAreaId` ASC) ,
  CONSTRAINT `FK_LowerHouseAreaCoordinates_LowerHouseAreaId`
    FOREIGN KEY (`LowerHouseAreaId` )
    REFERENCES `Timm`.`LowerHouseAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`MetropolitanCoreAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`MetropolitanCoreAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`MetropolitanCoreAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Type` VARCHAR(8) NOT NULL ,
  `Status` VARCHAR(8) NOT NULL ,
  `GEOCODE` VARCHAR(8) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`MetropolitanCoreAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`MetropolitanCoreAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`MetropolitanCoreAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `MetropolitanCoreAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_MetropolitanCoreAreaBoxMappings_MetropolitanCoreAreaId` (`MetropolitanCoreAreaId` ASC) ,
  INDEX `IDX_MetropolitanCoreAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_MetropolitanCoreAreaBoxMappings_MetropolitanCoreAreaId`
    FOREIGN KEY (`MetropolitanCoreAreaId` )
    REFERENCES `Timm`.`MetropolitanCoreAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`MetropolitanCoreAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`MetropolitanCoreAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`MetropolitanCoreAreaCoordinates` (
  `Id` INT NOT NULL ,
  `MetropolitanCoreAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_MetropolitanCoreAreaCoordinates_MetropolitanCoreAreaId` (`MetropolitanCoreAreaId` ASC) ,
  CONSTRAINT `FK_MetropolitanCoreAreaCoordinates_MetropolitanCoreAreaId`
    FOREIGN KEY (`MetropolitanCoreAreaId` )
    REFERENCES `Timm`.`MetropolitanCoreAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`Addresses`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Addresses` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Addresses` (
  `Id` INT NOT NULL ,
  `Address` VARCHAR(256) NOT NULL ,
  `ZipCode` VARCHAR(8) NOT NULL ,
  `OriginalLatitude` FLOAT NOT NULL ,
  `OriginalLongitude` FLOAT NOT NULL ,
  `Longitude` FLOAT NOT NULL ,
  `Latitude` FLOAT NOT NULL ,
  `Color` VARCHAR(8) NOT NULL ,
  `CampaignId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_Addresses_Campaign` (`CampaignId` ASC) ,
  CONSTRAINT `FK_Addresses_Campaign`
    FOREIGN KEY (`CampaignId` )
    REFERENCES `Timm`.`Campaigns` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`Radiuses`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Radiuses` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Radiuses` (
  `Id` INT NOT NULL ,
  `Length` DOUBLE NOT NULL ,
  `LengthMeasuresId` INT NOT NULL ,
  `AddressId` INT NOT NULL ,
  `IsDisplay` BIT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_Radiuses_Address` (`AddressId` ASC) ,
  CONSTRAINT `FK_Radiuses_Address`
    FOREIGN KEY (`AddressId` )
    REFERENCES `Timm`.`Addresses` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`SecondarySchoolAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SecondarySchoolAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SecondarySchoolAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`SecondarySchoolAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SecondarySchoolAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SecondarySchoolAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `SecondarySchoolAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_SecondarySchoolAreaBoxMappings_SecondarySchoolAreaId` (`SecondarySchoolAreaId` ASC) ,
  INDEX `IDX_SecondarySchoolAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_SecondarySchoolAreaBoxMappings_SecondarySchoolAreaId`
    FOREIGN KEY (`SecondarySchoolAreaId` )
    REFERENCES `Timm`.`SecondarySchoolAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`SecondarySchoolAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SecondarySchoolAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SecondarySchoolAreaCoordinates` (
  `Id` INT NOT NULL ,
  `SecondarySchoolAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_SecondarySchoolAreaCoordinates_SecondarySchoolAreaId` (`SecondarySchoolAreaId` ASC) ,
  CONSTRAINT `FK_SecondarySchoolAreaCoordinates_SecondarySchoolAreaId`
    FOREIGN KEY (`SecondarySchoolAreaId` )
    REFERENCES `Timm`.`SecondarySchoolAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`SubMaps`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SubMaps` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SubMaps` (
  `Id` INT NOT NULL ,
  `OrderId` INT NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Total` INT NOT NULL ,
  `Penetration` INT NOT NULL ,
  `Percentage` DOUBLE NOT NULL ,
  `ColorR` INT NOT NULL ,
  `ColorG` INT NOT NULL ,
  `ColorB` INT NOT NULL ,
  `ColorString` VARCHAR(16) NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `TotalAdjustment` INT NOT NULL DEFAULT 0 ,
  `CountAdjustment` INT NOT NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_SubMaps_Campaign` (`CampaignId` ASC) ,
  CONSTRAINT `FK_SubMaps_Campaign`
    FOREIGN KEY (`CampaignId` )
    REFERENCES `Timm`.`Campaigns` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`SubMapRecords`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SubMapRecords` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SubMapRecords` (
  `Id` INT NOT NULL ,
  `SubMapId` INT NOT NULL ,
  `Classification` INT NOT NULL ,
  `AreaId` INT NOT NULL ,
  `Value` BIT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_SubMapRecords_SubMap` (`SubMapId` ASC) ,
  CONSTRAINT `FK_SubMapRecords_SubMap`
    FOREIGN KEY (`SubMapId` )
    REFERENCES `Timm`.`SubMaps` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`ThreeZipAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`ThreeZipAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`ThreeZipAreas` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NULL DEFAULT NULL ,
  `LSADTrans` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`ThreeZipAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`ThreeZipAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`ThreeZipAreaCoordinates` (
  `Id` BIGINT NOT NULL ,
  `ThreeZipAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `ShapeId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_ThreeZipAreaCoordinates_ThreeZipAreaId` (`ThreeZipAreaId` ASC) ,
  CONSTRAINT `FK_ThreeZipAreaCoordinates_ThreeZipAreaId`
    FOREIGN KEY (`ThreeZipAreaId` )
    REFERENCES `Timm`.`ThreeZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`ThreeZipBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`ThreeZipBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`ThreeZipBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `ThreeZipAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_ThreeZipBoxMappings_ThreeZipAreaId` (`ThreeZipAreaId` ASC) ,
  INDEX `IDX_ThreeZipBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_ThreeZipBoxMappings_ThreeZipAreaId`
    FOREIGN KEY (`ThreeZipAreaId` )
    REFERENCES `Timm`.`ThreeZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`Tracts`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Tracts` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Tracts` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `CountyCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NULL DEFAULT NULL ,
  `LSADTrans` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `IsEnabled` INT NOT NULL ,
  `OTotal` INT NULL DEFAULT NULL ,
  `Description` VARCHAR(256) NULL DEFAULT NULL ,
  `ArbitraryUniqueCode` VARCHAR(20) NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_Tracts_ArbitraryUniqueCode` (`ArbitraryUniqueCode` ASC) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`TractBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`TractBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`TractBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `TractId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_TractBoxMappings_BoxId` (`BoxId` ASC) ,
  INDEX `FK_TractBoxMappings_TractId` (`TractId` ASC) ,
  CONSTRAINT `FK_TractBoxMappings_TractId`
    FOREIGN KEY (`TractId` )
    REFERENCES `Timm`.`Tracts` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`TractCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`TractCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`TractCoordinates` (
  `Id` BIGINT NOT NULL ,
  `TractId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_TractCoordinates_TractId` (`TractId` ASC) ,
  CONSTRAINT `FK_TractCoordinates_TractId`
    FOREIGN KEY (`TractId` )
    REFERENCES `Timm`.`Tracts` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UnifiedSchoolAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UnifiedSchoolAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UnifiedSchoolAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UnifiedSchoolAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UnifiedSchoolAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UnifiedSchoolAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `UnifiedSchoolAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_UnifiedSchoolAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  INDEX `FK_UnifiedSchoolAreaBoxMappings_UnifiedSchoolAreaId` (`UnifiedSchoolAreaId` ASC) ,
  CONSTRAINT `FK_UnifiedSchoolAreaBoxMappings_UnifiedSchoolAreaId`
    FOREIGN KEY (`UnifiedSchoolAreaId` )
    REFERENCES `Timm`.`UnifiedSchoolAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UnifiedSchoolAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UnifiedSchoolAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UnifiedSchoolAreaCoordinates` (
  `Id` INT NOT NULL ,
  `UnifiedSchoolAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_UnifiedSchoolAreaCoordinates_UnifiedSchoolAreaId` (`UnifiedSchoolAreaId` ASC) ,
  CONSTRAINT `FK_UnifiedSchoolAreaCoordinates_UnifiedSchoolAreaId`
    FOREIGN KEY (`UnifiedSchoolAreaId` )
    REFERENCES `Timm`.`UnifiedSchoolAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UpperSenateAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UpperSenateAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UpperSenateAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NULL DEFAULT NULL ,
  `GEO_ID` VARCHAR(8) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UpperSenateAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UpperSenateAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UpperSenateAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `UpperSenateAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_UpperSenateAreaBoxMappings_UpperSenateAreaId` (`UpperSenateAreaId` ASC) ,
  INDEX `IDX_UpperSenateAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_UpperSenateAreaBoxMappings_UpperSenateAreaId`
    FOREIGN KEY (`UpperSenateAreaId` )
    REFERENCES `Timm`.`UpperSenateAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UpperSenateAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UpperSenateAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UpperSenateAreaCoordinates` (
  `Id` INT NOT NULL ,
  `UpperSenateAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_UpperSenateAreaCoordinates_UpperSenateAreaId` (`UpperSenateAreaId` ASC) ,
  CONSTRAINT `FK_UpperSenateAreaCoordinates_UpperSenateAreaId`
    FOREIGN KEY (`UpperSenateAreaId` )
    REFERENCES `Timm`.`UpperSenateAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UrbanAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UrbanAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UrbanAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `LSAD` VARCHAR(8) NOT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UrbanAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UrbanAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UrbanAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `UrbanAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_UrbanAreaBoxMappings_UrbanAreaId` (`UrbanAreaId` ASC) ,
  INDEX `IDX_UrbanAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_UrbanAreaBoxMappings_UrbanAreaId`
    FOREIGN KEY (`UrbanAreaId` )
    REFERENCES `Timm`.`UrbanAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`UrbanAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`UrbanAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`UrbanAreaCoordinates` (
  `Id` INT NOT NULL ,
  `UrbanAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_UrbanAreaCoordinates_UrbanAreaId` (`UrbanAreaId` ASC) ,
  CONSTRAINT `FK_UrbanAreaCoordinates_UrbanAreaId`
    FOREIGN KEY (`UrbanAreaId` )
    REFERENCES `Timm`.`UrbanAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`Users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Users` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Users` (
  `Id` INT NOT NULL ,
  `UserName` VARCHAR(64) NOT NULL ,
  `Password` VARCHAR(64) NOT NULL ,
  `Enabled` BIT NOT NULL ,
  `FullName` VARCHAR(128) NOT NULL ,
  `UserCode` VARCHAR(64) NOT NULL ,
  `Role` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  UNIQUE INDEX `UserName_UNIQUE` (`UserName` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`VotingDistrictAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`VotingDistrictAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`VotingDistrictAreas` (
  `Id` INT NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `VTD` VARCHAR(8) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `CountyCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NULL DEFAULT NULL ,
  `LSAD_TRAN` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`VotingDistrictAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`VotingDistrictAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`VotingDistrictAreaBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `VotingDistrictAreaId` INT NOT NULL ,
  INDEX `FK_VotingDistrictAreaBoxMappings_VotingDistrictAreaId` (`VotingDistrictAreaId` ASC) ,
  INDEX `IDX_VotingDistrictAreaBoxMappings_BoxId` (`BoxId` ASC) ,
  PRIMARY KEY (`Id`) ,
  CONSTRAINT `FK_VotingDistrictAreaBoxMappings_VotingDistrictAreaId`
    FOREIGN KEY (`VotingDistrictAreaId` )
    REFERENCES `Timm`.`VotingDistrictAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`VotingDistrictAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`VotingDistrictAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`VotingDistrictAreaCoordinates` (
  `Id` INT NOT NULL ,
  `VotingDistrictAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_VotingDistrictAreaCoordinates_VotingDistrictAreaId` (`VotingDistrictAreaId` ASC) ,
  CONSTRAINT `FK_VotingDistrictAreaCoordinates_VotingDistrictAreaId`
    FOREIGN KEY (`VotingDistrictAreaId` )
    REFERENCES `Timm`.`VotingDistrictAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`SubMapCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SubMapCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SubMapCoordinates` (
  `Id` INT NOT NULL AUTO_INCREMENT ,
  `SubMapId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_SubMapCoordinates_SubMap` (`SubMapId` ASC) ,
  CONSTRAINT `FK_SubMapCoordinates_SubMap`
    FOREIGN KEY (`SubMapId` )
    REFERENCES `Timm`.`SubMaps` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CustomAreas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CustomAreas` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CustomAreas` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(32) NOT NULL ,
  `IsEnabled` INT NOT NULL ,
  `total` INT NOT NULL ,
  `Description` VARCHAR(256) NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`BlockGroups`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`BlockGroups` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`BlockGroups` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(64) NOT NULL ,
  `Code` VARCHAR(8) NOT NULL ,
  `StateCode` VARCHAR(8) NOT NULL ,
  `CountyCode` VARCHAR(8) NOT NULL ,
  `TractCode` VARCHAR(8) NOT NULL ,
  `LSAD` VARCHAR(8) NULL DEFAULT NULL ,
  `LSADTrans` VARCHAR(64) NULL DEFAULT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `IsEnabled` INT NOT NULL ,
  `OTotal` INT NULL DEFAULT NULL ,
  `Description` VARCHAR(256) NULL DEFAULT NULL ,
  `ArbitraryUniqueCode` VARCHAR(20) NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_BlockGroups_ArbitraryUniqueCode` (`ArbitraryUniqueCode` ASC) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`BlockGroupBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`BlockGroupBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`BlockGroupBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `BlockGroupId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_BLOCKGROUPBOXMAPPINGS_BLOCKGROUPID` (`BlockGroupId` ASC) ,
  INDEX `IDX_BLOCKGROUPBOXMAPPINGS_BOXID` (`BoxId` ASC) ,
  CONSTRAINT `FK_BLOCKGROUPBOXMAPPINGS_BLOCKGROUPID`
    FOREIGN KEY (`BlockGroupId` )
    REFERENCES `Timm`.`BlockGroups` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`BlockGroupCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`BlockGroupCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`BlockGroupCoordinates` (
  `Id` BIGINT NOT NULL ,
  `BlockGroupId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_BLOCKGROUPCOORDINATES_BLOCKGROUPID` (`BlockGroupId` ASC) ,
  CONSTRAINT `FK_BLOCKGROUPCOORDINATES_BLOCKGROUPID`
    FOREIGN KEY (`BlockGroupId` )
    REFERENCES `Timm`.`BlockGroups` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`NdAddresses`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`NdAddresses` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`NdAddresses` (
  `Id` INT NOT NULL ,
  `Street` VARCHAR(128) NOT NULL ,
  `ZipCode` VARCHAR(32) NOT NULL ,
  `Geofence` INT NOT NULL ,
  `Description` VARCHAR(256) NULL DEFAULT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CampaignPercentageColors`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignPercentageColors` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignPercentageColors` (
  `Id` INT NOT NULL AUTO_INCREMENT ,
  `CampaignId` INT NOT NULL ,
  `Min` DOUBLE NOT NULL ,
  `Max` DOUBLE NOT NULL ,
  `ColorId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_CampaignPercentageColors_Campaign` (`CampaignId` ASC) ,
  CONSTRAINT `FK_CampaignPercentageColors_Campaign`
    FOREIGN KEY (`CampaignId` )
    REFERENCES `Timm`.`Campaigns` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `Timm`.`CustomAreaCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CustomAreaCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CustomAreaCoordinates` (
  `Id` INT NOT NULL AUTO_INCREMENT ,
  `CustomAreaId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_CUSTOMAREACOORDINATES_CUSTOMAREAID` (`CustomAreaId` ASC) ,
  CONSTRAINT `FK_CUSTOMAREACOORDINATES_CUSTOMAREAID`
    FOREIGN KEY (`CustomAreaId` )
    REFERENCES `Timm`.`CustomAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CustomAreaBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CustomAreaBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CustomAreaBoxMappings` (
  `Id` INT NOT NULL AUTO_INCREMENT ,
  `BoxId` INT NOT NULL ,
  `CustomAreaId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_CUSTOMAREABOXMAPPINGS_CUSTOMAREAID` (`CustomAreaId` ASC) ,
  INDEX `IDX_CUSTOMAREABOXMAPPINGS_BOXID` (`BoxId` ASC) ,
  CONSTRAINT `FK_CUSTOMAREABOXMAPPINGS_CUSTOMAREAID`
    FOREIGN KEY (`CustomAreaId` )
    REFERENCES `Timm`.`CustomAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`NdAddressCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`NdAddressCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`NdAddressCoordinates` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT ,
  `NdAddressId` INT NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_NdAddressCoordinates_NdAddressId` (`NdAddressId` ASC) ,
  CONSTRAINT `FK_NdAddressCoordinates_NdAddressId`
    FOREIGN KEY (`NdAddressId` )
    REFERENCES `Timm`.`NdAddresses` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`NdAddressBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`NdAddressBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`NdAddressBoxMappings` (
  `Id` INT NOT NULL AUTO_INCREMENT ,
  `BoxId` INT NOT NULL ,
  `NdAddressId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_NdAddressBoxMappings_NdAddressId` (`NdAddressId` ASC) ,
  INDEX `IDX_NdAddressBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_NdAddressBoxMappings_NdAddressId`
    FOREIGN KEY (`NdAddressId` )
    REFERENCES `Timm`.`NdAddresses` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`PremiumCRoutes`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`PremiumCRoutes` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`PremiumCRoutes` (
  `Id` INT NOT NULL ,
  `FIPSSTCO` VARCHAR(8) NOT NULL ,
  `GEOCODE` VARCHAR(16) NOT NULL ,
  `ZIP` VARCHAR(8) NOT NULL ,
  `CROUTE` VARCHAR(8) NOT NULL ,
  `STATE_FIPS` VARCHAR(8) NOT NULL ,
  `STATE` VARCHAR(8) NOT NULL ,
  `COUNTY` VARCHAR(32) NOT NULL ,
  `ZIP_NAME` VARCHAR(32) NOT NULL ,
  `HOME_COUNT` INT NULL ,
  `BUSINESS_COUNT` INT NULL ,
  `APT_COUNT` INT NULL ,
  `TOTAL_COUNT` INT NULL ,
  `MinLatitude` DOUBLE NOT NULL ,
  `MaxLatitude` DOUBLE NOT NULL ,
  `MinLongitude` DOUBLE NOT NULL ,
  `MaxLongitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `IsEnabled` INT NOT NULL ,
  `OTotal` INT NULL ,
  `Description` VARCHAR(256) NULL ,
  `PartCount` INT NOT NULL DEFAULT 0 ,
  `IsInnerShape` INT NOT NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_PremiumCRoutes_GEOCODE` (`GEOCODE` ASC) )
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`PremiumCRouteSelectMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`PremiumCRouteSelectMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`PremiumCRouteSelectMappings` (
  `Id` INT NOT NULL ,
  `ThreeZipAreaId` INT NOT NULL ,
  `FiveZipAreaId` INT NOT NULL ,
  `PremiumCRouteId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_PremiumCRouteSelectMappings_PremiumCRoutes` (`PremiumCRouteId` ASC) ,
  INDEX `FK_PremiumCRouteSelectMappings_ThreeZipArea` (`ThreeZipAreaId` ASC) ,
  INDEX `FK_PremiumCRouteSelectMappings_FiveZipArea` (`FiveZipAreaId` ASC) ,
  CONSTRAINT `FK_PremiumCRouteSelectMappings_PremiumCRoutes`
    FOREIGN KEY (`PremiumCRouteId` )
    REFERENCES `Timm`.`PremiumCRoutes` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_PremiumCRouteSelectMappings_ThreeZipArea`
    FOREIGN KEY (`ThreeZipAreaId` )
    REFERENCES `Timm`.`ThreeZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_PremiumCRouteSelectMappings_FiveZipArea`
    FOREIGN KEY (`FiveZipAreaId` )
    REFERENCES `Timm`.`FiveZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`PremiumCRouteCoordinates`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`PremiumCRouteCoordinates` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`PremiumCRouteCoordinates` (
  `Id` INT NOT NULL ,
  `PreminumCRouteId` INT NOT NULL ,
  `ShapeId` INT NOT NULL ,
  `Longitude` DOUBLE NOT NULL ,
  `Latitude` DOUBLE NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_PremiumCRouteCoordinates_PremiumCRoutes` (`PreminumCRouteId` ASC) ,
  CONSTRAINT `FK_PremiumCRouteCoordinates_PremiumCRoutes`
    FOREIGN KEY (`PreminumCRouteId` )
    REFERENCES `Timm`.`PremiumCRoutes` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`PremiumCRouteBoxMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`PremiumCRouteBoxMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`PremiumCRouteBoxMappings` (
  `Id` INT NOT NULL ,
  `BoxId` INT NOT NULL ,
  `PreminumCRouteId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_PremiumCRouteBoxMappings_PremiumCRoutes` (`PreminumCRouteId` ASC) ,
  INDEX `IDX_PremiumCRouteBoxMappings_BoxId` (`BoxId` ASC) ,
  CONSTRAINT `FK_PremiumCRouteBoxMappings_PremiumCRoutes`
    FOREIGN KEY (`PreminumCRouteId` )
    REFERENCES `Timm`.`PremiumCRoutes` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`BlockGroupSelectMappings`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`BlockGroupSelectMappings` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`BlockGroupSelectMappings` (
  `Id` INT NOT NULL ,
  `ThreeZipAreaId` INT NOT NULL ,
  `FiveZipAreaId` INT NOT NULL ,
  `TractId` INT NOT NULL ,
  `BlockGroupId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_BlockGroupSelectMappings_ThreeZipArea` (`ThreeZipAreaId` ASC) ,
  INDEX `FK_BlockGroupSelectMappings_FiveZipArea` (`FiveZipAreaId` ASC) ,
  INDEX `FK_BlockGroupSelectMappings_Tract` (`TractId` ASC) ,
  INDEX `FK_BlockGroupSelectMappings_BlockGroup` (`BlockGroupId` ASC) ,
  CONSTRAINT `FK_BlockGroupSelectMappings_ThreeZipArea`
    FOREIGN KEY (`ThreeZipAreaId` )
    REFERENCES `Timm`.`ThreeZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_BlockGroupSelectMappings_FiveZipArea`
    FOREIGN KEY (`FiveZipAreaId` )
    REFERENCES `Timm`.`FiveZipAreas` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_BlockGroupSelectMappings_Tract`
    FOREIGN KEY (`TractId` )
    REFERENCES `Timm`.`Tracts` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_BlockGroupSelectMappings_BlockGroup`
    FOREIGN KEY (`BlockGroupId` )
    REFERENCES `Timm`.`BlockGroups` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = MyISAM;


-- -----------------------------------------------------
-- Table `Timm`.`SequenceCounters`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`SequenceCounters` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`SequenceCounters` (
  `BlockGroup` INT NOT NULL ,
  `BlockGroupBoxMapping` INT NOT NULL ,
  `BlockGroupCoordinate` INT NOT NULL ,
  `BlockGroupSelectMapping` INT NOT NULL ,
  `CustomArea` INT NOT NULL ,
  `CustomAreaBoxMapping` INT NOT NULL ,
  `CustomAreaCoordinate` INT NOT NULL ,
  `NdAddress` INT NOT NULL ,
  `NdAddressBoxMapping` INT NOT NULL ,
  `NdAddressCoordinate` INT NOT NULL ,
  `SubMapCoordinate` INT NOT NULL )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`Gtus`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Gtus` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Gtus` (
  `Id` INT NOT NULL ,
  `UniqueID` VARCHAR(50) NULL ,
  `Model` VARCHAR(50) NULL ,
  `IsEnabled` BIT NOT NULL DEFAULT 1 ,
  `UserId` INT NULL ,
  PRIMARY KEY (`Id`) ,
  UNIQUE INDEX `UniqueID_UNIQUE` (`UniqueID` ASC) ,
  INDEX `FK_Gtus_Users` (`UserId` ASC) ,
  CONSTRAINT `FK_Gtus_Users`
    FOREIGN KEY (`UserId` )
    REFERENCES `Timm`.`Users` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`DistributionMaps`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`DistributionMaps` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`DistributionMaps` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(50) NOT NULL ,
  `SubMapId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_DistributionMaps_SubMap` (`SubMapId` ASC) ,
  CONSTRAINT `FK_DistributionMaps_SubMap`
    FOREIGN KEY (`SubMapId` )
    REFERENCES `Timm`.`SubMaps` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`DistributionJobs`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`DistributionJobs` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`DistributionJobs` (
  `Id` INT NOT NULL ,
  `Name` VARCHAR(100) NOT NULL ,
  `CampaignId` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_DistributionJobs_Campaign` (`CampaignId` ASC) ,
  CONSTRAINT `FK_DistributionJobs_Campaign`
    FOREIGN KEY (`CampaignId` )
    REFERENCES `Timm`.`Campaigns` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`DistributionJobMaps`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`DistributionJobMaps` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`DistributionJobMaps` (
  `Id` INT NOT NULL ,
  `DistributionJobId` INT NULL ,
  `DistributionMapId` INT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_DistributionJobMaps_DistributionJobs` (`DistributionJobId` ASC) ,
  INDEX `FK_DistributionJobMaps_DistributionMaps` (`DistributionMapId` ASC) ,
  CONSTRAINT `FK_DistributionJobMaps_DistributionJobs`
    FOREIGN KEY (`DistributionJobId` )
    REFERENCES `Timm`.`DistributionJobs` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_DistributionJobMaps_DistributionMaps`
    FOREIGN KEY (`DistributionMapId` )
    REFERENCES `Timm`.`DistributionMaps` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`Walkers`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Walkers` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Walkers` (
  `Id` INT NOT NULL ,
  `DjRole` INT NOT NULL ,
  `GtuId` INT NULL ,
  `UserId` INT NULL ,
  `DistributionJobId` INT NULL ,
  `FullName` VARCHAR(128) NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_Walkers_Users` (`UserId` ASC) ,
  INDEX `FK_Walkers_Gtus` (`GtuId` ASC) ,
  INDEX `FK_Walkers_DistributionJobs` (`DistributionJobId` ASC) ,
  CONSTRAINT `FK_Walkers_Users`
    FOREIGN KEY (`UserId` )
    REFERENCES `Timm`.`Users` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Walkers_Gtus`
    FOREIGN KEY (`GtuId` )
    REFERENCES `Timm`.`Gtus` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Walkers_DistributionJobs`
    FOREIGN KEY (`DistributionJobId` )
    REFERENCES `Timm`.`DistributionJobs` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`Auditors`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Auditors` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Auditors` (
  `Id` INT NOT NULL ,
  `DjRole` INT NOT NULL ,
  `GtuId` INT NULL ,
  `UserId` INT NULL ,
  `DistributionJobId` INT NULL ,
  `FullName` VARCHAR(128) NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_Auditors_Users` (`UserId` ASC) ,
  INDEX `FK_Auditors_Gtus` (`GtuId` ASC) ,
  INDEX `FK_Auditors_DistributionJobs` (`DistributionJobId` ASC) ,
  CONSTRAINT `FK_Auditors_Users`
    FOREIGN KEY (`UserId` )
    REFERENCES `Timm`.`Users` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Auditors_Gtus`
    FOREIGN KEY (`GtuId` )
    REFERENCES `Timm`.`Gtus` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Auditors_DistributionJobs`
    FOREIGN KEY (`DistributionJobId` )
    REFERENCES `Timm`.`DistributionJobs` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`Drivers`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`Drivers` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`Drivers` (
  `Id` INT NOT NULL ,
  `DjRole` INT NOT NULL ,
  `GtuId` INT NULL ,
  `UserId` INT NULL ,
  `DistributionJobId` INT NULL ,
  `FullName` VARCHAR(128) NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_Drivers_Users` (`UserId` ASC) ,
  INDEX `FK_Drivers_Gtus` (`GtuId` ASC) ,
  INDEX `FK_Drivers_DistributionJobs` (`DistributionJobId` ASC) ,
  CONSTRAINT `FK_Drivers_Users`
    FOREIGN KEY (`UserId` )
    REFERENCES `Timm`.`Users` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Drivers_Gtus`
    FOREIGN KEY (`GtuId` )
    REFERENCES `Timm`.`Gtus` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Drivers_DistributionJobs`
    FOREIGN KEY (`DistributionJobId` )
    REFERENCES `Timm`.`DistributionJobs` (`Id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`CampaignCRouteImported`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`CampaignCRouteImported` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`CampaignCRouteImported` (
  `Id` INT NOT NULL ,
  `PremiumCRouteId` INT NOT NULL ,
  `CampaignId` INT NOT NULL ,
  `Total` INT NOT NULL ,
  `Penetration` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_CampaignCRouteImported_PremiumCRouteId` (`PremiumCRouteId` ASC) ,
  INDEX `IDX_CampaignCRouteImported_CampaignId` (`CampaignId` ASC) )
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Timm`.`RadiusRecords`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Timm`.`RadiusRecords` ;

CREATE  TABLE IF NOT EXISTS `Timm`.`RadiusRecords` (
  `Id` INT NOT NULL ,
  `RadiusId` INT NOT NULL ,
  `AreaId` INT NOT NULL ,
  `Classification` INT NOT NULL ,
  PRIMARY KEY (`Id`) ,
  INDEX `IDX_RadiusRecords_RadiusId` (`RadiusId` ASC) )
ENGINE = InnoDB;



SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
