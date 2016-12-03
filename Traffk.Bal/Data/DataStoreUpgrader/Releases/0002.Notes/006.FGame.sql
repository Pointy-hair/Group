insert into Releases values ('urn:traffk.com/portal', '2016-09-16', 'Flight Simulator', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Site now connected to domain portal-dev.traffk.com' Title union all
	select 'new' ChangeType, 'HTTPS connections are now required' Title union all
	select 'new' ChangeType, 'Health records now appear in the CRM' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
