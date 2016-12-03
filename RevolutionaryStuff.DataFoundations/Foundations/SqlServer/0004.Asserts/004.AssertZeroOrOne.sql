CREATE proc [db].[AssertZeroOrOne]
	@actual int,
	@msg varchar(4000)=null
as
begin

	if (@actual not in (0,1))
	begin

		set @msg = 'AssertZeroOrOne FAIL as ['+cast(@actual as varchar(10))+'] not in (0,1): '+@msg;
		throw 50001, @msg, 1;

	end

end

GO
