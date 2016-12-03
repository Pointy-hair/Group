insert into Releases values ('urn:traffk.com/portal', '2016-09-02', 'Civilization', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, '"Amazon.com" like filtering on CRM screen' Title union all
	select 'new' ChangeType, 'Release Log screen' Title union all
	select 'new' ChangeType, 'Tag editing on contacts' Title union all
	select 'fix' ChangeType, 'Sorting bug that when clicking on a column header you returned to the homepage' Title union all
	select 'new' ChangeType, 'Ability to direct message a CRM user' Title union all
	select 'new' ChangeType, 'Email tracking' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
