insert into Releases values ('urn:traffk.com/portal', '2017-05-09', 'Sim City', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add new background job runner' Title
	union all
	select 'new' ChangeType, 'Refactor database' Title
	union all
	select 'new' ChangeType, 'Add Fiscal Year configuration UI' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
