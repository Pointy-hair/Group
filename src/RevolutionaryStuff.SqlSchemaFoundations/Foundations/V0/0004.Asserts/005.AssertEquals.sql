CREATE proc [db].[AssertEquals]
	@expected sql_variant,
	@actual sql_variant,
	@msg varchar(4000)=null
as
begin

	if (
		(@expected is not null and (@actual is null or @expected<>@actual)) or
		(@expected is null and @actual is not null)
		)
	begin

		set @msg = 
			'AssertEquals FAIL as '+
			case when @actual is null then 'null' else '['+cast(@actual as varchar(max))+']' end +
			' <> '+
			case when @expected is null then 'null' else '['+cast(@expected as varchar(max))+']' end +
			': '+
			coalesce(@msg,'');			
		throw 50001, @msg, 1;

	end

end
