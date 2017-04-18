insert into Releases values ('urn:traffk.com/portal', '2017-04-09', 'Populous', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add UI for editing fiscal year' Title
	union all
	select 'new' ChangeType, 'Initial work on Tableau tenant creation' Title
	union all
	select 'new' ChangeType, 'PHI/Non-PHI report acccess now based on permissions' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
