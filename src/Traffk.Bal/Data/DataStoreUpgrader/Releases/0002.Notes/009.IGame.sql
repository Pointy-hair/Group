﻿insert into Releases values ('urn:traffk.com/portal', '2016-12-05', 'Ikari Warriors', null)

GO

insert into ReleaseChanges
(ReleaseId, ChangeType, Title)
select r.ReleaseId, a.ChangeType, a.Title
from
(
	select 'new' ChangeType, 'Upgrade from .NET Core 1.0 to .NET Core 1.1' Title union all
	select 'new' ChangeType, 'Add support for SerliLog;  App logs now emitted to Azure Tables AppLogs' Title union all
	select 'new' ChangeType, 'Auto-log out of web aplication after N minutes, popup warning give to those still logged in' Title union all
	select 'new' ChangeType, 'Password policy can now be configured on a per tenant basis' Title union all
	select 'new' ChangeType, 'Row level database security to enforce isolation with different database users per tenant' Title union all
	select 'new' ChangeType, 'Row level database security to allow for database level soft deletes via instead of triggers' Title union all
	select 'new' ChangeType, 'Conversion of source control to GIT' Title union all
	select 'new' ChangeType, 'Date dimension incorportated into health records' 
) a,
(
	select max(ReleaseId) ReleaseId from Releases
) r

GO
