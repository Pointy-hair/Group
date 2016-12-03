insert into Releases values ('urn:traffk.com/portal', '2016-08-19', 'Archon', 'Initial Software Release')

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Integration with SQL Azure for storage of users and related data' Title union all
	select 'new' ChangeType, 'Multi-Tenancy' Title union all
	select 'new' ChangeType, 'Invitiations for users to join the system' Title union all
	select 'new' ChangeType, 'Configurable roles' Title union all
	select 'new' ChangeType, 'Integration with Azure Tables for tenant files like custom CSS' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
