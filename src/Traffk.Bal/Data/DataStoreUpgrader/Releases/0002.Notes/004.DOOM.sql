insert into Releases values ('urn:traffk.com/portal', '2016-09-09', 'DOOM', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Styling the user interface' Title union all
	select 'new' ChangeType, 'Ability to create notes tagged to a CRM user' Title union all
	select 'new' ChangeType, 'User defineable re-usable values that can be used across emails' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
