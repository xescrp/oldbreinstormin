using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DotNet.Tools.WIN32
{
    public class Win32API
    {
        [DllImport("ntdll.dll")]
        public static extern int NtQueryObject(IntPtr ObjectHandle, int
            ObjectInformationClass, IntPtr ObjectInformation, int ObjectInformationLength,
            ref int returnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

        [DllImport("ntdll.dll")]
        public static extern uint NtQuerySystemInformation(int
            SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength,
            ref int returnLength);

        [DllImport("kernel32.dll", EntryPoint = "RtlCopyMemory")]
        public static extern void CopyMemory(byte[] Destination, IntPtr Source, uint Length);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
        
        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
           ushort hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle,
           uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(
        IntPtr hToken,
        int impersonationLevel,
        ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();



        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
          string principal,
          string authority,
          string password,
          LogonTypes logonType,
          LogonProviders logonProvider,
          out IntPtr token);


        public enum LogonTypes : uint
        {
            Interactive = 2,
            Network,
            Batch,
            Service,
            NetworkCleartext = 8,
            NewCredentials
        }
        public enum LogonProviders : uint
        {
            Default = 0, // default por plataforma (usar este!)
            WinNT35,     // envia señales de humo a la autoridad
            WinNT40,     // usa NTLM
            WinNT50      // negocia Kerb o NTLM
        }


        public enum ObjectInformationClass : int
        {
            ObjectBasicInformation = 0,
            ObjectNameInformation = 1,
            ObjectTypeInformation = 2,
            ObjectAllTypesInformation = 3,
            ObjectHandleInformation = 4
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_BASIC_INFORMATION
        { // Information Class 0
            public int Attributes;
            public int GrantedAccess;
            public int HandleCount;
            public int PointerCount;
            public int PagedPoolUsage;
            public int NonPagedPoolUsage;
            public int Reserved1;
            public int Reserved2;
            public int Reserved3;
            public int NameInformationLength;
            public int TypeInformationLength;
            public int SecurityDescriptorLength;
            public System.Runtime.InteropServices.ComTypes.FILETIME CreateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_TYPE_INFORMATION
        { // Information Class 2
            public UNICODE_STRING Name;
            public int ObjectCount;
            public int HandleCount;
            public int Reserved1;
            public int Reserved2;
            public int Reserved3;
            public int Reserved4;
            public int PeakObjectCount;
            public int PeakHandleCount;
            public int Reserved5;
            public int Reserved6;
            public int Reserved7;
            public int Reserved8;
            public int InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public int ValidAccess;
            public byte Unknown;
            public byte MaintainHandleDatabase;
            public int PoolType;
            public int PagedPoolUsage;
            public int NonPagedPoolUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_NAME_INFORMATION
        { // Information Class 1
            public UNICODE_STRING Name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GENERIC_MAPPING
        {
            public int GenericRead;
            public int GenericWrite;
            public int GenericExecute;
            public int GenericAll;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SYSTEM_HANDLE_INFORMATION
        { // Information Class 16
            public int ProcessID;
            public byte ObjectTypeNumber;
            public byte Flags; // 0x01 = PROTECT_FROM_CLOSE, 0x02 = INHERIT
            public ushort Handle;
            public int Object_Pointer;
            public UInt32 GrantedAccess;
        }

        public const int MAX_PATH = 260;
        public const uint STATUS_INFO_LENGTH_MISMATCH = 0xC0000004;
        public const int DUPLICATE_SAME_ACCESS = 0x2;


        public enum ProcessReturnCode: uint
        {
            SuccessfulCompletion = 0,
            AccessDenied = 2,
            InsufficientPrivilege = 3,
            UnknownFailure = 8,
            PathNotFound = 9,
            InvalidParameter = 21
        }




        // ****************** SERVICIOS ************************* //


        public enum ServiceStartMode
        {
            Automatic,
            Boot,
            System,
            Manual,
            Disabled,
        }

        /// <summary>
        /// El codigo de retorno de la clase WMI Win32_Service
        /// </summary>
        public enum ServiceReturnCode
        {
            Success = 0,
            NotSupported = 1,
            AccessDenied = 2,
            DependentServicesRunning = 3,
            InvalidServiceControl = 4,
            ServiceCannotAcceptControl = 5,
            ServiceNotActive = 6,
            ServiceRequestTimeout = 7,
            UnknownFailure = 8,
            PathNotFound = 9,
            ServiceAlreadyRunning = 10,
            ServiceDatabaseLocked = 11,
            ServiceDependencyDeleted = 12,
            ServiceDependencyFailure = 13,
            ServiceDisabled = 14,
            ServiceLogonFailure = 15,
            ServiceMarkedForDeletion = 16,
            ServiceNoThread = 17,
            StatusCircularDependency = 18,
            StatusDuplicateName = 19,
            StatusInvalidName = 20,
            StatusInvalidParameter = 21,
            StatusInvalidServiceAccount = 22,
            StatusServiceExists = 23,
            ServiceAlreadyPaused = 24
        }

        /// <summary>
        /// El tipo de serivicio del que se trata, normalmente OwnProcess
        /// </summary>
        public enum ServiceType
        {
            KernalDriver = 1,
            FileSystemDriver = 2,
            Adapter = 4,
            RecognizerDriver = 8,
            OwnProcess = 16,
            ShareProcess = 32,
            InteractiveProcess = 256,
        }

        internal enum ServiceErrorControl
        {
            UserNotNotified = 0,
            UserNotified = 1,
            SystemRestartedWithLastKnownGoodConfiguration = 2,
            SystemAttemptsToStartWithAGoodConfiguration = 3
        }
    
    //*********************** FILES **********************//
        public enum FileReturnCode
        {
            Success = 0, 
            Accessdenied = 2,
            Unspecifiedfailure = 8,
            Invalidobject = 9,
            Objectalreadyexists = 10,
            FilesystemnotNTFS = 11,
            PlatformnotWindowsNT_based = 12, 
            Drivenotthesame = 13, 
            Directorynotempty = 14, 
            Sharingviolation = 15, 
            Invalidstartfile = 16, 
            Privilegenotheld = 17, 
            Invalidparameter = 21
        }
 
        public enum ConfigManagerErrorCode:uint 
        {
            Device_is_working_properly = 0x0,
            Device_is_not_configured_correctly = 0x1,
            Windows_cannot_load_the_driver_for_this_device = 0x2,
            Driver_for_this_device_might_be_corrupted_or_the_system_may_be_low_on_memory_or_other_resources = 0x3,
            Device_is_not_working_properly_One_of_its_drivers_or_the_registry_might_be_corrupted = 0x4,
            Driver_for_the_device_requires_a_resource_that_Windows_cannot_manage = 0x5,
            Boot_configuration_for_the_device_conflicts_with_other_devices = 0x6,
            Cannot_filter = 0x7,
            Driver_loader_for_the_device_is_missing = 0x8,
            Device_is_not_working_properly_The_controlling_firmware_is_incorrectly_reporting_the_resources_for_the_device = 0x9,
            Device_cannot_start = 0xA,
            Device_failed = 0xB,
            Device_cannot_find_enough_free_resources_to_use = 0xC,
            Windows_cannot_verify_the_devices_resources = 0xD,
            Device_cannot_work_properly_until_the_computer_is_restarted = 0xE, 
            Device_is_not_working_properly_due_to_a_possible_reenumeration_problem = 0xF, 
            Windows_cannot_identify_all_of_the_resources_that_the_device_uses = 0x10,
            Device_is_requesting_an_unknown_resource_type = 0x11,
            Device_drivers_must_be_reinstalled = 0x12,
            Failure_using_the_VxD_loader = 0x13,
            Registry_might_be_corrupted = 0x14,
            System_failure_If_changing_the_device_driver_is_ineffective_see_the_hardware_documentation_Windows_is_removing_the_device = 0x15,
            Device_is_disabled = 0x16,
            System_failure_If_changing_the_device_driver_is_ineffective_see_the_hardware_documentation = 0x17,
            Device_is_not_present,_not_working_properly_or_does_not_have_all_of_its_drivers_installed = 0x18,
            Windows_is_still_setting_up_the_device = 0x19, 
            Windows_is_still_setting_up_the_device_v = 0x1A,
            Device_does_not_have_valid_log_configuration = 0x1B,
            Device_drivers_are_not_installed = 0x1C,
            Device_is_disabled_The_device_firmware_did_not_provide_the_required_resources = 0x1D,
            Device_is_using_an_IRQ_resource_that_another_device_is_using = 0x1E,
            Device_is_not_working_properly_Windows_cannot_load_the_required_device_drivers = 0x1F
        }
 

    
    
    }













}
