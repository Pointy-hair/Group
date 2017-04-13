insert into Releases values ('urn:traffk.com/portal', '2017-03-12', 'NASCAR by Papyrus', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Add Risk Index section' Title
	union all
	select 'new' ChangeType, 'Restore messaging capabilities' Title
	union all
	select 'new' ChangeType, 'Update UI of action confirmations' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
