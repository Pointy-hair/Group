create table HangfireTenantMap
(
	HangfireTenantMapId int not null identity primary key,
	JobId int not null references Hangfire.Job(Id) unique,
	TenantId int not null
)

GO

