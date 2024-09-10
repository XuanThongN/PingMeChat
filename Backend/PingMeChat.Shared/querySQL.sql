
INSERT INTO Students (Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, Code, FirstName, LastName, Birthday, Address, PhoneNumber, Email, Status)
VALUES
    ('92525536-fbee-46f1-8447-8304d90f6ee2', '2023-06-02 10:00:00', 'created_by_1', '2023-06-02 10:00:00', 'updated_by_1', 'code1', 'John', 'Doe', '2000-01-01', '123 Main St', '1234567890', 'john.doe@example.com', 1),
    ('87ae8c83-f264-409d-8d38-49c1ccecebce', '2023-06-02 11:00:00', 'created_by_2', '2023-06-02 11:00:00', 'updated_by_2', 'code2', 'Jane', 'Smith', '1999-02-02', '456 Elm St', '0987654321', 'jane.smith@example.com', 1),
    ('beb8db43-be6c-403c-a239-fee362bb8516', '2023-06-02 12:00:00', 'created_by_3', '2023-06-02 12:00:00', 'updated_by_3', 'code3', 'David', 'Johnson', '2001-03-03', '789 Oak St', '5432167890', 'david.johnson@example.com', 1);


INSERT INTO TranscriptRequests (Id, StudentName, StudentCode, Birthday, Birthplace, PhoneNumber, Email, BelongToClass, BelongToMajor, BelongToFaculty, EducationalSystem, Reason, DateOSubmission, FromYear, ToYear, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, DomicileRegistration, SchoolYearType, SchoolYear, FromSemester, ToSemester, FromShoolYear, ToSchoolYear, StudentId)
VALUES
  ('1', 'John Doe', '12345', '2000-01-01', 'City A', '123456789', 'john@example.com', 'Class A', 'Major A', 'Faculty A', 'System A', 'Reason A', GETDATE(), 2020, 2022, GETDATE(), 'admin', GETDATE(), 'admin', 'Address A', 1, 2022, 'Semester A', 'Semester B', '2020', '2022', '92525536-fbee-46f1-8447-8304d90f6ee2'),
  ('2', 'Jane Smith', '67890', '1999-01-01', 'City B', '987654321', 'jane@example.com', 'Class B', 'Major B', 'Faculty B', 'System B', 'Reason B', GETDATE(), 2019, 2021, GETDATE(), 'admin', GETDATE(), 'admin', 'Address B', 1, 2021, 'Semester C', 'Semester D', '2019', '2021', '92525536-fbee-46f1-8447-8304d90f6ee2'),
  -- Add more rows here with the same structure
  ('10', 'Mike Johnson', '54321', '2001-01-01', 'City C', '555555555', 'mike@example.com', 'Class C', 'Major C', 'Faculty C', 'System C', 'Reason C', GETDATE(), 2023, 2024, GETDATE(), 'admin', GETDATE(), 'admin', 'Address C', 1, 2024, 'Semester E', 'Semester F', '2023', '2024', 'beb8db43-be6c-403c-a239-fee362bb8516');

INSERT INTO FormTypes (Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, Code, Name, Description)
VALUES
   ('11111111-1111-1111-1111-111111111111', GETDATE(), 'John Doe', GETDATE(), 'John Doe', 'BIA_DXCBD', 'Đơn xin cấp bảng điểm', 'Description 1'),
   ('22222222-2222-2222-2222-222222222222', GETDATE(), 'Jane Smith', GETDATE(), 'Jane Smith', 'BIA_XCBTN', 'Đơn xin cấp bằng tốt nghiệp', 'Description 2'),
   ('33333333-3333-3333-3333-333333333333', GETDATE(), 'Mike Johnson', GETDATE(), 'Mike Johnson', 'BIA_DDKHP', 'Đơn đăng ký học phần', 'Description 3'),
   ('44444444-4444-4444-4444-444444444444', GETDATE(), 'Emily Davis', GETDATE(), 'Emily Davis', 'BIA_XCTTV', 'Đơn xin cấp thẻ thư viện', 'Description 4');
