# Phần mềm quản lý cửa hàng kinh doanh vàng bạc đá quý
## Hướng dẫn sử dụng
-	Download hoặc clone repo về máy tính.
-	Chạy file `SQL/Create_GBMS_DB.sql`
-	Chạy file `GemstonesBusinessManagementSystem/GemstonesBusinessManagementSystem.sln`
-	Tài khoản mặc định: admin
-	Password: 1
-	Mã xác thực: admin
## Công nghệ sử dụng
- Nền tảng: .Net FrameWork, version 4.7.2
- Ngôn ngữ: C#, XAML
- UI Framework: Windows Presentation Foundation (WPF)
- UI design tool: Figma
- Hệ quản trị cơ sở dữ liệu: MySQL
- IDE: Microsoft Visual Studio 2019
- Thư viện hỗ trợ khác: MaterialDesignXAML, ToastNotifications, LiveCharts, System.Windows.Interactivity.WPF

##	Mục đích và yêu cầu
###	Mục đích
-	Giảm thiểu số lượng công việc thủ công, tiết kiệm thời gian công sức. 
-	Nâng cao tính chính xác và bảo mật trong kinh doanh, quản lý thông tin nhân viên.
-	Kiểm soát tốt công việc mua bán.
###	Yêu cầu
####	Yêu cầu chức năng
- Quản lý Cửa hàng:
	- Lập hóa đơn bán hàng.
	- Lập phiếu dịch vụ.
	- In hóa đơn thanh toán.
- Quản lý Kho:
	- Xem tồn kho của từng sản phẩm theo từng tháng.
	-	Tra cứu tồn kho của sản phẩm.
	-	Xuất file excel thống kê tồn kho.
	-	Xem danh sách phiếu nhập hàng và chi tiết của từng phiếu.
	-	Tra cứu phiếu nhập hàng.
	-	Nhập hàng và in phiếu nhập hàng khi thanh toán.
	-	Xuất file excel danh sách các phiếu nhập hàng. 
- 	Quản lý Đối tác:
	-	Khách hàng:
		-	Thêm, sửa thông tin khách hàng.
		-	Tra cứu thông tin khách hàng.
		-	Thêm, xóa, sửa hạng thành viên.
		-	Xuất file excel danh sách thông tin các khách hàng.	
	-	Nhà cung cấp:
		-	Thêm, sửa thông tin nhà cung cấp.
		-	Tra cứu thông tin nhà cung cấp.
		-	Xuất file excel danh sách thông tin nhà cung cấp.
- 	Quản lý Sản phẩm:
	-	Thêm, xóa, sửa thông tin từng sản phẩm sản phẩm.
	-	Thêm, sửa loại sản phẩm.
	-	Ngừng hoạt động và hoạt động trở lại loại sản phẩm.
	-	Tra cứu thông tin sản phẩm.
	-	Xuất file excel danh sách thông tin loại sản phẩm.
- 	Quản lý Dịch vụ:
	-	Thêm, xóa, sửa thông tin của từng dịch vụ.
	-	Tra cứu thông tin dịch vụ.
	-	Khôi phục dữ liệu của dịch vụ. 
	-	Xuất file excel danh sách thông tin từng dịch vụ.
 - 	Quản lý Nhân viên:
	-	Thêm, xóa, sửa thông tin nhân viên.
	-	Thêm, xóa, sửa chức vụ.
	-	Tra cứu nhân viên.
	-	Xuất file excel danh sách thông tin của nhân viên.
- Quản lý Báo cáo:
	-	Xem danh sách hóa đơn bán hàng và chi tiết của từng hóa đơn.
	-	Xem danh sách hóa đơn dịch vụ và chi tiết của từng hóa đơn.
	-	Xem biểu đồ thống kê doanh thu và chi tiêu theo từng mốc thời gian.
- 	Quản lý Tài khoản:
	-	Đăng nhập, đăng ký.
	-	Đổi mật khẩu, đổi thông tin tài khoản.
- Quản lý Thông tin khác:
	-	Quản lý thông tin cửa hàng.
	-	Cài đặt quyền truy cập cho từng chức vụ nhân viên.
## Đối tượng sử dụng
-	Chủ cửa hàng
-	Nhân viên trong cửa hàng
## Thực hiện
- [Phan Ngọc Quang](https://github.com/quangpn0204)
- [Huỳnh Quang Trung](https://github.com/trunghuynh2304)
- [Đỗ Văn Bảo](https://github.com/baodv1001)
- [Nguyễn Viết Lưu](https://github.com/Luunguyen2412)