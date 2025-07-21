INSERT INTO Users
    (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, Created, IsActive, SecurityStamp, PasswordHash, AccessFailedCount, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'school1@example.com', 'SCHOOL1@EXAMPLE.COM', 'school1@example.com', 'SCHOOL1@EXAMPLE.COM', 1, 'John', 'Doe', GETUTCDATE(), 1, NEWID(), 'AQAAAAIAAYagAAAAEExample_Password_Hash_Here', 0, 1, 1, 1),
    ('22222222-2222-2222-2222-222222222222', 'school2@example.com', 'SCHOOL2@EXAMPLE.COM', 'school2@example.com', 'SCHOOL2@EXAMPLE.COM', 1, 'Jane', 'Smith', GETUTCDATE(), 1, NEWID(), 'AQAAAAIAAYagAAAAEExample_Password_Hash_Here', 0, 1, 1, 1);

INSERT INTO Addresses (Street,Coordinate,Comment)
VALUES
('Aurel Vlaicu',0xE6100000010C52B81E85EB913740295C8FC2F5084740,''),
('Strada Libertății',0xE6100000010C52B81E85EB913740295C8FC2F5084740,''),
('Strada Unirii',0xE6100000010C52B81E85EB913740295C8FC2F5084740,'');

INSERT INTO Images
    (Id, FileName, ContentType, Url, Description, Created, CreatedBy, LastModified, LastModifiedBy)
VALUES
    ('33333333-3333-3333-3333-333333333333', 'school1-logo.png', 'image/png', 'https://example.com/images/school1-logo.png', 'School 1 Logo', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('44444444-4444-4444-4444-444444444444', 'school2-logo.jpg', 'image/jpeg', 'https://example.com/images/school2-logo.jpg', 'School 2 Logo', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('77777777-7777-7777-7777-777777777777', 'instructor1.jpg', 'image/jpeg', 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=300&h=300&fit=crop&crop=face', 'Adrian Moldovan Profile', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('88888888-8888-8888-8888-888888888888', 'instructor2.jpg', 'image/jpeg', 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=300&h=300&fit=crop&crop=face', 'Anca Popescu Profile', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('99999999-9999-9999-9999-999999999999', 'instructor3.jpg', 'image/jpeg', 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=300&fit=crop&crop=face', 'Florin Iacob Profile', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'instructor4.jpg', 'image/jpeg', 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=300&h=300&fit=crop&crop=face', 'Maria Vasilescu Profile', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'instructor5.jpg', 'image/jpeg', 'https://images.unsplash.com/photo-1560250097-0b93528c311a?w=300&h=300&fit=crop&crop=face', 'Cristian Popescu Profile', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'instructor6.jpg', 'image/jpeg', 'https://images.unsplash.com/photo-1494790108755-2616c96ce75f?w=300&h=300&fit=crop&crop=face', 'Elena Ionescu Profile', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO Schools
    (Id, OwnerId, Name, CompanyName, Email, Slug, PhoneNumber, PhoneNumbersGroups, BussinessHours, CityId, CountyId, AddressId, Slogan, Description, IconId, IsApproved, Created, CreatedBy, LastModified,"Statistics")
VALUES
    ('55555555-5555-5555-5555-555555555555',
    '11111111-1111-1111-1111-111111111111',
    'Elite Driving Academy',
    'Elite Driving Academy SRL',
    'contact@elitedriving.ro',
    'elite-driving-academy-srl',
    '+40721123456',
    '[{{"Name":"Main Office","PhoneNumbers":[{{"Value":"+40721123456"}}]}}]',
    '{{"BussinessHoursEntries":[{{"DayOfTheWeek":[1,2,3,4],"Times":[{{"Start":{{"Hour":8,"Minute":0}},"End":{{"Hour":18,"Minute":0}},"Span":"10:00:00"}}]}},{{"DayOfTheWeek":[5],"Times":[{{"Start":{{"Hour":8,"Minute":0}},"End":{{"Hour":17,"Minute":0}},"Span":"09:00:00"}}]}},{{"DayOfTheWeek":[6],"Times":[{{"Start":{{"Hour":9,"Minute":0}},"End":{{"Hour":14,"Minute":0}},"Span":"05:00:00"}}]}}]}}',
    46,
    2,
    (SELECT TOP 1 Id FROM Addresses),
    'Learn to drive with confidence!',
    'Professional driving school with experienced instructors and modern vehicles.',
    '33333333-3333-3333-3333-333333333333',
    1,
    GETUTCDATE(),
    'System',
    GETUTCDATE(),
    '{{"NumberOfStudents":123}}'),

    ('66666666-6666-6666-6666-666666666666', '22222222-2222-2222-2222-222222222222', 'Professional Drive Center', 'Professional Drive Center SRL', 'info@profdrive.ro', 'professional-drive-center-srl', '+40722987654', '[{{"Name":"Reception","PhoneNumbers":[{{"Value":"+40722987654"}}]}}]', '{{"BussinessHoursEntries":[{{"DayOfTheWeek":[1,2,3,4],"Times":[{{"Start":{{"Hour":9,"Minute":0}},"End":{{"Hour":19,"Minute":0}},"Span":"10:00:00"}}]}},{{"DayOfTheWeek":[5],"Times":[{{"Start":{{"Hour":9,"Minute":0}},"End":{{"Hour":18,"Minute":0}},"Span":"09:00:00"}}]}},{{"DayOfTheWeek":[6],"Times":[{{"Start":{{"Hour":10,"Minute":0}},"End":{{"Hour":15,"Minute":0}},"Span":"05:00:00"}}]}}]}}', 46, 2, (SELECT TOP 1 Id FROM Addresses), 'Your path to safe driving!', 'Comprehensive driving education with focus on road safety and practical skills.', '44444444-4444-4444-4444-444444444444', 0, GETUTCDATE(), 'System', GETUTCDATE(),'{{"NumberOfStudents":465}}');

INSERT INTO SchoolCategories
    (SchoolId, VehicleCategoryId)
VALUES
    ('55555555-5555-5555-5555-555555555555', 6),
    ('55555555-5555-5555-5555-555555555555', 4),
    ('66666666-6666-6666-6666-666666666666', 6);

INSERT INTO Teams
    (Id, SchoolId, Created, CreatedBy, LastModified, LastModifiedBy)
VALUES
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', '55555555-5555-5555-5555-555555555555', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '66666666-6666-6666-6666-666666666666', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO Instructors
    (Id, TeamId, FirstName, LastName, BirthYear, YearsExperience, Specialization, Description, Phone, Email, ProfileImageId, Gender, Created, CreatedBy, LastModified, LastModifiedBy)
VALUES
    ('11111111-aaaa-bbbb-cccc-111111111111', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Adrian', 'Moldovan', 1979, 22, 'Chief Instructor, Passenger Transport', 'With over 22 years of experience in the automotive field, Adrian is the chief instructor of our team. Specialized in all categories (B, C, D), he has a methodical and patient approach, being recognized for his ability to train responsible and safe drivers. His vast experience in passenger transport makes him an excellent mentor for those who want to obtain higher categories.', '+40742567891', 'adrian.moldovan@elitedriving.ro', '77777777-7777-7777-7777-777777777777', 'Male', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('22222222-aaaa-bbbb-cccc-222222222222', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Anca', 'Popescu', 1992, 8, 'Category B Instructor', 'Anca is known for her calm and encouraging style, being preferred by anxious students or those with their first driving experience. With 8 years of experience, she has developed efficient techniques to help students overcome their fear of driving and gain confidence quickly. Her friendly and professional approach makes lessons relaxing and productive.', '+40742567892', 'anca.popescu@elitedriving.ro', '88888888-8888-8888-8888-888888888888', 'Female', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('33333333-aaaa-bbbb-cccc-333333333333', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Florin', 'Iacob', 1986, 12, 'Category B and C Instructor', 'Florin combines 12 years of experience with a real passion for road education. Specialized in categories B and C, he is appreciated for clear explanations and practical demonstrations. His teaching style is based on repetition and consolidation, ensuring that each student perfectly understands traffic rules before moving on to practice.', '+40742567893', 'florin.iacob@elitedriving.ro', '99999999-9999-9999-9999-999999999999', 'Male', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('44444444-aaaa-bbbb-cccc-444444444444', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Maria', 'Vasilescu', 1989, 9, 'Category B Instructor', 'Maria brings a unique perspective to driving instruction, combining modern techniques with a personalized approach for each student. With 9 years of experience, she is an expert at adapting her teaching style according to the personality and needs of each student. Students appreciate her patience and attention to detail, guaranteeing solid preparation for the exam.', '+40742567894', 'maria.vasilescu@elitedriving.ro', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Female', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('55555555-aaaa-bbbb-cccc-555555555555', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Cristian', 'Popescu', 1985, 15, 'Category B and C Instructor', 'Cristian is a seasoned instructor with 15 years of experience, known for his systematic approach to teaching. He specializes in categories B and C, helping students build confidence through structured learning. His emphasis on safety and defensive driving techniques has earned him excellent student reviews.', '+40733456789', 'cristian.popescu@profdrive.ro', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Male', GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
    ('66666666-aaaa-bbbb-cccc-666666666666', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Elena', 'Ionescu', 1990, 7, 'Category B Instructor', 'Elena brings fresh energy and modern teaching methods to our team. With 7 years of experience, she excels at working with young drivers and those who need extra confidence building. Her patient and supportive approach helps nervous students feel comfortable behind the wheel.', '+40734567890', 'elena.ionescu@profdrive.ro', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'Female', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO InstructorVehicleCategories
    (InstructorsId, VehicleCategoriesId)
VALUES
    ('11111111-aaaa-bbbb-cccc-111111111111', 6), -- Adrian - Category B
    ('11111111-aaaa-bbbb-cccc-111111111111', 9), -- Adrian - Category C
    ('11111111-aaaa-bbbb-cccc-111111111111', 13), -- Adrian - Category D
    ('22222222-aaaa-bbbb-cccc-222222222222', 6), -- Anca - Category B
    ('33333333-aaaa-bbbb-cccc-333333333333', 6), -- Florin - Category B
    ('33333333-aaaa-bbbb-cccc-333333333333', 9), -- Florin - Category C
    ('44444444-aaaa-bbbb-cccc-444444444444', 6), -- Maria - Category B
    ('55555555-aaaa-bbbb-cccc-555555555555', 6), -- Cristian - Category B
    ('55555555-aaaa-bbbb-cccc-555555555555', 9), -- Cristian - Category C
    ('66666666-aaaa-bbbb-cccc-666666666666', 6); -- Elena - Category B