insert into Releases values ('urn:traffk.com/portal', '2017-06-25', 'Yukon Trail', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Complete Tableau report export' Title
	union all
	select 'new' ChangeType, 'Complete background job runner to create new tenant' Title
	union all
	select 'new' ChangeType, 'Integrate Redis cacher and custom cacher into new Traffk cacher' Title
	union all
	select 'new' ChangeType, 'Bug fixes' Title

) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
