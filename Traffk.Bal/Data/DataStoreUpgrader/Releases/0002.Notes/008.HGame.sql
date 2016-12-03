insert into Releases values ('urn:traffk.com/portal', '2016-10-7', 'Half-Life', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Pagination' Title union all
	select 'new' ChangeType, 'Attachment support for emails' Title union all
	select 'bugfix' ChangeType, 'Moved Traffk specific images to be hosted on secure site to fix SSL warning' Title union
	select 'new' ChangeType, 'Browsing of additional health record types of: Care Alerts, Demographics, High Cost Diagnosis, Historical Scores, MemberPCP, Participation, Quality Metics, Score, Visits' Title 
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
