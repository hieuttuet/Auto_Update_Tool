# Auto_Update_Tool
tool nhúng vào ứng dụng chính giúp tự động update ứng dụng chính   
Hướng dẫn setup:  

B1: Cài Costura.Fody và Fody để nén các file dll thành 1 file .exe  

B2: Vào Solution Explorer -> Properties -> Build Events -> Copy vào mục "Post-build event command line"  ( loại bỏ file exe.config, *.xml, *.pdb )
del "$(TargetDir)$(TargetName).exe.config" /q  
del "$(TargetDir)*.xml" /s /q /f  
del "$(TargetDir)*.pdb" /q  

B3: Copy file Updater.exe vào dự án rồi nhúng vào ứng dụng:  
Add -> Existing Item -> Chọn file Updater.exe   

B4: Chỉnh  Buil Action = Embedded Resource   

B5: Thêm code xử lý update : thêm file IniService.cs ( config) , file LoginViewModel.cs ( check ver)  
Nhớ thêm đường dẫn update ở file config nhé !!  
