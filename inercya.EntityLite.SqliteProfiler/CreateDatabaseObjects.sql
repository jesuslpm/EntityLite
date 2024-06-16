CREATE TABLE Statements
(
	StatementId INTEGER PRIMARY KEY AUTOINCREMENT,
	CommandTextHash INTEGER NOT NULL,
	CommandText TEXT NOT NULL,
	SampleCommandText TEXT NOT NULL,
	MaxTime REAL NOT NULL,
	MinTime REAL NOT NULL,
	TotalTime REAL NOT NULL,
	SampleTime REAL NOT NULL,
	ExecutionCount INTEGER NOT NULL,
	MaxTimeParams TEXT NOT NULL,
	MinTimeParams TEXT NOT NULL,
	SampleParams TEXT NOT NULL
); 

CREATE INDEX IX_Statements_CommandTextHash ON Statements(CommandTextHash);

CREATE TABLE Executions
(
	ExecutionId INTEGER PRIMARY KEY AUTOINCREMENT,
	StatementId INTEGER NOT NULL,
	ExecutionDate DATETIME NOT NULL,
	ExecutionTime REAL NOT NULL,
	DataServiceInstanceId TEXT NULL,
	ApplicationContext TEXT NULL
);

CREATE VIEW Statement_Basic 
AS
	SELECT 
		StatementId, CommandText, SampleCommandText, MaxTime, MinTime, TotalTime, SampleTime, 
		(TotalTime / ExecutionCount) AS AvgTime, ExecutionCount, MaxTimeParams, MinTimeParams, SampleParams
	FROM 
		Statements;

CREATE VIEW Statement_Detailed
AS
	SELECT 
		S.StatementId, S.CommandText, S.SampleCommandText, AC.ApplicationContexts, S.MaxTime, S.MinTime, S.TotalTime, S.SampleTime, 
		(S.TotalTime / S.ExecutionCount) AS AvgTime, TotalTime / sqrt(ExecutionCount) AS Weight, S.ExecutionCount, S.MaxTimeParams, S.MinTimeParams, S.SampleParams
	FROM 
		Statements S
    LEFT OUTER JOIN (
      SELECT StatementId, group_concat(ApplicationContext, ', ') AS ApplicationContexts
      FROM
      (
        SELECT StatementId, CASE WHEN ApplicationContext IS NULL THEN '<unknown>' ELSE ApplicationContext END AS ApplicationContext
        FROM
        (
          SELECT StatementId, ApplicationContext
          FROM Executions
          GROUP BY StatementId, ApplicationContext
        ) T
       ) T
      GROUP BY StatementId    
    ) AC ON S.StatementId = AC.StatementId;

