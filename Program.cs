using System.Diagnostics;
using System.Reflection;

Directory.CreateDirectory("Embedded_Circuits");

//extract assemblies
string[] assemblyNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
foreach (string assemblyName in assemblyNames)
{
    if (assemblyName.Contains("Properties")) continue;
    var assembly = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyName);
    string fileName = assemblyName.Replace("Interrupts_8086.Resources.", "");
    var file = new FileStream("Embedded_Circuits\\" + fileName, FileMode.Create, FileAccess.Write);
    assembly.CopyTo(file);
}

Console.Write("Java Version: ");
findJavaVersion();

//Start Main Simulation screen
Process p = new Process();
p.StartInfo.UseShellExecute = false;
p.StartInfo.CreateNoWindow = true;
p.StartInfo.FileName = "java";
string targetDir = Directory.GetCurrentDirectory() + "\\Embedded_Circuits\\";
p.StartInfo.Arguments = "-jar " + targetDir + "Digital.jar " + targetDir + "Main_Interruptverarbeitung_8086.dig";
Console.WriteLine("starting Digital with the following command:\n" +
    p.StartInfo.FileName + " " +
    p.StartInfo.Arguments);
p.Start();
Thread.Sleep(50); //to allow Digital to start before this program terminates..

static void findJavaVersion()
{
    try
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "java";
        psi.Arguments = " -version";
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;

        Process pr = Process.Start(psi);
        string strOutput = pr.StandardError.ReadLine().Split(' ')[2].Replace("\"", "");

        Console.WriteLine(strOutput);
        if (strOutput.Contains("'java' is not recognized as an internal or external command"))
        {
            Console.WriteLine("Java doesn't seem to be installed on this machine.\n" +
                "Please download & install java, then try again.");
            throw new Exception("Java not found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception is " + ex.Message);
    }
}