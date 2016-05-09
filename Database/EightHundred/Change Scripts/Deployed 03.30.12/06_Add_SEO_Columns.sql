USE [EightHundred]
GO

-- ADD SEO COLUMNS TO LOCATION TABLE

if not Exists(select * from sys.columns where Name = N'MetaTitle'  
            and Object_ID = Object_ID(N'Location'))
begin
ALTER TABLE Location ADD MetaTitle NVARCHAR(500);
end
ELSE
print 'Invalid column name: MetaTitle (column already exists)'

if not Exists(select * from sys.columns where Name = N'MetaTitle'  
            and Object_ID = Object_ID(N'Location'))
begin
ALTER TABLE Location ADD MetaDescription NVARCHAR(500);
end
ELSE
print 'Invalid column name: MetaDescription (column already exists)';

if not Exists(select * from sys.columns where Name = N'MetaTitle'  
            and Object_ID = Object_ID(N'Location'))
begin
ALTER TABLE Location ADD MetaKeywords NVARCHAR(500);
end
ELSE
print 'Invalid column name: MetaKeywords (column already exists)';