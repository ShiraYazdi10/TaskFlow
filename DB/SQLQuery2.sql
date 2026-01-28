CREATE OR ALTER PROCEDURE dbo.Tasks_Create
    @Title NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @StatusId INT,
    @PriorityId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Tasks (Title, Description, StatusId, PriorityId)
    VALUES (@Title, @Description, @StatusId, @PriorityId);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO
CREATE OR ALTER PROCEDURE dbo.Tasks_Update
    @Id INT,
    @Title NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @StatusId INT,
    @PriorityId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Tasks
    SET Title=@Title,
        Description=@Description,
        StatusId=@StatusId,
        PriorityId=@PriorityId
    WHERE Id=@Id;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO
CREATE OR ALTER PROCEDURE dbo.Tasks_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.Id,
        t.Title,
        t.Description,
        t.StatusId,
        s.Name AS StatusName,
        t.PriorityId,
        p.Name AS PriorityName,
        t.CreatedAt
    FROM dbo.Tasks t
    JOIN dbo.TaskStatuses s ON s.Id = t.StatusId
    JOIN dbo.Priorities p ON p.Id = t.PriorityId
    WHERE t.Id = @Id;
END
GO
CREATE OR ALTER PROCEDURE dbo.Tasks_GetAll
    @Search NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.Id,
        t.Title,
        t.Description,
        t.StatusId,
        s.Name AS StatusName,
        t.PriorityId,
        p.Name AS PriorityName,
        t.CreatedAt
    FROM dbo.Tasks t
    JOIN dbo.TaskStatuses s ON s.Id = t.StatusId
    JOIN dbo.Priorities p ON p.Id = t.PriorityId
    WHERE (@Search IS NULL OR t.Title LIKE N'%' + @Search + N'%')
    ORDER BY t.CreatedAt DESC;
END
GO
CREATE OR ALTER PROCEDURE dbo.Tasks_ChangeStatus
    @Id INT,
    @StatusId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Tasks
    SET StatusId=@StatusId
    WHERE Id=@Id;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO
EXEC dbo.Tasks_Create @Title=N'Test task', @Description=N'hello', @StatusId=1, @PriorityId=2;
EXEC dbo.Tasks_GetAll @Search=NULL;
