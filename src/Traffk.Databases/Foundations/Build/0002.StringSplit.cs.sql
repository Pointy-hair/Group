create function dbo.LeftOf(@s nvarchar(4000), @pivot nvarchar(4000))
returns nvarchar(4000)
begin

	declare @z int
	set @z = charindex(@pivot, @s)
	if (@z is not null)
	begin
		set @s = left(@s, @z-1)
	end
	return @s

end

GO

create function dbo.RightOf(@s nvarchar(4000), @pivot nvarchar(4000))
returns nvarchar(4000)
begin

	declare @z int
	set @z = charindex(@pivot, @s)
	if (@z is not null)
	begin
		set @s = right(@s, len(@s)-@z)
	end
	else begin
		set @s = null
	end
	return @s

end

GO

select dbo.LeftOf('jason.thomas', '.'), dbo.RightOf('jason.thomas', '.')

GO

CREATE FUNCTION SplitString(@csv_str NVARCHAR(4000), @delimiter nvarchar(20))
 RETURNS @splittable table (val nvarchar(max), pos int)
AS
BEGIN  
 
-- Check for NULL string or empty sting
    IF  (LEN(@csv_str) < 1 OR @csv_str IS NULL)
    BEGIN
        RETURN
    END
 
    ; WITH csvtbl(i,j, pos)
    AS
    (
        SELECT i=1, j= CHARINDEX(@delimiter,@csv_str+@delimiter), 1
 
        UNION ALL 
 
        SELECT i=j+1, j=CHARINDEX(@delimiter,@csv_str+@delimiter,j+1), pos+1
        FROM csvtbl
        WHERE CHARINDEX(@delimiter,@csv_str+@delimiter,j+1) <> 0
    )   
    INSERT  INTO @splittable(val, pos)
    SELECT  SUBSTRING(@csv_str,i,j-i), pos
    FROM    csvtbl 
 
    RETURN
END  

GO
