insert into Releases values ('urn:traffk.com/portal', '2016-08-28', 'The Bard''s Tale', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Integration with Azure DocumentDB for basic CRM functionality' Title union all
	select 'new' ChangeType, 'Click on listing headings to sort' Title union all
	select 'new' ChangeType, 'Nested message layouts' Title
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
