insert into Releases values ('urn:traffk.com/portal', '2017-05-21', 'Tetris', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Increase expiration time for background jobs' Title
	union all
	select 'new' ChangeType, 'Update dependencies' Title
	union all
	select 'new' ChangeType, 'Stress test data retrieval on Tableau server' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
