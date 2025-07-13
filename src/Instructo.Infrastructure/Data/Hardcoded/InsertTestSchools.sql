INSERT INTO Users
    (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, Created, IsActive, SecurityStamp, PasswordHash, AccessFailedCount, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'school1@example.com', 'SCHOOL1@EXAMPLE.COM', 'school1@example.com', 'SCHOOL1@EXAMPLE.COM', 1, 'John', 'Doe', GETUTCDATE(), 1, NEWID(), 'AQAAAAIAAYagAAAAEExample_Password_Hash_Here', 0, 1, 1, 1),
    ('22222222-2222-2222-2222-222222222222', 'school2@example.com', 'SCHOOL2@EXAMPLE.COM', 'school2@example.com', 'SCHOOL2@EXAMPLE.COM', 1, 'Jane', 'Smith', GETUTCDATE(), 1, NEWID(), 'AQAAAAIAAYagAAAAEExample_Password_Hash_Here', 0, 1, 1, 1);

INSERT INTO Addresses (Street,Coordinate,Comment)
VALUES
('Aurel Vlaicu',0xE6100000010C52B81E85EB913740295C8FC2F5084740,'')

INSERT INTO Images
    (Id, FileName, ContentType, Url, Description, Created, CreatedBy, LastModified, LastModifiedBy)
VALUES
    ('33333333-3333-3333-3333-333333333333', 'school1-logo.png', 'image/png', 'https://example.com/images/school1-logo.png', 'School 1 Logo', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('44444444-4444-4444-4444-444444444444', 'school2-logo.jpg', 'image/jpeg', 'https://example.com/images/school2-logo.jpg', 'School 2 Logo', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO Schools
    (Id, OwnerId, Name, CompanyName, Email, Slug, PhoneNumber, PhoneNumbersGroups, BussinessHours, CityId, CountyId, AddressId, Slogan, Description, IconId, IsApproved, Created, CreatedBy, LastModified)
VALUES
    ('55555555-5555-5555-5555-555555555555', '11111111-1111-1111-1111-111111111111', 'Elite Driving Academy', 'Elite Driving Academy SRL', 'contact@elitedriving.ro', 'elite-driving-academy-srl', '+40721123456', '[{{"Name":"Main Office","PhoneNumbers":[{{"Value":"+40721123456"}}]}}]', '{{"BussinessHoursEntries":[{{"DayOfTheWeek":[1,2,3,4],"Times":[{{"Start":{{"Hour":8,"Minute":0}},"End":{{"Hour":18,"Minute":0}},"Span":"10:00:00"}}]}},{{"DayOfTheWeek":[5],"Times":[{{"Start":{{"Hour":8,"Minute":0}},"End":{{"Hour":17,"Minute":0}},"Span":"09:00:00"}}]}},{{"DayOfTheWeek":[6],"Times":[{{"Start":{{"Hour":9,"Minute":0}},"End":{{"Hour":14,"Minute":0}},"Span":"05:00:00"}}]}}]}}', 46, 2, (SELECT TOP 1 Id FROM Addresses), 'Learn to drive with confidence!', 'Professional driving school with experienced instructors and modern vehicles.', '33333333-3333-3333-3333-333333333333', 1, GETUTCDATE(), 'System', GETUTCDATE()),
    ('66666666-6666-6666-6666-666666666666', '22222222-2222-2222-2222-222222222222', 'Professional Drive Center', 'Professional Drive Center SRL', 'info@profdrive.ro', 'professional-drive-center-srl', '+40722987654', '[{{"Name":"Reception","PhoneNumbers":[{{"Value":"+40722987654"}}]}}]', '{{"BussinessHoursEntries":[{{"DayOfTheWeek":[1,2,3,4],"Times":[{{"Start":{{"Hour":9,"Minute":0}},"End":{{"Hour":19,"Minute":0}},"Span":"10:00:00"}}]}},{{"DayOfTheWeek":[5],"Times":[{{"Start":{{"Hour":9,"Minute":0}},"End":{{"Hour":18,"Minute":0}},"Span":"09:00:00"}}]}},{{"DayOfTheWeek":[6],"Times":[{{"Start":{{"Hour":10,"Minute":0}},"End":{{"Hour":15,"Minute":0}},"Span":"05:00:00"}}]}}]}}', 46, 2, (SELECT TOP 1 Id FROM Addresses), 'Your path to safe driving!', 'Comprehensive driving education with focus on road safety and practical skills.', '44444444-4444-4444-4444-444444444444', 0, GETUTCDATE(), 'System', GETUTCDATE());

INSERT INTO SchoolCategories
    (SchoolId, VehicleCategoryId)
VALUES
    ('55555555-5555-5555-5555-555555555555', 6),
    ('55555555-5555-5555-5555-555555555555', 4),
    ('66666666-6666-6666-6666-666666666666', 6);