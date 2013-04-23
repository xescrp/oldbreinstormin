using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DotNet.Tools.IO
{
    public class CopyEngine
    {
        public static void CopyDirectory(string sourcedirectory, string targetdirectory, string logname, string[] extensionesexcluidas, bool iferrorfinish)
        {
            DirectoryInfo source = new DirectoryInfo(sourcedirectory);
            DirectoryInfo target = new DirectoryInfo(targetdirectory);
            System.Collections.ArrayList extensiones = new System.Collections.ArrayList();
            if (extensionesexcluidas != null)
            {
                extensiones.AddRange(extensionesexcluidas);
            }
            _CopyAll(source, target, logname, extensiones,iferrorfinish);
        }

        private static void _CopyAll(DirectoryInfo source, DirectoryInfo target, string logname, System.Collections.ArrayList extensionesexcluidas, bool iferrorfinish)
        {
            //Si el destino no existe lo creamos
            if (!target.Exists) 
            {
                target.Create();
            }
            //Obtenemos archivos del directorio origen
            FileInfo[] files = source.GetFiles();
            //Recorremos la lista de archivos
            foreach (FileInfo file in files) 
            {
                //Comprobamos la extension si esta excluida de copia
                if (extensionesexcluidas != null && extensionesexcluidas.Contains(file.Extension.ToLower()))
                {
                    Log.LogEngine.WriteLog(logname, "[OK] - OJO! NO SE COPIA FICHERO POR EXTENSION EXCLUIDA (" + file.Extension.ToLower() + "): " + file.FullName);
                }
                else 
                {
                    //Modificamos los atributos de solo lectura de los archivos destino si existen
                    try
                    {
                        if (File.Exists(target.FullName + @"\" + file.Name)) { File.SetAttributes(target.FullName + @"\" + file.Name, FileAttributes.Normal); }
                        file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                        Log.LogEngine.WriteLog(logname, "[OK] - ORIGEN: " + file.FullName + " - DESTINO: " + target.FullName + @"\" + file.Name);
                    }
                    catch (Exception ex)
                    {
                        Log.LogEngine.WriteLog(logname, "[ERROR] - ORIGEN: " + file.FullName + " - DESTINO: " + target.FullName + @"\" + file.Name + "\r\n Error copiando archivo " + ex.Message + ex.StackTrace);
                        if (iferrorfinish) { throw ex; }
                    }
                }
            }
            //Recorremos los subdirectorios del directorio origen y los creamos si es necesario en el directorio destino
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs) {
                DirectoryInfo targetsubdir = target.CreateSubdirectory(dir.Name);
                try
                {
                    _CopyAll(dir, targetsubdir, logname, extensionesexcluidas, iferrorfinish);
                }
                catch (Exception ex) 
                {
                    Log.LogEngine.WriteLog(logname, "[ERROR] - ORIGEN: " + dir.FullName + " - DESTINO: " + targetsubdir.FullName + "\r\n Error copiando directorio " + ex.Message + ex.StackTrace);
                    if (iferrorfinish) { throw ex; }
                }
            }
        }
    }
}
