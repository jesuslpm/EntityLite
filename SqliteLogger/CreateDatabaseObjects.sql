CREATE TABLE Statements
(
	StatementId INTEGER PRIMARY KEY AUTOINCREMENT,
	CommandTextHash INTEGER NOT NULL,
	CommandText TEXT NOT NULL,
	MaxTime INTEGER NOT NULL,
	MinTime INTEGER NOT NULL,
	TotalTime INTEGER NOT NULL,
	SampleTime INTEGER NOT NULL,
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
	ExecutionTime INTEGER NOT NULL
);

CREATE VIEW Statement_Basic 
AS
	SELECT 
		StatementId, CommandTextHash, CommandText, MaxTime, MinTime, TotalTime, SampleTime, 
		(TotalTime / ExecutionCount) AS AvgTime, ExecutionCount, MaxTimeParams, MinTimeParams, SampleParams
	FROM 
		Statements;

