delete from usergroupmappings;

delete from groupprivilegemappings;

delete from groups;

delete from privilege;


INSERT INTO `groups` (`Id`,`Name`) VALUES
 (1,'Walker'),
 (46,'Sales'),
 (47,'CampaignSupervisor'),
 (48,'Driver'),
 (49,'Client'),
 (50,'Auditor'),
 (51,'DistributionManager'),
 (52,'DistributionSupervisor'),
 (53,'Administrator');


INSERT INTO `privilege` (`Id`,`Name`,`Value`) VALUES 
 (1,'CreateCampaign',1),
 (2,'SubmapView',2),
 (3,'HistoricalData',3),
 (4,'PublishCampaign',4),
 (5,'RealTimeWalkerLocation',5),
 (6,'AssignNameToGTU',6),
 (7,'StartSuspendStopGTU',7),
 (8,'NotifiedByEmail',8),
 (9,'AssignGTUToDM',9),
 (10,'AssignAuditor',10),
 (11,'GeneratePdf',11),
 (12,'CreateDriverClientAuditorAccount',12),
 (13,'DNDManagement',13),
 (14,'SubmapDMView',14),
 (15,'AssignManager',15),
 (16,'GTUManagement',16),
 (17,'CreateDriverClientAuditorManagerAccount',17),
 (18,'CreateAllTypeUserAccount',18),
 (19,'DMView',19),
 (20,'ReportOfDM',20),
 (21,'UndoPublishCampaign',21);


INSERT INTO `groupprivilegemappings` (`Id`,`GroupId`,`PrivilegeId`) VALUES 
 (84,46,1),
 (85,46,2),
 (86,46,3),
 (87,47,1),
 (88,47,4),
 (89,47,13),
 (90,48,5),
 (91,48,19),
 (92,49,2),
 (93,49,3),
 (94,49,5),
 (95,50,3),
 (96,50,5),
 (97,50,6),
 (98,50,7),
 (99,50,8),
 (100,50,20),
 (101,51,3),
 (102,51,5),
 (103,51,6),
 (104,51,7),
 (105,51,8),
 (106,51,9),
 (107,51,10),
 (108,51,11),
 (109,51,12),
 (110,51,13),
 (111,51,14),
 (112,52,3),
 (113,52,5),
 (114,52,6),
 (115,52,7),
 (116,52,8),
 (117,52,9),
 (118,52,10),
 (119,52,11),
 (120,52,13),
 (121,52,14),
 (122,52,15),
 (123,52,16),
 (124,52,17),
 (125,53,1),
 (126,53,3),
 (127,53,4),
 (128,53,5),
 (129,53,6),
 (130,53,7),
 (131,53,9),
 (132,53,10),
 (133,53,11),
 (134,53,13),
 (135,53,14),
 (136,53,15),
 (137,53,16),
 (138,53,18),
 (139,46,4),
 (140,52,21);


insert into usergroupmappings (UserId,GroupId) values (1,53);

