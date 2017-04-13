insert into Releases values ('urn:traffk.com/portal', '2017-03-26', 'Oregon Trail', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add Tableau Report Metadata' Title
	union all
	select 'new' ChangeType, 'Extend user invitation expiration' Title
	union all
	select 'new' ChangeType, 'Add friendly error message page' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
