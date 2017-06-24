exec db.TablePropertySet  'ShardMapsGlobal', '1', @propertyName='AddToDbContext', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardMapsGlobal', '1', @propertyName='GeneratePoco', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardsGlobal', '1', @propertyName='AddToDbContext', @tableSchema='__ShardManagement'
exec db.TablePropertySet  'ShardsGlobal', '1', @propertyName='GeneratePoco', @tableSchema='__ShardManagement'
