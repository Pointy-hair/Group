CREATE proc [db].[AssertNull]
	@actual int,
	@msg varchar(4000)=null
as
begin

	if (@actual is not NULL)
	begin

		set @msg = 'AssertNull FAIL as value ['+cast(@actual as varchar(20))+'] is not null: '+@msg;
		throw 50001, @msg, 1;

	end

end
