insert into Releases values ('urn:traffk.com/portal', '2017-05-21', 'Tetris', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Create background job for new Tableau tenants' Title
	union all
	select 'new' ChangeType, 'Publish new background job runner/server' Title
	union all
	select 'new' ChangeType, 'Add timeout for report cache' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
