insert into Releases values ('urn:traffk.com/portal', '2016-09-30', 'Grand Theft Auto', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'TraffkContactsFromEligibilityRunner background service' Title union all
	select 'new' ChangeType, 'TraffkCommunicationBlastRunner background service' Title union all
	select 'new' ChangeType, 'UI for creating an advanced blast query' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
