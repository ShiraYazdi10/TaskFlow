CREATE TABLE TaskStatuses (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Priorities (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Tasks (
    Id INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000),
    StatusId INT,
    PriorityId INT,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (StatusId) REFERENCES TaskStatuses(Id),
    FOREIGN KEY (PriorityId) REFERENCES Priorities(Id)
);

INSERT INTO TaskStatuses (Name) VALUES ('Open'), ('In Progress'), ('Done');
INSERT INTO Priorities (Name) VALUES ('Low'), ('Medium'), ('High');
