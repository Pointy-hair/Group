insert into Releases values ('urn:traffk.com/portal', '2017-02-19', 'Leisure Suit Larry', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Change inactivity timespan to 15 minutes' Title
	union all
	select 'new' ChangeType, 'Convert action confirmations to Frankie UI' Title
	union all
	select 'new' ChangeType, 'Update navigation bar functionality' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
