using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Tools.Tests
{
    class Testing
    {
        static void Test1() { 
            IO.CopyEngine.CopyDirectory(@"C:\Temp",@"c:\tmp","pruebalog",null, false);
        }

        private void Test2() 
        { 
            System.Diagnostics.Process[] prs = System.Diagnostics.Process.GetProcessesByName("chrome");
            List<WIN32.Win32API.SYSTEM_HANDLE_INFORMATION> handles = WIN32.ProcessApi.GetHandles(prs[0]);
            if (handles != null) 
            {
                foreach (WIN32.Win32API.SYSTEM_HANDLE_INFORMATION handle in handles) 
                {
                    Console.WriteLine("HANDLE: " + handle.Handle.ToString() + " - PROCESSID: " + handle.ProcessID);
                }
            }
        }



        private void Test3()
        {
            System.Diagnostics.Process[] prs = System.Diagnostics.Process.GetProcessesByName("devenv");
            List<WIN32.Win32API.SYSTEM_HANDLE_INFORMATION> handles = WIN32.ProcessApi.GetFileHandles(prs[0]);
            if (handles != null)
            {
                foreach (WIN32.Win32API.SYSTEM_HANDLE_INFORMATION handle in handles)
                {
                    Console.WriteLine("HANDLE: " + handle.Handle.ToString() + " - PROCESSID: " + handle.ProcessID);
                }
            }
        }
        
        [MTAThread()]
        private void Test4()
        {

            System.IO.StreamReader str = new System.IO.StreamReader(@"C:\globdata.ini");
            str.Read();

            System.Diagnostics.Process[] prs = System.Diagnostics.Process.GetProcessesByName("w3wp", "svigi-11");
            List<WIN32.FileApi> handles = WIN32.ProcessApi.GetFiles(prs[0]);
            if (handles != null)
            {
                foreach (WIN32.FileApi handle in handles)
                {
                    Console.WriteLine("FILE: " + handle.Name + " (HANDLE: " + handle.Handle.Handle.ToString() + 
                        " - PROCESSID: " + handle.Handle.ProcessID + ")"  );
                }
            }

            WIN32.Win32API.SYSTEM_HANDLE_INFORMATION[] hands = WIN32.ProcessApi.FindHandles("negPromociones");

            Console.WriteLine("Buscamos: negPromociones");
            foreach (WIN32.Win32API.SYSTEM_HANDLE_INFORMATION hwnd in hands) 
            {
                Console.WriteLine("Encontrado: " + hwnd.Handle.ToString() + " (Proceso: " + hwnd.ProcessID.ToString() + ")");
            }

            str.Close();
        }

        private void test5() 
        {
            
            string dir = @"c:\temp\temp2\io.txt";
            Console.WriteLine(System.IO.Path.GetDirectoryName(dir));
        }

        private void test6()
        {

            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
            Console.WriteLine(Random.RandomEngine.GetRandomNumber(0, 10));
        
        }

        private void Test7() 
        {
            WIN32.WmiAccess w_acc = new DotNet.Tools.WIN32.WmiAccess();
            WIN32.Win32_Service w_serv = new DotNet.Tools.WIN32.Win32_Service(w_acc);
            w_serv.Install("SVAPL02-36", "DeployServer 2.0", "Deploy Server 2.0", 
                    @"D:\Services .NET\dotNET\DeployServerSVCHost\", DotNet.Tools.WIN32.Win32API.ServiceStartMode.Disabled,
                    @"UI\DI_ROSSELLO2","KUKUXUMUXU007",null);
            
        }

        private void Test8() 
        {
            WIN32.Win32_Service w_serv = new DotNet.Tools.WIN32.Win32_Service("svapl02-36", "DeployServer 2.0", null, null);
            Console.WriteLine(w_serv.AcceptPause.ToString());
            Console.WriteLine(w_serv.Name);
            Console.WriteLine(w_serv.Status);
            Console.WriteLine(w_serv.SystemName);
            
        }

        private void Test9() 
        {
            System.Management.ManagementObjectCollection mg = 
                WIN32.WmiAccess.GetInstancesOfClass("svapl02-36", "Win32_Process");
            foreach (System.Management.ManagementObject m in mg) 
            {
                Console.WriteLine(m.Properties["Name"].Value);

            }

            
        }
        private void Test10() 
        {
            System.Management.ManagementObjectCollection mg =
                WIN32.WmiAccess.GetInstancesOfClass("svapl02-36", "Win32_Process");
            foreach (System.Management.ManagementObject m in mg)
            {
                
                Console.WriteLine(m.Properties["Name"].Value);
                System.Management.ManagementObjectCollection mg2 = m.GetRelationships();
                foreach (System.Management.ManagementObject mm in mg2) 
                {
                    Console.WriteLine(mm.ClassPath);
                    System.Management.ManagementObjectCollection mg3 = mm.GetRelated();
                    foreach (System.Management.ManagementObject mm2 in mg2) 
                    {
                        Console.WriteLine(mm2.ClassPath);
                    }
                }

                WIN32.Win32_Process prc = new DotNet.Tools.WIN32.Win32_Process(m);
                string user; string domain;
                WIN32.Win32API.ProcessReturnCode rs = prc.GetOwner(out user,out domain);
                Console.WriteLine(rs.ToString());
                Console.WriteLine(user);
                Console.WriteLine(domain);

            }
        }




        private void Test11() 
        {
            WIN32.Win32_Service[] srvs = WIN32.Win32_Service.GetWin32_Services("svapl02-36");
            WIN32.Win32_Process[] prcs = WIN32.Win32_Process.GetWin32_Processes("svapl02-36");
            Console.WriteLine("****SERVICES****");
            foreach (WIN32.Win32_Service srv in srvs) 
            {
                Console.WriteLine(srv.Name);
                Console.WriteLine("i:" + srv.InterrogateService().ToString());
            }
            Console.WriteLine("****PROCESSES****");
            foreach (WIN32.Win32_Process prc in prcs) 
            {
                Console.WriteLine(prc.Name);
                //prc.getFiles();
                if (prc.Name.ToLower() == "w3wp.exe")
                {
                    Console.WriteLine("Command: " + prc.CommandLine);
                    WIN32.CIM_DataFile[] files = prc.GetOpenedCIM_DataFiles();
                    foreach (WIN32.CIM_DataFile file in files)
                    {

                        
                            Console.WriteLine(file.FileName);
                        
                    }
                }
            }
        }

        private void Test12() 
        {
            //WIN32.Win32_Process[] prc = WIN32.Win32_Process.GetProcessesWithOpenedFile("svapl02-36",
            //        @"d:\Services .NET\dotNET\DeployServerSVCHost\DotNet.Tools.dll");
            WIN32.Win32_Process[] prc = WIN32.Win32_Process.GetProcessesWithOpenedFile("localhost",
                   @"C:\Program Files\TestDriven.NET 2.0\TestDriven.TestRunner.Server.dll");
            foreach (WIN32.Win32_Process pr in prc) 
            {
                Console.WriteLine(pr.Name);
            }
        }

        private void Test19() 
        {
            WIN32.Win32_Process[] prc = WIN32.Win32_Process.GetWin32_Processes("localhost");
            Console.WriteLine(prc.Length.ToString());
            foreach (WIN32.Win32_Process proc in prc) 
            {
                if (proc.Name.ToLower().Contains("chrome")) { proc.Terminate(); }
            }
            
        }

        private void Test13() 
        {
            string machineName = "svapl02-36";
            string name = @"d:\\Web\\Facilities.Instalaciones.BS.WS\\web.config";
            
            System.Management.ManagementScope scope = WIN32.WmiAccess.Connect(machineName);
            string ext = System.IO.Path.GetExtension(name);
            if (ext.StartsWith(".")) { ext = ext.Replace(".", ""); }
            string fname = System.IO.Path.GetFileNameWithoutExtension(name);
            System.IO.FileInfo f = new System.IO.FileInfo(name);
            string dir = f.DirectoryName;
            dir = dir.Replace(@"\", @"\\");
            dir = dir.ToLower().Replace("d:", "").Replace("c:", "").Replace("e:", "").Replace("f:", "");
            System.Management.ObjectQuery query = new System.Management.ObjectQuery("SELECT * FROM CIM_DataFile WHERE Name = '" + name + "'");
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(scope, query);
            System.Management.ManagementObjectCollection results = searcher.Get();

            foreach (System.Management.ManagementObject mo in results) 
            {
                WIN32.CIM_DataFile cm = new DotNet.Tools.WIN32.CIM_DataFile(mo);
                System.Management.ManagementObjectCollection m_R = mo.GetRelated("Win32_Process");
                foreach (System.Management.ManagementObject mmm in m_R) 
                {
                    Console.WriteLine(mmm.ClassPath);
                }
                
            }
            
        }

        private void Test14() 
        {
            string machineName = "svapl02-36";
            string name = @"d:\\Web\\Facilities.Instalaciones.BS.WS\\web.config";

            WIN32.Win32_Process proc =new DotNet.Tools.WIN32.Win32_Process(WIN32.WmiAccess.GetInstanceByName("svapl02-36", "Win32_Process", "Infomallorca.Facilities.Instalaciones.SVC.exe"));

            System.Management.ManagementObjectCollection results = WIN32.WmiAccess.GetAssociatorsByClassName(machineName, "Win32_Process=" + proc.ProcessId.ToString() + "", "CIM_DataFile");
            foreach (System.Management.ManagementObject mo in results) 
            {
                Console.WriteLine(mo.ClassPath);
            }
        }

        private void Test15() 
        {
            WIN32.Win32_LogicalDisk[] ldisks = WIN32.Win32_LogicalDisk.GetWin32_LogicalDisks("svweb03-03");
            foreach (WIN32.Win32_LogicalDisk disk in ldisks) 
            {
                Console.WriteLine(disk.Name + " - " + disk.Description);
            }
        }

        private void Test16() 
        {
            WIN32.Win32_Process.StartProcess("localhost", "cmd /c IISReset");
            WIN32.Win32_Process.StartProcess("localhost", "notepad.exe");
        }

        private void NTAccess() 
        {
            IntPtr token = new IntPtr();
            IntPtr dupToken = new IntPtr();
            //if (WIN32.Win32API.LogonUser("BATCHWEB", "SVIGI-11", "HEXTRADIFERENTE",
            if (WIN32.Win32API.LogonUser("DOTNETADMIN", "SVWEB02-07", "P@ELL@59",
                DotNet.Tools.WIN32.Win32API.LogonTypes.NewCredentials,
                DotNet.Tools.WIN32.Win32API.LogonProviders.Default, out token)) 
            {
                int ok = WIN32.Win32API.DuplicateToken(token, 2, ref dupToken);
            }
            System.Security.Principal.WindowsIdentity id = new System.Security.Principal.WindowsIdentity(dupToken);

            System.Security.Principal.WindowsImpersonationContext impersonatedUser = id.Impersonate();

            impersonatedUser.ToString();

            //System.Security.Principal.WindowsPrincipal p = new System.Security.Principal.WindowsPrincipal(id);
        }

        private void MAPDrive() 
        {
            WIN32.Win32_MapNetworkDrive dr = new DotNet.Tools.WIN32.Win32_MapNetworkDrive();
            WIN32.Win32_MapNetworkDrive dr2 = new DotNet.Tools.WIN32.Win32_MapNetworkDrive();
            dr.LocalDrive = "z:";
            dr2.LocalDrive = "x:"; 
            dr.ShareName = @"\\svweb02-07\c$";
            dr2.ShareName = @"\\svapl02-36\d$\Webs";
            dr.MapDrive("dotnetadmin", "P@ELL@59");
            dr2.MapDrive("BATCHWEB", "HEXTRADIFERENTE");
            dr.UnMapDrive();
            dr2.UnMapDrive();
        }

        private void LOGTEST() 
        {
            Console.WriteLine(Log.LogEngine.GetLogDirectory());
            Log.LogEngine.WriteLog(@"TEMP\test", "prueba de log en directorio");
            Console.WriteLine("Prueba de log fin");
            System.Threading.Thread.CurrentThread.Join(5000);
        }

        private void test20() 
        {
            UI.Service_UIInstaller st = new DotNet.Tools.UI.Service_UIInstaller();
            st.ShowDialog();
        }

    }


}
