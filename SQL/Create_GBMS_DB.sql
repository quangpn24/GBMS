CREATE DATABASE GemstonesBusinessManagementSystem;
USE GemstonesBusinessManagementSystem;

CREATE TABLE Account
(
	idAccount int not null PRIMARY KEY,
	username varchar(50) not null,
	password varchar(50) not null
);
CREATE TABLE Goods
(
	idGoods int not null PRIMARY KEY,
	name nvarchar(50) not null,
	importPrice bigint,
	quantity int,
idGoodsType int,
imageFile mediumtext ,
	isDeleted boolean
-- 1 là đã xóa, 0 là bình thường
);

CREATE TABLE GoodsType
(
	idGoodsType int not null PRIMARY KEY,
name nvarchar(50) not null,
profitPercentage double,
unit nvarchar(50),
isActive boolean
);

CREATE TABLE Employee
(
	idEmployee int not null PRIMARY KEY,
	name nvarchar(50) not null,
	gender nvarchar(5) not null,
	phoneNumber varchar(11),
	address text,
	dateofBirth date,
	idPosition int, 
	startingDate date,
	idAccount int,
	imageFile mediumtext ,
	isDeleted boolean
);
CREATE TABLE EmployeePosition
(
	idEmployeePosition int not null PRIMARY KEY,
position nvarchar(50) not null,
salaryBase bigint,
	moneyPerShift bigint,
	moneyPerFault bigint,
	standardWorkDays int,
	isDeleted boolean
);

CREATE TABLE Bill
(
	idBill int not null PRIMARY KEY,
	idAccount int,
	invoiceDate date,
	totalMoney bigint,
	idCustomer int,
	note text
);
CREATE TABLE BillInfo
(
	idBill int not null,
	idGoods int not null,
	quantity int,
	price bigint
);
CREATE TABLE StockReceipt 
(
	idStockReceipt int not null PRIMARY KEY,
	idAccount int,
	receiptDate date,
	totalPaidMoney bigint,
	discount bigint,
    	idSupplier int
);
CREATE TABLE StockReceiptInfo
(
	idStockReceipt int not null,
	idGoods int not null,
	quantity int,
	importPrice  bigint
);

CREATE TABLE Customer 
(
idCustomer int not null PRIMARY KEY,
 	customerName nvarchar(50) not null,
    	phoneNumber varchar(11),
address text(100), 
    	idNumber varchar(12),
	totalSpending bigint,
	idMembership int

);
create table MembershipsType
(
	idMembershipsType int not null PRIMARY KEY,
  	membership nvarchar(50),
	target bigint
);

CREATE TABLE Supplier
(
	idSupplier int not null PRIMARY KEY,
    	name nvarchar(50) not null,
    	address text,
    	phoneNumber varchar(11)
);
CREATE TABLE Service
(
	idService int not null PRIMARY KEY,
name nvarchar(50) not null,
price bigint,
isActive boolean,
	isDeleted boolean
);
CREATE TABLE BillService
(
	idBillService int not null PRIMARY KEY,
    	idAccount int,
    	createdDate date,
   	 total bigint,
    	totalPaidMoney bigint,
   	idCustomer int,
    	status tinyint -- 0 chưa giao, 1 đã giao
);
CREATE TABLE BillServiceInfo
(
	idBillService int,
    	idService int,
	price bigint,
	tips bigint,
    	quantity int,
    	paidMoney bigint,
    	status tinyint, -- 0 chưa giao, 1 đã giao
deliveryDate date
);

CREATE TABLE Parameter
(
	idParameter int not null primary key,
	parameterName nvarchar(50) not null,
	value mediumtext 
);

create table PositionDetail (
    	idEmployeePosition int not null,
    	idPermission int not null,
    	isPermitted boolean
);

create table Permission (
	idPermission int not null primary key,
    	permissionName nvarchar(50) not null
);

-- primary key

ALTER TABLE BillInfo
ADD CONSTRAINT PK_BillInfo_Id PRIMARY KEY(idBill,idGoods);

ALTER TABLE StockReceiptInfo
ADD CONSTRAINT PK_StockReceiptInfo_Id PRIMARY KEY(idStockReceipt,idGoods);

ALTER TABLE BillServiceInfo
ADD CONSTRAINT PK_BillServiceInfo_Id PRIMARY KEY(idBillService,idService);

ALTER TABLE PositionDetail 
ADD CONSTRAINT PK_PositionDetail_Id PRIMARY KEY(idEmployeePosition, idPermission);

-- foreign key
ALTER TABLE Goods
ADD CONSTRAINT FK_Goods_IdGoodsType FOREIGN KEY(idGoodsType) REFERENCES GoodsType(idGoodsType);

ALTER TABLE Bill
ADD CONSTRAINT FK_Bill_IdAccount FOREIGN KEY(idAccount) REFERENCES Account(idAccount);

ALTER TABLE Bill
ADD CONSTRAINT FK_Bill_IdCustomer FOREIGN KEY(idCustomer) REFERENCES Customer(idCustomer);

ALTER TABLE BillInfo
ADD CONSTRAINT FK_BillInfo_IdBill FOREIGN KEY(idBill) REFERENCES Bill(idBill);

ALTER TABLE BillInfo
ADD CONSTRAINT FK_BillInfo_IdGoods FOREIGN KEY(idGoods) REFERENCES Goods(idGoods);

ALTER TABLE StockReceipt 
ADD CONSTRAINT FK_StockReceipt_IdAccount FOREIGN KEY(idAccount) REFERENCES Account(idAccount);

ALTER TABLE StockReceipt 
ADD CONSTRAINT FK_StockReceipt_IdSupplier FOREIGN KEY(idSupplier) REFERENCES Supplier(idSupplier);

ALTER TABLE StockReceiptInfo
ADD CONSTRAINT FK_StockReceiptInfo_IdStockReceipt FOREIGN KEY(idStockReceipt) REFERENCES StockReceipt(idStockReceipt);

ALTER TABLE StockReceiptInfo
ADD CONSTRAINT FK_StockReceiptInfo_IdGoodsReceipt FOREIGN KEY(idGoods) REFERENCES Goods(idGoods);

ALTER TABLE Employee
ADD CONSTRAINT FK_Employee_IdAccount FOREIGN KEY(idAccount) REFERENCES Account(idAccount);

ALTER TABLE BillService
ADD CONSTRAINT FK_BillService_IdCustomer FOREIGN KEY(idCustomer) REFERENCES Customer(idCustomer);

ALTER TABLE BillService
ADD CONSTRAINT FK_BillService_IdAccount FOREIGN KEY(idAccount) REFERENCES Account(idAccount);

ALTER TABLE BillServiceInfo
ADD CONSTRAINT FK_BillServiceInfo_IdBillService FOREIGN KEY(idBillService) REFERENCES BillService(idBillService);

ALTER TABLE BillServiceInfo
ADD CONSTRAINT FK_BillServiceInfo_IdService FOREIGN KEY(idService) REFERENCES Service(idService);

ALTER TABLE Employee
ADD CONSTRAINT FK_EmployeePosition_IdPosition FOREIGN KEY(idPosition) REFERENCES EmployeePosition(idEmployeePosition); 

ALTER TABLE Customer
ADD CONSTRAINT FK_MembershipsType_IdMembership FOREIGN KEY(idMembership) REFERENCES MembershipsType(idMembershipsType);


ALTER TABLE PositionDetail
ADD CONSTRAINT FK_PositionDetail_idPermission FOREIGN KEY(idPermission) REFERENCES Permission(idPermission);

ALTER TABLE PositionDetail
ADD CONSTRAINT FK_PositionDetail_idEmployeePosition FOREIGN KEY(idEmployeePosition) REFERENCES EmployeePosition(idEmployeePosition);


-- INSERT DATABASE DEFAULT

-- parameter 
INSERT INTO Parameter(idParameter, parameterName, value) VALUES (1,'PrepaymentPercent', '50');
INSERT INTO Parameter(idParameter, parameterName, value) VALUES (2,'StoreName','PNJ');
INSERT INTO Parameter(idParameter, parameterName, value) VALUES (3,'StoreAddress', 'Song Hành, khu phố 6, Thủ Đức, Thành phố Hồ Chí Minh');
INSERT INTO Parameter(idParameter, parameterName, value) VALUES (4,'StorePhoneNumber', '0326089954');
INSERT INTO Parameter(idParameter, parameterName, value) VALUES (5,'StoreEmail', 'gbms@gmail.com');
INSERT INTO Parameter(idParameter, parameterName) VALUES (6,'StoreAvatar');
INSERT INTO Parameter(idParameter, parameterName, value) VALUES (7, 'AuthKey','admin');

-- permission
INSERT INTO Permission (idPermission, permissionName) VALUES (1,'Truy cập trang chủ');
INSERT INTO Permission (idPermission, permissionName) VALUES (2,'Tạo hóa đơn bán hàng');
INSERT INTO Permission (idPermission, permissionName) VALUES (3,'Tạo phiếu dịch vụ');
INSERT INTO Permission (idPermission, permissionName) VALUES (4,'Quản lý tồn kho');
INSERT INTO Permission (idPermission, permissionName) VALUES (5,'Quản lý phiếu nhập kho');
INSERT INTO Permission (idPermission, permissionName) VALUES (6,'Quản lý nhà cung cấp');
INSERT INTO Permission (idPermission, permissionName) VALUES (7,'Quản lý khách hàng');
INSERT INTO Permission (idPermission, permissionName) VALUES (8,'Quản lý nhân viên');
INSERT INTO Permission (idPermission, permissionName) VALUES (9,'Quản lý hàng hóa');
INSERT INTO Permission (idPermission, permissionName) VALUES (10,'Quản lý dịch vụ');
INSERT INTO Permission (idPermission, permissionName) VALUES (11,'Xem phiếu dịch vụ');
INSERT INTO Permission (idPermission, permissionName) VALUES (12,'Xem hóa đơn');
INSERT INTO Permission (idPermission, permissionName) VALUES (13,'Cài đặt phân quyền, tham số');
-- Account
INSERT INTO `account` VALUES (1,'admin','C4CA4238A0B923820DCC509A6F75849B');

-- Membership
INSERT INTO `membershipstype` VALUES (1,'Thường',0);

-- Employee
insert into employee(idEmployee, name, gender, phoneNumber, address, dateOfBirth, idPosition, startingDate, idAccount) values('0', 'Admin','Nam','326089954','HCM','2001-01-01',NULL, '2021-01-01','1');

