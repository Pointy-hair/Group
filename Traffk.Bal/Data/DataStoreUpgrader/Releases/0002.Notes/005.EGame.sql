insert into Releases values ('urn:traffk.com/portal', '2016-09-16', 'Epic Pinball', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Recurrence settings on communication blasts' Title union all
	select 'new' ChangeType, 'Updated to use .NET Core 1.0.1' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
