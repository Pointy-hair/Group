insert into Releases values ('urn:traffk.com/portal', '2017-02-27', 'M.U.L.E', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'User interface improvements' Title
	union all
	select 'new' ChangeType, 'Change Traffk logo in navigation' Title
	union all
	select 'new' ChangeType, 'Add ability to resend user invitations' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
