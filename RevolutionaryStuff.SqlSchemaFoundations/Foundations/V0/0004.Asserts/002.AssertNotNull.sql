CREATE proc [db].[AssertNotNull]
	@actual int,
	@msg varchar(4000)=null
as
begin

	if (@actual is NULL)
	begin

		set @msg = 'AssertNotNull FAIL as value is null: '+@msg;
		throw 50001, @msg, 1;

	end

end
