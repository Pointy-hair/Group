insert into Releases values ('urn:traffk.com/portal', '2017-07-02', 'Zork', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Custom domain matching' Title
	union all
	select 'new' ChangeType, 'Upgrade Tableau Server' Title
	union all
	select 'new' ChangeType, 'Initial work on handling recurrence patterns' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
