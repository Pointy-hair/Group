insert into Releases values ('urn:traffk.com/portal', '2017-04-23', 'Rogue', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add improved mobile menu' Title
	union all
	select 'new' ChangeType, 'Refactor Tableau Service' Title
	union all
	select 'new' ChangeType, 'Research background job server' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
