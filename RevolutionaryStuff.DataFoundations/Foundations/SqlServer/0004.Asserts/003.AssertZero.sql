CREATE proc [db].[AssertZero]
	@actual int,
	@msg varchar(4000)=null
as
begin

	if (@actual not in (0))
	begin

		set @msg = 'AssertZero FAIL as ['+cast(@actual as varchar(10))+'] <> 0: '+@msg;
		throw 50001, @msg, 1;

	end

end
