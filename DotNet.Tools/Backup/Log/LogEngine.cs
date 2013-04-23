using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using System.IO;

namespace DotNet.Tools.Log
{
    class _Log
    {
        private ArrayList _Messages = new ArrayList();
        private string _LogName;
        private bool _MessagesToFlush = false;
        private bool _Flushing = false;
        private int _LogWaitSeconds;
        private int _MaxLogQueueLength;
        private bool _WritersWaiting = false;

        public _Log(string pLogName)
        {
            this._LogName = pLogName;
            // No podemos guardar el nombre del fichero porque cambia de un día a otro 
            this._LogWaitSeconds = this._GetLogWaitSeconds();
            this._MaxLogQueueLength = this._GetMaxQueueLength();
        }

        public void Put(string pMessage)
        {
            // Si hay demasiados mensajes en cola ignoramos los nuevos para no provocar un Out Of Memory 
            if ((this._MaxLogQueueLength == -1) || (this._Messages.Count <= this._MaxLogQueueLength))
            {
                _WritersWaiting = true;
                // Bloqueamos la cola de mensajes frente a otros puts o flushes 
                lock (this)
                {
                    this._Messages.Add(pMessage);
                }
                this._MessagesToFlush = true;
                // Si las listas estaban vacías activamos el hilo de flush 
                if (!this._Flushing)
                {
                    this._Flushing = true;
                    //Console.WriteLine("Activa flush " & Me._LogName) 
                    System.Threading.ThreadStart entry = new System.Threading.ThreadStart(this.DoFlush);
                    System.Threading.Thread thread1 = new System.Threading.Thread(entry);
                    thread1.Start();
                }
            }
            //Else 
            // Console.WriteLine("Para") 
            // Stop 
        }

        // Método privado. Se arranca automáticamente cuando entra la primera escritura en algún log 
        public void DoFlush()
        {
            System.Threading.Thread.Sleep(this._LogWaitSeconds * 1000);
            do
            {
                //Console.WriteLine("Empieza flush " & Me._LogName) 
                this._MessagesToFlush = false;
                this.Flush();
                //Console.WriteLine("Acaba flush " & Me._LogName) 
                System.Threading.Thread.Sleep(this._LogWaitSeconds * 1000);
                if (!this._MessagesToFlush)
                {
                    this._Flushing = false;
                    // Volvemos a comprobar que no han entrado mensajes por escribir antes de salir del flush 
                    if (!this._MessagesToFlush)
                    {
                        break; // TODO: might not be correct. Was : Exit Do 
                    }
                    else
                    {
                        this._Flushing = true;
                    }
                }
            }
            //Console.WriteLine("Sale del flush " & Me._LogName) 
            while (true);
        }

        public void Flush()
        {
            int contador = 0;
            _WritersWaiting = false;
            if (this._Messages.Count > 0)
            {
                // Bloqueamos la cola de mensajes frente a otros puts o flushes 
                lock (this)
                {
                    // Volvemos a comprobar que no está vacía por si acaso otro hilo lo ha vaciado mientras esperaba en el lock 
                    if (this._Messages.Count > 0)
                    {
                        string msg = "";
                        StreamWriter writer = default(StreamWriter);
                        try
                        {
                            writer = new StreamWriter(_GetLogFilename(), true);
                            //Console.WriteLine("Vacia " & _Messages.Count & " mensajes " & Me._LogName) 
                            //Console.Out.Flush() 

                            foreach (string mensaje in this._Messages)
                            {
                                writer.WriteLine(mensaje);
                            }
                            //While Me._Messages.Count > 0 
                            // writer.WriteLine(CType(Me._Messages(0), String)) 
                            // 'Me._Messages.RemoveAt(0) 
                            // ' Esto da prioridad a los writers. 
                            // ' A partir de 1000 escrituras deja paso a los writers si hay alguno en espera. 
                            // 'contador += 1 
                            // 'If contador > 1000 AndAlso _WritersWaiting Then 
                            // ' Exit While 
                            // 'End If 
                            //End While 
                            this._Messages = new ArrayList();
                        }
                        catch (Exception ex)
                        {
                            // Ignora cualquier error 
                            Console.WriteLine("Error en Flush " + this._LogName + ". La exc es: " + ex.Message + "\r\n" + ex.StackTrace);
                        }
                        finally
                        {
                            if ((writer != null))
                            {
                                try
                                {
                                    writer.Close();
                                }
                                catch (Exception ex)
                                {
                                    // Ignora cualquier error 
                                    Console.WriteLine("Error en Flush en el close " + this._LogName + ". La exc es: " + ex.Message + "\r\n" + ex.StackTrace);
                                }
                            }
                        }
                    }
                }
            }
        }

        // LogWaitSeconds -> Número de segundos a esperar entre vaciados del buffer a disco 
        // Si el valor es muy bajo se ralentizará el sistema por los accesos a disco que serán más frecuentes 
        // Si el valor es muy alto hay más riesgo de que se pierdan mensajes si se cae la aplicación 
        // y se tarda más en ver los logs escritos. También la aplicación tarda más en pararse. 
        // Se permite indicar una configuración especifica para cada log con la clave "<LogName>.LogWaitSeconds" 
        // Si no existe se usa la genérica "LogMgr.LogWaitSeconds" 
        // Si tampoco existe se usa 3 
        private int _GetLogWaitSeconds()
        {
            string logConfig = null;
            try
            {
                logConfig = ConfigurationSettings.AppSettings[this._LogName + ".LogWaitSeconds"];
                if (logConfig == null)
                {
                    logConfig = ConfigurationSettings.AppSettings["LogMgr.LogWaitSeconds"];
                }
                if ((logConfig != null))
                {
                    return Convert.ToInt16(logConfig);
                }
                else
                {
                    return 3;
                }
            }
            catch
            {
                return 3;
            }
        }

        // MaxLogQueueLength -> Número máximo de mensajes a encolar sin vaciar a disco. 
        // Si se supera ese valor descarta los mensajes. 
        // Pretende ser una protección frente a un bloqueo en las escrituras a disco para evitar que la aplicación falle for falta de memoria 
        // Si vale -1 no se aplica ningún límite y todos los mensajes se encolan para su escritura 
        // Se permite indicar una configuración especifica para cada log con la clave "<LogName>.MaxLogQueueLength" 
        // Si no existe se usa la genérica "LogMgr.MaxLogQueueLength" 
        // Si tampoco existe se usa 50000 
        private int _GetMaxQueueLength()
        {
            string logConfig = null;
            try
            {
                logConfig = ConfigurationSettings.AppSettings[this._LogName + ".MaxLogQueueLength"];
                if (logConfig == null)
                {
                    logConfig = ConfigurationSettings.AppSettings["LogMgr.MaxLogQueueLength"];
                }
                if ((logConfig != null))
                {
                    return Convert.ToInt16(logConfig);
                }
                else
                {
                    return 100000;
                }
            }
            catch
            {
                return 100000;
            }
        }

        internal string _GetLogFilename()
        {
            string filename = null;
            string foldername = null;
            int pos = 0;
            string assemblyName = null;
            System.Reflection.Assembly ejecutable = default(System.Reflection.Assembly);
            ejecutable = System.Reflection.Assembly.GetEntryAssembly();
            // Si no existe GetEntryAssembly asumimos que es una aplicacion ASP.NET 
            if (ejecutable == null)
            {
                filename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                filename = filename.Replace("/", "\\");
            }
            else
            {
                filename = ejecutable.Location;
            }
            pos = filename.LastIndexOf("\\");
            foldername = filename.Substring(0, pos) + "\\Log";
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            filename = string.Format("{0}\\{1}_{2:yyyyMMdd}.log", foldername, this._LogName, DateTime.Today);

            string dir = System.IO.Path.GetDirectoryName(filename);
            if (!System.IO.Directory.Exists(dir)) { System.IO.Directory.CreateDirectory(dir); }

            return filename;
        }

    } 

    class _LogCollection
    {
        private Hashtable _List = new Hashtable();

        public void Put(string pLogName, string pMessage)
        {
            object obj = this._List[pLogName];
            _Log log = default(_Log);
            if (obj == null)
            {
                // Bloqueamos la hashtable de logs por si otro thread está insertando logs al mismo tiempo 
                lock (this)
                {
                    // Volvemos a comprobar que mientras esperabamos en lo lock no ha creado el log otro hilo 
                    obj = this._List[pLogName];
                    if (obj == null)
                    {
                        log = new _Log(pLogName);
                        this._List[pLogName] = log;
                    }
                    else
                    {
                        log = (_Log)obj;
                    }
                }
            }
            else
            {
                log = (_Log)obj;
            }
            log.Put(pMessage);
        }

        public int GetLogLevel(string pLogName)
        {
            string logConfig = null;
            // JCA 30/12/04 Se permite indicar una configuración especifica para cada log con la clave "<LogName>.LogLevel" 
            // Si no existe se usa la genérica "LogLevel" 
            // Si tampoco existe se usa -1 
            try
            {
                logConfig = ConfigurationSettings.AppSettings[pLogName + ".LogLevel"];
                if (logConfig == null)
                {
                    logConfig = ConfigurationSettings.AppSettings["LogLevel"];
                }
                if ((logConfig != null))
                {
                    return Convert.ToInt16(logConfig);
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
        }
        

        public static string GetLogFileName(string pLogName)
        {
            return pLogName + ".log";
        }

        public static string GetLogContent(string pLogName)
        {
            string filename = pLogName + ".log";

            //verifico que exista el archivo 
            if ((System.IO.File.Exists(filename)))
            {
                System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs);

                //Leo toda la informacion del archivo 
                string sContent = sr.ReadToEnd();

                //cierro los objetos 
                fs.Close();
                sr.Close();
                return sContent;
            }
            else
            {
                return "";
            }
        }

        public static bool IsSingleFile()
        {
            string singlefilename = GetSingleFileName();
            return ((singlefilename != null)) && (singlefilename.Length > 0);
        }

        public static string GetSingleFileName()
        {
            return ConfigurationSettings.AppSettings["LogMgr.SingleFileName"];
        }
    } 


    public class LogEngine
    {
        private static _LogCollection _listaLogs = new _LogCollection();
        public static string GetLogDirectory() 
        {
            try
            {
                _Log log = new _Log("dd_name");
                string name = log._GetLogFilename();
                return name.Replace(@"\" + System.IO.Path.GetFileName(name), "");
            }
            catch (Exception ex) 
            {
                return null;
            }
        }

        public static void WriteLog(string pLogName, string pText)
        {

            string logName = string.Empty;
            if ((pLogName != null))
            {
                logName = pLogName;
            }
            string logFichero = null;
            if (_LogCollection.IsSingleFile())
            {
                logFichero = _LogCollection.GetSingleFileName();
            }
            else
            {
                logFichero = logName;
            }

            // -- 

                string fmt = null;
                if (_LogCollection.IsSingleFile())
                {
                    if (pText == null)
                    {
                        fmt = string.Format("{0:s}##", DateTime.Now);
                    }
                    else
                    {
                        fmt = string.Format("{0:s}#{1}#{2}", DateTime.Now, logName, pText);
                    }
                }
                else
                {
                    if (pText == null)
                    {
                        fmt = string.Format("{0:s}", DateTime.Now);
                    }
                    else
                    {
                        fmt = string.Format("{0:s} {1}", DateTime.Now, pText);
                    }
                }
                _listaLogs.Put(logName, pText);
                //_listaLogs.Put(pLogName = logFichero, pMessage = string.Format(fmt, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
            
        } 
    }
}
