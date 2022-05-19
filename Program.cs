using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
[DllImport("User32.dll", CharSet = CharSet.Unicode)]
static extern int MessageBox(IntPtr h, string m, string c, int type);

findJavaVersion();

Directory.CreateDirectory("Embedded_Circuits");

//extract assemblies
string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
foreach (string resourceName in resourceNames)
{
    if (resourceName.Contains("Properties")) continue;

    //has to be using! Will not work on some seemingly random files without using!
    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
    {
        string fileName = resourceName.Replace("Interrupts_8086.Resources.", "");
        using (var file = new FileStream("Embedded_Circuits\\" + fileName, FileMode.Create, FileAccess.Write))
        {
            resource.CopyTo(file);
        }
    }

}

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
//Thread.Sleep(50); //to allow Digital to start before this program terminates..

static void findJavaVersion()
{
    ProcessStartInfo psi = new ProcessStartInfo();
    psi.FileName = "java";
    psi.Arguments = " -version";
    psi.RedirectStandardError = true;
    psi.UseShellExecute = false;
    Process pr = Process.Start(psi);

    string strOutput = pr.StandardError.ReadLine().Split(' ')[2].Replace("\"", "");
    if (strOutput.Contains("'java' is not recognized as an internal or external command"))
    {
        MessageBox((IntPtr)0, "Java doesn't seem to be installed on this machine.\n" +
        "Please download & install java, then try again.\n\n" +
        "I determined that Java is missing by trying the command 'java -version' in console,\n" +
        "to which the output was:\n" + strOutput, "Java not found on this machine.", 0);
    throw new ApplicationException("Java not found."); //to not continue unpacking and trying to launch Digital
    }
}