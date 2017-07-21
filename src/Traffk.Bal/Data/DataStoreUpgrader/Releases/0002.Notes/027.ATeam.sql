insert into Releases values ('urn:traffk.com/portal', '2017-07-17', 'Angels', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add Data Source, Fetch, Item user interface' Title
	union all
	select 'new' ChangeType, 'Add Tableau report scheduling UI' Title
	union all
	select 'new' ChangeType, 'Make project compatible with .NET Standard 1.5' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
