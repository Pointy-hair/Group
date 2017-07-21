insert into Releases values ('urn:traffk.com/portal', '2017-06-11', 'Wizardry', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Continue work on integrating Redis' Title
	union all
	select 'new' ChangeType, 'Customize background job server for child jobs' Title
	union all
	select 'new' ChangeType, 'Continue working on Tableau background jobs' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
