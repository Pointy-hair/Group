insert into Releases values ('urn:traffk.com/portal', '2017-04-16', 'Quake', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add UI to pin navigation menu' Title
	union all
	select 'new' ChangeType, 'Complete work on Tableau tenant creation' Title
	union all
	select 'new' ChangeType, 'Add clickable IDs to Health screens' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
