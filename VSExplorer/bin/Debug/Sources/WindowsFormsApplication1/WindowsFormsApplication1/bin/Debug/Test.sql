USE model; 
GO 
EXEC sys.sp_addextendedproperty 
@name = N'Test example', 
@value = N'This is an example.', 
@level0type = N'SCHEMA', @level0name = 'dbo', 
@level1type = N'TABLE', @level1name = 'my_table'; 
GO