-- Database schema of Comments SPA in MySQL syntax, for MySQL Workbench.
-- The application itself runs on SQL Server; this script mirrors the EF Core model
-- (server/src/Comments.Infrastructure/Persistence/Migrations) with the closest MySQL types:
-- nvarchar(n) -> VARCHAR(n) utf8mb4, nvarchar(max) -> LONGTEXT, datetime2 -> DATETIME(6).
-- docs/schema.mwb is generated from this file: File -> Import -> Reverse Engineer MySQL Create Script.

CREATE SCHEMA IF NOT EXISTS `Comments` DEFAULT CHARACTER SET utf8mb4;
USE `Comments`;

CREATE TABLE `Comments` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `ParentId` INT NULL COMMENT 'Direct parent; NULL for a root comment',
  `RootId` INT NULL COMMENT 'Root of the thread; loads a whole thread in one query',
  `UserName` VARCHAR(50) NOT NULL COMMENT 'Latin letters and digits',
  `Email` VARCHAR(254) NOT NULL,
  `HomePage` VARCHAR(2048) NULL,
  `Text` LONGTEXT NOT NULL COMMENT 'Sanitized XHTML: a, code, i, strong',
  `IpAddress` VARCHAR(45) NOT NULL COMMENT 'Client identification',
  `UserAgent` VARCHAR(512) NOT NULL COMMENT 'Client identification',
  `CreatedAt` DATETIME(6) NOT NULL COMMENT 'UTC',
  PRIMARY KEY (`Id`),
  INDEX `IX_Comments_ParentId_CreatedAt` (`ParentId`, `CreatedAt`),
  INDEX `IX_Comments_ParentId_UserName` (`ParentId`, `UserName`),
  INDEX `IX_Comments_ParentId_Email` (`ParentId`, `Email`),
  INDEX `IX_Comments_RootId` (`RootId`),
  CONSTRAINT `FK_Comments_Comments_ParentId`
    FOREIGN KEY (`ParentId`) REFERENCES `Comments` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Comments_Comments_RootId`
    FOREIGN KEY (`RootId`) REFERENCES `Comments` (`Id`) ON DELETE RESTRICT
) ENGINE = InnoDB;

CREATE TABLE `Attachments` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `CommentId` INT NOT NULL,
  `Type` INT NOT NULL COMMENT '1 = Image, 2 = Text',
  `OriginalName` VARCHAR(255) NOT NULL,
  `StoredName` VARCHAR(100) NOT NULL COMMENT 'File name on disk (GUID + extension)',
  `ContentType` VARCHAR(50) NOT NULL,
  `SizeBytes` INT NOT NULL,
  `Width` INT NULL COMMENT 'Images only, max 320',
  `Height` INT NULL COMMENT 'Images only, max 240',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Attachments_CommentId` (`CommentId`),
  CONSTRAINT `FK_Attachments_Comments_CommentId`
    FOREIGN KEY (`CommentId`) REFERENCES `Comments` (`Id`) ON DELETE CASCADE
) ENGINE = InnoDB;
