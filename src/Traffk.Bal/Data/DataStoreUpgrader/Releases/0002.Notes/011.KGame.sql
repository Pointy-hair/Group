insert into Releases values ('urn:traffk.com/portal', '2017-02-19', 'Kings Quest', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Example New Item' Title
	union all
	select 'fix' ChangeType, 'Example Fix'
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
