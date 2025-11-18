	CREATE DATABASE RecruitmentSystem;
	GO
	USE RecruitmentSystem;
	GO
	CREATE TABLE JobTypes (
		JobTypeID INT PRIMARY KEY IDENTITY(1,1),
		Name NVARCHAR(50) NOT NULL UNIQUE
	);

	CREATE TABLE ExperienceLevels (
		LevelID INT PRIMARY KEY IDENTITY(1,1),
		Name NVARCHAR(50) NOT NULL UNIQUE
	);

	CREATE TABLE Locations (
		LocationID INT PRIMARY KEY IDENTITY(1,1),
		City NVARCHAR(100) NOT NULL,
		Country NVARCHAR(100) NOT NULL,
		CONSTRAINT UC_CityCountry UNIQUE (City, Country)
	);

	CREATE TABLE Companies (
		CompanyID INT PRIMARY KEY IDENTITY(1,1),
		Name NVARCHAR(255) NOT NULL,
		TaxCode NVARCHAR(20) UNIQUE NOT NULL,
		Email NVARCHAR(255) UNIQUE NOT NULL,
		PasswordHash NVARCHAR(255) NOT NULL,
		Phone NVARCHAR(20) NOT NULL,
		Website NVARCHAR(255),
		Description NVARCHAR(MAX),
		LogoUrl NVARCHAR(512),
		IsLocked BIT NOT NULL DEFAULT 0,
		Address NVARCHAR(MAX),
		Verified BIT DEFAULT 0,
		CreatedAt DATETIME2 DEFAULT GETDATE(),
		UpdatedAt DATETIME2 DEFAULT GETDATE()
	);
	CREATE TABLE Students (
		StudentID INT PRIMARY KEY IDENTITY(1,1),
		StudentCode NVARCHAR(20) UNIQUE NOT NULL,
		PasswordHash NVARCHAR(255) NOT NULL,
		FullName NVARCHAR(255) NOT NULL,
		AvatarUrl NVARCHAR(512),
		DateOfBirth DATE,
		Phone NVARCHAR(20),
		GPA DECIMAL(3,2),
		IsLocked BIT NOT NULL DEFAULT 0,
		GraduationYear INT,
		GitHubProfile NVARCHAR(255),
		CreatedAt DATETIME2 DEFAULT GETDATE(),
		UpdatedAt DATETIME2 DEFAULT GETDATE()
	);
	CREATE TABLE Skills (
		SkillID INT PRIMARY KEY IDENTITY(1,1),
		Name NVARCHAR(100) NOT NULL UNIQUE,
		Description NVARCHAR(MAX)
	);
	CREATE TABLE StudentSkills (
		StudentID INT NOT NULL,
		SkillID INT NOT NULL,
		PRIMARY KEY (StudentID, SkillID),
		FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
		FOREIGN KEY (SkillID) REFERENCES Skills(SkillID)
	);
	CREATE TABLE JobPostings (
		JobID INT PRIMARY KEY IDENTITY(1,1),
		CompanyID INT NOT NULL,
		JobTypeID INT NOT NULL,
		LevelID INT NOT NULL,
		LocationID INT,
		Title NVARCHAR(255) NOT NULL,
		Description NVARCHAR(MAX) NOT NULL,
		Requirements NVARCHAR(MAX) NOT NULL,
		Benefits NVARCHAR(MAX),
		SalaryRange NVARCHAR(100),
		ApplicationDeadline DATETIME2 NOT NULL,
		Vacancies INT DEFAULT 1,
		IsActive BIT DEFAULT 1,
		IsApproved BIT DEFAULT 0,
		CreatedAt DATETIME2 DEFAULT GETDATE(),
		UpdatedAt DATETIME2 DEFAULT GETDATE(),
		FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID) ON DELETE CASCADE,
		FOREIGN KEY (JobTypeID) REFERENCES JobTypes(JobTypeID),
		FOREIGN KEY (LevelID) REFERENCES ExperienceLevels(LevelID),
		FOREIGN KEY (LocationID) REFERENCES Locations(LocationID)
	);
	CREATE TABLE JobSkills (
		JobID INT NOT NULL,
		SkillID INT NOT NULL,
		PRIMARY KEY (JobID, SkillID),
		FOREIGN KEY (JobID) REFERENCES JobPostings(JobID) ON DELETE CASCADE,
		FOREIGN KEY (SkillID) REFERENCES Skills(SkillID)
	);
	CREATE TABLE Applications (
		ApplicationID INT PRIMARY KEY IDENTITY(1,1),
		JobID INT NOT NULL,
		StudentID INT NOT NULL,
		CoverLetter NVARCHAR(MAX),
		ResumeUrl NVARCHAR(512) NOT NULL,
		Status NVARCHAR(20) NOT NULL CHECK (Status IN ('pending', 'reviewing', 'approved', 'rejected')) DEFAULT 'pending',
		AppliedAt DATETIME2 DEFAULT GETDATE(),
		ReviewedAt DATETIME2,
		FOREIGN KEY (JobID) REFERENCES JobPostings(JobID) ON DELETE CASCADE,
		FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
		CONSTRAINT UC_JobStudent UNIQUE (JobID, StudentID)
	);
	CREATE TABLE Interviews (
		InterviewID INT PRIMARY KEY IDENTITY(1,1),
		ApplicationID INT NOT NULL,
		InterviewType NVARCHAR(20) CHECK (InterviewType IN ('online', 'in-person')) NOT NULL,
		StartTime DATETIME2 NOT NULL,
		EndTime DATETIME2 NOT NULL,
		Location NVARCHAR(MAX),
		OnlineLink NVARCHAR(512),
		Notes NVARCHAR(MAX),
		Result NVARCHAR(20) CHECK (Result IN ('passed', 'failed', 'pending')) DEFAULT 'pending',
		FOREIGN KEY (ApplicationID) REFERENCES Applications(ApplicationID) ON DELETE CASCADE
	);

	CREATE TABLE Admins (
		AdminID INT PRIMARY KEY IDENTITY(1,1),
		Email NVARCHAR(255) UNIQUE NOT NULL,
		PasswordHash NVARCHAR(255) NOT NULL,
		FullName NVARCHAR(255) NOT NULL,
		CreatedAt DATETIME2 DEFAULT GETDATE()
	);

	CREATE TABLE Notifications (
		NotificationID INT PRIMARY KEY IDENTITY(1,1),
		UserID INT NOT NULL,
		UserType NVARCHAR(20) CHECK (UserType IN ('student', 'company', 'admin')) NOT NULL,
		Message NVARCHAR(MAX) NOT NULL,
		IsRead BIT DEFAULT 0,
		CreatedAt DATETIME2 DEFAULT GETDATE()
	);
	CREATE TABLE ApprovalHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobId INT NOT NULL,
    Action NVARCHAR(50) NOT NULL, 
    ActionDate DATETIME NOT NULL,
    AdminId INT NOT NULL,
    CONSTRAINT FK_ApprovalHistory_JobPostings FOREIGN KEY (JobId) REFERENCES JobPostings(JobId)
	);

	CREATE INDEX idx_CompanyName ON Companies(Name);
	CREATE INDEX idx_JobTitle ON JobPostings(Title);
	CREATE INDEX idx_ApplicationStatus ON Applications(Status);
	CREATE INDEX idx_StudentCode ON Students(StudentCode);
	GO
	INSERT INTO JobTypes (Name) VALUES
	(N'Toàn thời gian'),
	(N'Bán thời gian'),
	(N'Thực tập');

	INSERT INTO ExperienceLevels (Name) VALUES
	(N'Không yêu cầu'),
	(N'Dưới 1 năm'),
	(N'1-3 năm');

	INSERT INTO Locations (City, Country) VALUES
	(N'Hà Nội', N'Việt Nam'),
	(N'TP.HCM', N'Việt Nam'),
	(N'Đà Nẵng', N'Việt Nam');

	INSERT INTO Companies (Name, TaxCode, Email,PasswordHash, Phone, Website, Description, LogoUrl, Address, Verified) VALUES
	(N'Công ty FPT Software', N'0101234567', N'recruit@fpt.com','$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'0241234567', N'https://fptsoftware.com', N'FPT Software là một trong những tập đoàn công nghệ hàng đầu tại Việt Nam, chuyên cung cấp các giải pháp công nghệ thông tin toàn diện cho khách hàng trên toàn cầu. Với đội ngũ kỹ sư giàu kinh nghiệm và công nghệ tiên tiến, FPT Software cam kết mang lại sản phẩm chất lượng cao, giúp chuyển đổi số và tối ưu hóa quy trình kinh doanh của các doanh nghiệp.', N'/images/logos/2c098358-fdca-4a0d-b8ed-f1f67439bcd9.png', N'123 Đường Láng, Hà Nội', 1),
	(N'Công ty TMA Solutions', N'0209876543', N'hr@tma.com','$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'0289876543', N'https://tmasolutions.com',  N'TMA Solutions là công ty outsourcing CNTT hàng đầu, chuyên cung cấp dịch vụ phát triển phần mềm, kiểm thử và bảo trì ứng dụng cho khách hàng trong và ngoài nước. Với tầm nhìn đổi mới sáng tạo và cam kết chất lượng, công ty đã xây dựng mối quan hệ bền vững với nhiều tập đoàn lớn, mang đến các giải pháp công nghệ đột phá đáp ứng mọi nhu cầu kinh doanh.', N'/images/logos/add6c18e-c904-488f-861f-19d299ebca9e.png', N'456 Nguyễn Thị Minh Khai, TP.HCM', 1),
	(N'Công ty Axon Active', N'0304567891', N'jobs@axonactive.com','$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'0236456789', N'https://axonactive.com',N'Axon Active là công ty phát triển phần mềm quốc tế, chuyên cung cấp các giải pháp công nghệ thông tin chất lượng cao cho khách hàng toàn cầu. Với văn hóa sáng tạo và môi trường làm việc linh hoạt, Axon Active tập trung vào phát triển các sản phẩm tiên tiến, góp phần thúc đẩy sự chuyển đổi số và tăng trưởng bền vững cho doanh nghiệp.',N'/images/logos/2a5aa8a0-cbb9-414c-b924-3791de5c7995.png', N'789 Trần Hưng Đạo, Đà Nẵng', 1),
	(N'Công ty Techcombank', N'0401122334', N'hr@techcombank.com','$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'0245678901', N'https://techcombank.com',  N'Techcombank là một trong những ngân hàng thương mại hàng đầu Việt Nam, cung cấp đa dạng các dịch vụ tài chính hiện đại và an toàn. Với triết lý "Sáng tạo - Chuyên nghiệp - Tận tâm", Techcombank không ngừng đổi mới công nghệ, mở rộng dịch vụ nhằm mang đến trải nghiệm tốt nhất cho khách hàng và góp phần vào sự phát triển kinh tế của đất nước.', N'/images/logos/17b0c322-8f0f-4f57-88c8-dd04abf15cde.png', N'191 Bà Triệu, Hà Nội', 1),
	(N'Công ty VNG Corporation', N'0501234567', N'hr@vng.com',N'$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'0281234567', N'https://vng.com', N'VNG Corporation là một trong những tập đoàn công nghệ và giải trí số hàng đầu tại Việt Nam, cung cấp đa dạng các dịch vụ trong lĩnh vực giải trí kỹ thuật số, thương mại điện tử và công nghệ tài chính. Với tinh thần đổi mới sáng tạo và năng động, VNG cam kết mang lại các giải pháp số tiên tiến nhằm nâng cao trải nghiệm người dùng và thúc đẩy quá trình chuyển đổi số trên toàn quốc.',N'/images/logos/e881c0df-d121-4d30-a221-5c8838a42835.png',N'45 Lê Duẩn, Hà Nội', 1),
	(N'Công ty Viettel Group', N'0601234567', N'hr@viettel.com',N'$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'02411223344', N'https://viettel.com', N'Viettel Group là nhà cung cấp dịch vụ viễn thông hàng đầu Việt Nam, nổi bật với công nghệ tiên tiến và dịch vụ toàn diện. Công ty đóng góp quan trọng trong việc xây dựng hạ tầng số, nâng cao kết nối quốc gia và quốc tế, từ đó mang lại trải nghiệm liên lạc chất lượng cho hàng triệu người dùng.',N'/images/logos/cdb3b487-8eaf-4599-9dba-609f6968bcf3.png', N'10 Phan Chu Trinh, Hà Nội', 1),
	(N'Công ty CMC Corporation', N'0701234567', N'hr@cmc.com',N'$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'0289988776', N'https://cmc.com', N'CMC Corporation là nhà cung cấp giải pháp công nghệ thông tin hàng đầu tại Việt Nam, chuyên về chuyển đổi số, an ninh mạng và phát triển phần mềm. Với đội ngũ chuyên gia giàu kinh nghiệm, CMC không ngừng đổi mới để thúc đẩy tiến bộ công nghệ và tối ưu hóa hiệu quả kinh doanh cho các doanh nghiệp trong và ngoài nước.',N'/images/logos/002364c1-31d2-4eed-a8fc-0aa8d5988f22.png', N'123 Nguyễn Văn Cừ, TP.HCM',1),
	(N'Công ty Zalo Group', N'0801234567', N'hr@zalogroup.com',N'$2a$11$mn.Rv6CtXMtlsQr0H0YlceRTE2X6HG6p/6.14ae5ndeNzePCfsMYy', N'02833445566', N'https://zalogroup.com', N'Zalo Group là công ty công nghệ nổi bật với ứng dụng nhắn tin phổ biến và các dịch vụ số đa dạng. Tập trung vào trải nghiệm người dùng và sự đổi mới liên tục, Zalo Group đóng góp tích cực vào việc xây dựng hệ sinh thái số tại Việt Nam, tạo nền tảng cho giao tiếp và kết nối hiện đại.',N'/images/logos/01950917-68db-4134-b264-60f89f96866a.png', N'50 Lê Lợi, TP.HCM',1);
	
	INSERT INTO Students (StudentCode, PasswordHash, FullName, AvatarUrl, DateOfBirth, Phone, GPA, GraduationYear, GitHubProfile) VALUES
	(N'20201234', N'hash123', N'Nguyễn Văn A', N'avatar_a.png', '2002-05-15', N'0912345678', 3.5, 2024, N'github.com/nguyenvana'),
	(N'20205678', N'hash456', N'Trần Thị B', N'avatar_b.png', '2001-08-20', N'0987654321', 3.8, 2023, N'github.com/tranthib'),
	(N'20209012', N'hash789', N'Lê Văn C', N'avatar_c.png', '2003-01-10', N'0934567890', 3.2, 2025, N'github.com/levanc');

	INSERT INTO Skills (Name, Description) VALUES
	(N'C#', N'Ngôn ngữ lập trình hướng đối tượng'),
	(N'Java', N'Ngôn ngữ lập trình đa nền tảng'),
	(N'SQL', N'Quản lý cơ sở dữ liệu'),
	(N'HTML/CSS', N'Thiết kế giao diện web'),
	(N'Python', N'Lập trình AI và dữ liệu');

	INSERT INTO StudentSkills (StudentID, SkillID) VALUES
	(1, 1), -- Nguyễn Văn A biết C#
	(1, 3), -- Nguyễn Văn A biết SQL
	(2, 2), -- Trần Thị B biết Java
	(2, 4), -- Trần Thị B biết HTML/CSS
	(3, 5); -- Lê Văn C biết Python
	SELECT * FROM JobPostings WHERE CompanyId = 1;
		SELECT * FROM Students WHERE StudentId = 1;
	INSERT INTO JobPostings (CompanyID, JobTypeID, LevelID, LocationID, Title, Description, Requirements, Benefits, SalaryRange, ApplicationDeadline, Vacancies) VALUES
	(1, 1, 1, 1, N'Lập trình viên C#', N'Phát triển ứng dụng doanh nghiệp', N'Biết C#, SQL', N'Lương cao, bảo hiểm', N'10-15 triệu', '2025-04-01', 2),
	(2, 3, 1, 2, N'Thực tập sinh Java', N'Hỗ trợ dự án web', N'Cơ bản Java, HTML', N'Hỗ trợ chi phí, đào tạo', N'3-5 triệu', '2025-03-20', 3),
	(3, 2, 2, 3, N'Kỹ sư phần mềm Python', N'Phát triển AI', N'Python, 1 năm kinh nghiệm', N'Môi trường quốc tế', N'15-20 triệu', '2025-03-30', 1),
	(4, 1, 1, 1, N'Chuyên viên tài chính', N'Hỗ trợ tài chính ngân hàng', N'Tốt nghiệp tài chính, biết Excel', N'Lương cạnh tranh, bảo hiểm', N'12-18 triệu', '2025-04-15', 2);

	INSERT INTO JobSkills (JobID, SkillID) VALUES
	(1, 1), -- Lập trình viên C# yêu cầu C#
	(1, 3), -- Lập trình viên C# yêu cầu SQL
	(2, 2), -- Thực tập sinh Java yêu cầu Java
	(2, 4), -- Thực tập sinh Java yêu cầu HTML/CSS
	(3, 5); -- Kỹ sư phần mềm Python yêu cầu Python

	INSERT INTO Applications (JobID, StudentID, CoverLetter, ResumeUrl, Status, AppliedAt) VALUES
	(1, 1, N'Tôi rất quan tâm đến vị trí này', N'resume_a.pdf', N'pending', '2025-03-10 10:00:00'),
	(2, 2, N'Mong được học hỏi tại TMA', N'resume_b.pdf', N'reviewing', '2025-03-11 14:30:00'),
	(3, 3, N'Tôi có kinh nghiệm Python', N'resume_c.pdf', N'approved', '2025-03-12 09:15:00');

	INSERT INTO Interviews (ApplicationID, InterviewType, StartTime, EndTime, Location, OnlineLink, Notes, Result) VALUES
	(3, N'online', '2025-03-15 10:00:00', '2025-03-15 11:00:00', NULL, N'zoom.us/123456', N'Phỏng vấn kỹ thuật', N'pending');

	INSERT INTO Admins (Email, PasswordHash, FullName) VALUES
	(N'admin@xyz.edu.vn', N'hashadmin', N'Nguyễn Thị Admin'),
	(N'admin123@xyz.edu.vn', N'alo123', N'Nguyễn  Admin');

	INSERT INTO Notifications (UserID, UserType, Message, IsRead) VALUES
	(1, N'student', N'Đơn ứng tuyển của bạn đang được xem xét', 0),
	(3, N'company', N'Có ứng viên mới cho vị trí Kỹ sư Python', 0),
	(1, N'admin', N'Công ty Axon Active cần xác minh', 0);


	-- Bảng Giảng viên hướng dẫn
CREATE TABLE Supervisors (
    SupervisorID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);

-- Bảng Kỳ thực tập
CREATE TABLE Internships (
    InternshipID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    CompanyID INT NOT NULL,
    SupervisorID INT NOT NULL,  -- 1 giảng viên duy nhất hướng dẫn sinh viên đó
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Đang thực tập',

    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
    FOREIGN KEY (SupervisorID) REFERENCES Supervisors(SupervisorID)
);

-- Bảng báo cáo hàng tuần (Sinh viên ghi report)
-- Bảng giảng viên hướng dẫn
CREATE TABLE Supervisors (
    SupervisorID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

-- Bảng báo cáo thực tập hàng tuần
CREATE TABLE WeeklyReports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    InternshipID INT NOT NULL,
    WeekNumber INT NOT NULL,
    ReportDate DATE DEFAULT GETDATE(),
    Content NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'Chờ duyệt',
    FOREIGN KEY (InternshipID) REFERENCES Internships(InternshipID)
);

-- Bảng đánh giá của doanh nghiệp
CREATE TABLE CompanyEvaluations (
    EvaluationID INT PRIMARY KEY IDENTITY(1,1),
    InternshipID INT NOT NULL,
    EvaluationDate DATE DEFAULT GETDATE(),
    Score INT CHECK (Score BETWEEN 0 AND 100),
    Comments NVARCHAR(1000),
    FOREIGN KEY (InternshipID) REFERENCES Internships(InternshipID)
);

-- Bảng đánh giá của giảng viên
CREATE TABLE SupervisorEvaluations (
    EvaluationID INT PRIMARY KEY IDENTITY(1,1),
    InternshipID INT NOT NULL,
    EvaluationDate DATE DEFAULT GETDATE(),
    Score INT CHECK (Score BETWEEN 0 AND 100),
    Comments NVARCHAR(1000),
    FOREIGN KEY (InternshipID) REFERENCES Internships(InternshipID)
);

-- Thêm các cột tiêu chí đánh giá vào bảng CompanyEvaluations
ALTER TABLE CompanyEvaluations
ADD CriteriaCompliance decimal(3,1) NOT NULL DEFAULT 0,
    CriteriaTaskPerformance decimal(3,1) NOT NULL DEFAULT 0,
    CriteriaRelationship decimal(3,1) NOT NULL DEFAULT 0;

-- Cập nhật Score về thang điểm 10 cho các đánh giá hiện tại (nếu có)
UPDATE CompanyEvaluations 
SET Score = CASE 
    WHEN Score > 10 THEN ROUND(Score / 10.0, 0)
    ELSE Score 
END;

-- ============================
-- Thêm phần mẫu: bảng Cơ sở pháp lý và liên kết (theo yêu cầu người dùng)
-- ============================

-- Bảng cơ sở pháp lý để hiển thị thông tin tham khảo
CREATE TABLE LegalBases (
	LegalID INT PRIMARY KEY IDENTITY(1,1),
	Title NVARCHAR(255) NOT NULL,            -- Tên văn bản pháp lý
	ReferenceCode NVARCHAR(50),              -- Số hiệu văn bản (VD: 34/2018/QH14)
	IssuedDate DATE,                         -- Ngày ban hành
	IssuedBy NVARCHAR(255),                  -- Cơ quan ban hành (VD: Bộ GD&ĐT)
	Category NVARCHAR(50),                   -- Loại: 'company', 'internship', 'recruitment'
	Description NVARCHAR(MAX),               -- Tóm tắt nội dung liên quan
	DocumentUrl NVARCHAR(500)                -- Link PDF/drive nếu có
);

-- Bảng liên kết công ty với cơ sở pháp lý
CREATE TABLE CompanyLegalBases (
	CompanyID INT NOT NULL,
	LegalID INT NOT NULL,
	ComplianceStatus NVARCHAR(20) DEFAULT 'unknown', -- 'compliant', 'non-compliant', 'unknown'
	VerifiedDate DATE,
	Notes NVARCHAR(MAX),
    
	PRIMARY KEY (CompanyID, LegalID),
	FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
	FOREIGN KEY (LegalID) REFERENCES LegalBases(LegalID)
);

-- Bảng liên kết tin tuyển dụng với cơ sở pháp lý
CREATE TABLE JobPostingLegalBases (
	JobID INT NOT NULL,
	LegalID INT NOT NULL,
	ComplianceStatus NVARCHAR(20) DEFAULT 'unknown',
	CheckedDate DATE,
	Notes NVARCHAR(MAX),
    
	PRIMARY KEY (JobID, LegalID),
	FOREIGN KEY (JobID) REFERENCES JobPostings(JobID),
	FOREIGN KEY (LegalID) REFERENCES LegalBases(LegalID)
);

-- Bảng liên kết thực tập với cơ sở pháp lý
CREATE TABLE InternshipLegalBases (
	InternshipID INT NOT NULL,
	LegalID INT NOT NULL,
	ComplianceStatus NVARCHAR(20) DEFAULT 'compliant',
	AppliedDate DATE DEFAULT GETDATE(),
	Notes NVARCHAR(MAX),
    
	PRIMARY KEY (InternshipID, LegalID),
	FOREIGN KEY (InternshipID) REFERENCES Internships(InternshipID),
	FOREIGN KEY (LegalID) REFERENCES LegalBases(LegalID)
);

-- Thêm indexes cho Legal Compliance
CREATE INDEX IX_LegalBases_Category ON LegalBases(Category);
CREATE INDEX IX_CompanyLegalBases_CompanyID ON CompanyLegalBases(CompanyID);
CREATE INDEX IX_JobPostingLegalBases_JobID ON JobPostingLegalBases(JobID);
CREATE INDEX IX_InternshipLegalBases_InternshipID ON InternshipLegalBases(InternshipID);

-- Insert dữ liệu mẫu cho Supervisors (nếu chưa có)
INSERT INTO Supervisors (Name, Email, Phone, Password, Position, Department) VALUES
(N'TS. Nguyễn Văn Hùng', N'hungnv@xyz.edu.vn', N'0912345678', N'supervisor123', N'Giảng viên chính', N'Khoa Công nghệ thông tin'),
(N'ThS. Trần Thị Lan', N'lantt@xyz.edu.vn', N'0987654321', N'supervisor456', N'Giảng viên', N'Khoa Công nghệ thông tin'),
(N'PGS.TS. Lê Văn Nam', N'namlv@xyz.edu.vn', N'0934567890', N'supervisor789', N'Phó Giáo sư', N'Khoa Công nghệ thông tin');

-- Insert dữ liệu mẫu cho LegalBases
INSERT INTO LegalBases (Title, ReferenceCode, IssuedDate, IssuedBy, Category, Description, DocumentUrl) VALUES
(N'Luật Doanh nghiệp 2020', N'59/2020/QH14', '2020-06-17', N'Quốc hội', N'company', N'Doanh nghiệp phải có giấy phép kinh doanh hợp lệ, đăng ký thuế đầy đủ, địa chỉ kinh doanh rõ ràng.', N'https://thuvienphapluat.vn/van-ban/Doanh-nghiep/Luat-Doanh-nghiep-2020-445015.aspx'),

(N'Nghị định về thực tập sinh', N'143/2018/NĐ-CP', '2018-10-15', N'Chính phủ', N'internship', N'Quy định về quản lý thực tập sinh tại doanh nghiệp. Đánh giá theo 3 tiêu chí: Tuân thủ quy định nội bộ, Hiệu suất thực hiện nhiệm vụ, Quan hệ giao tiếp với đồng nghiệp.', N''),

(N'Thông tư hướng dẫn tuyển dụng', N'15/2021/TT-BLĐTBXH', '2021-03-20', N'Bộ Lao động - Thương binh và Xã hội', N'recruitment', N'Hướng dẫn quy trình tuyển dụng hợp pháp: Mức lương không thấp hơn tối thiểu vùng, mô tả công việc trung thực, không phân biệt đối xử.', N''),

(N'Bộ luật Lao động 2019', N'45/2019/QH14', '2019-11-20', N'Quốc hội', N'recruitment', N'Quy định về thời gian làm việc tối đa 48h/tuần, quyền lợi người lao động, an toàn lao động, bảo hiểm xã hội.', N'https://thuvienphapluat.vn/van-ban/Lao-dong-Tien-luong/Bo-luat-lao-dong-2012-133234.aspx'),

(N'Thông tư về đánh giá thực tập', N'08/2020/TT-BGDĐT', '2020-05-15', N'Bộ Giáo dục và Đào tạo', N'internship', N'Hướng dẫn đánh giá kết quả thực tập: Đánh giá định kỳ, báo cáo tuần/tháng, tiêu chí đánh giá rõ ràng.', N'');

-- Insert dữ liệu mẫu cho CompanyLegalBases (tất cả công ty đều tuân thủ luật doanh nghiệp)
INSERT INTO CompanyLegalBases (CompanyID, LegalID, ComplianceStatus, VerifiedDate, Notes) VALUES
(1, 1, 'compliant', '2024-01-15', N'FPT Software đã được xác minh đầy đủ giấy tờ pháp lý'),
(2, 1, 'compliant', '2024-01-16', N'TMA Solutions có đầy đủ giấy phép kinh doanh'),
(3, 1, 'compliant', '2024-01-17', N'Axon Active đã xác minh thông tin doanh nghiệp'),
(4, 1, 'compliant', '2024-01-18', N'Techcombank là ngân hàng được cấp phép hoạt động'),
(5, 1, 'compliant', '2024-01-19', N'VNG Corporation có đầy đủ giấy tờ pháp lý'),
(6, 1, 'compliant', '2024-01-20', N'Viettel Group là doanh nghiệp nhà nước hợp pháp'),
(7, 1, 'compliant', '2024-01-21', N'CMC Corporation đã được kiểm định'),
(8, 1, 'compliant', '2024-01-22', N'Zalo Group thuộc VNG Corporation');

-- Insert dữ liệu mẫu cho JobPostingLegalBases
INSERT INTO JobPostingLegalBases (JobID, LegalID, ComplianceStatus, CheckedDate, Notes) VALUES
(1, 3, 'compliant', '2025-03-10', N'Mức lương và mô tả công việc phù hợp quy định'),
(1, 4, 'compliant', '2025-03-10', N'Thời gian làm việc tuân thủ Bộ luật Lao động'),
(2, 3, 'compliant', '2025-03-11', N'Vị trí thực tập có mức hỗ trợ hợp lý'),
(2, 4, 'compliant', '2025-03-11', N'Không yêu cầu làm thêm giờ'),
(3, 3, 'compliant', '2025-03-12', N'Mức lương cạnh tranh cho vị trí kỹ sư'),
(3, 4, 'compliant', '2025-03-12', N'Môi trường làm việc quốc tế tuân thủ quy định'),
(4, 3, 'compliant', '2025-03-13', N'Mức lương ngân hàng tuân thủ quy định'),
(4, 4, 'compliant', '2025-03-13', N'Thời gian làm việc hành chính chuẩn');
