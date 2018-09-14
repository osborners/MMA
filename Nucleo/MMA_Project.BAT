SET PATH=D:\Program Files\Keil_v5\ARM\ARMCC\Bin;C:\ProgramData\Oracle\Java\javapath;C:\Python34\;C:\Python34\Scripts;C:\Users\rosbo\AppData\Local\Programs\Python\Python35\Lib\site-packages\PyQt5;C:\Program Files (x86)\Intel\iCLS Client\;C:\Program Files\Intel\iCLS Client\;C:\Windows\system32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0\;C:\Program Files (x86)\Intel\Intel(R) Management Engine Components\DAL;C:\Program Files\Intel\Intel(R) Management Engine Components\DAL;C:\Program Files (x86)\Intel\Intel(R) Management Engine Components\IPT;C:\Program Files\Intel\Intel(R) Management Engine Components\IPT;C:\Program Files (x86)\NVIDIA Corporation\PhysX\Common;C:\Program Files (x86)\Windows Kits\8.1\Windows Performance Toolkit\;C:\WINDOWS\system32;C:\WINDOWS;C:\WINDOWS\System32\Wbem;C:\WINDOWS\System32\WindowsPowerShell\v1.0\;C:\Program Files\Microsoft\Web Platform Installer\;C:\Program Files\MiKTeX 2.9\miktex\bin\x64\;D:\QT\5.7\mingw53_32\bin;D:\QT\Qt5.7.0\Tools\mingw530_32\bin;D:\Matlab\runtime\win64;D:\Matlab\bin;C:\WINDOWS\System32\OpenSSH\;C:\Users\rosbo\AppData\Local\Microsoft\WindowsApps;C:\Users\rosbo\AppData\Local\Microsoft\WindowsApps;C:\Users\rosbo\AppData\Local\GitHubDesktop\bin
SET ARMCC5_ASMOPT=--diag_suppress=9931
SET ARMCC5_CCOPT=--diag_suppress=9931
SET ARMCC5_LINKOPT=--diag_suppress=9931
SET CPU_TYPE=STM32F446RE
SET CPU_VENDOR=STMicroelectronics
SET UV2_TARGET=MMA_Project
SET CPU_CLOCK=0x01F78A40
"D:\Program Files\Keil_v5\ARM\ARMCC\Bin\ArmCC" --Via ".\build\main.__i"
"D:\Program Files\Keil_v5\ARM\ARMCC\Bin\ArmCC" --Via ".\build\stepper.__i"
"D:\Program Files\Keil_v5\ARM\ARMCC\Bin\ArmLink" --Via ".\BUILD\MMA_Project.lnp"
"D:\Program Files\Keil_v5\ARM\ARMCC\Bin\fromelf.exe" ".\BUILD\MMA_Project.axf" --i32combined --output ".\BUILD\MMA_Project.hex"
