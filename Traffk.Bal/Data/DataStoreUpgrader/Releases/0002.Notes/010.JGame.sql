insert into Releases values ('urn:traffk.com/portal', '2016-12-11', 'Jumpman', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Finished getting rid of DocumentDB' 
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
