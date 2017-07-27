insert into Releases values ('urn:traffk.com/portal', '2017-06-17', 'Xonix', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Complete Redis integration' Title
	union all
	select 'new' ChangeType, 'Initial work on Tableau report export feature' Title
	union all
	select 'new' ChangeType, 'Publish site to prod app service' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
