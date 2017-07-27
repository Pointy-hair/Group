insert into Releases values ('urn:traffk.com/portal', '2017-06-04', 'Valhalla', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add ability to pull data from FTP' Title
	union all
	select 'new' ChangeType, 'Initial code for integrating Redis' Title
	union all
	select 'new' ChangeType, 'Design scheduled report UI' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
