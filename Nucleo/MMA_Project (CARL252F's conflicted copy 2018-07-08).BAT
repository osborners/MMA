SET PATH=C:\Keil_v5\ARM\ARMCC\Bin;C:\Program Files (x86)\NVIDIA Corporation\PhysX\Common;C:\ProgramData\Oracle\Java\javapath;C:\Program Files (x86)\Parallels\Parallels Tools\Applications;C:\WINDOWS\system32;C:\WINDOWS;C:\WINDOWS\System32\Wbem;C:\WINDOWS\System32\WindowsPowerShell\v1.0\;C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\110\Tools\Binn\;C:\Program Files (x86)\Microsoft SQL Server\120\Tools\Binn\;C:\Program Files\Microsoft SQL Server\120\Tools\Binn\;C:\Program Files\Microsoft SQL Server\120\DTS\Binn\;C:\Program Files (x86)\Windows Kits\8.1\Windows Performance Toolkit\;C:\Program Files\dotnet\;C:\WINDOWS\System32\OpenSSH\;C:\Users\carl\AppData\Local\Microsoft\WindowsApps
SET CPU_TYPE=STM32F446RE
SET CPU_VENDOR=STMicroelectronics
SET UV2_TARGET=MMA_Project
SET CPU_CLOCK=0x01F78A40
"C:\Keil_v5\ARM\ARMCC\Bin\ArmCC" --Via ".\build\main.__i"
"C:\Keil_v5\ARM\ARMCC\Bin\ArmCC" --Via ".\build\stepper.__i"
"C:\Keil_v5\ARM\ARMCC\Bin\ArmCC" --Via ".\build\uart.__i"
"C:\Keil_v5\ARM\ARMCC\Bin\ArmLink" --Via ".\BUILD\MMA_Project.lnp"
