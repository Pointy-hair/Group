create schema ETL

GO

create view ETL.MemberMap
AS
select distinct m.TenantId, m.PersonContactId, m.MemberId, m.MemberNumber, cCarriers.CarrierNumber, cPeople.ForeignId
from 
	health.members m (nolock)
		inner join
	contacts cCarriers (nolock)
		on cCarriers.ContactId=m.CarrierContactId
		inner join
	contacts cPeople (nolock)
		on cPeople.ContactId=m.PersonContactId
GO
