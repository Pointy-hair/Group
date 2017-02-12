insert into Releases values ('urn:traffk.com/portal', '2017-02-12', 'Jumpman', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Finished getting rid of DocumentDB' Title
	union all
	select 'new' ChangeType, 'Revamped messaging infrastructure'
	union all
	select 'new' ChangeType, 'Conversion to UI "Frankie"' 
	union all
	select 'new' ChangeType, 'Converted from Power BI to Tableau' 
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
