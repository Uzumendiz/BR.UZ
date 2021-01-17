using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

/* 
    [PORTUGUESE BRAZIL]
    Este código-fonte é de autoria da <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ é um software privado: você não pode usar,
    redistribuir e/ou modificar sem autorização.

    Você deveria ter recebido uma autorização com esse software/codigo-fonte
    para ter cópias, se você não recebeu, encerre/exclua imediatamente 
    todos os vestígios do mesmo para não ter futuros problemas.

    Atenciosamente, <BR.UZ/>, MomzGames, ExploitTeam.
*/

/*
    [INGLISH USA]
    This source code is written by <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ a private software: you will not be able to use it,
    redistribute it and / or modify without authorization.

    You should have received an authorization with this software/source code
    to have copies, if you did not receive it, immediately shutdown/exclude
    all traces of it so that you have no future problems.

    Sincerely, <BR.UZ/>, MomzGames, ExploitTeam.
*/

namespace PointBlank
{
    public class Debugger
    {
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        public static void RemoveMenuClose()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), 0xF060, 0x00000000);
        }
        public static void ShowMenuClose()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), true), 0xF060, 0x00000000);
        }

        public static void ShowLogo()
        {
            Logger.White(@"=========================================================");
            Logger.Success(@" </> BR UZ 2020.01.22 </>");
            Logger.White(@"=========================================================");
        }

        public static Random random = new Random();
        public static string RandomizeIP()
        {
            return string.Format("{0}.{1}.{2}.{3}", random.Next(1, 255), random.Next(1, 255), random.Next(1, 255), random.Next(1, 255));
        }

        public static string RandomizeMAC()
        {
            byte[] buffer = new byte[6];
            random.NextBytes(buffer);
            return string.Concat(buffer.Select(str => string.Format("{0}:", str.ToString("X2"))).ToArray()).TrimEnd(':');
        }

        public const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length).Select(str => str[random.Next(str.Length)]).ToArray()).Replace(" ", "");
        }

        //public void LoadAssemblyApp()
        //{
        //    Assembly a = Assembly.Load("BR.UZ.exe");
        //    Type myType = a.GetType("BR.UZ");
        //    MethodInfo myMethod = myType.GetMethod("MethodA");
        //    object obj = Activator.CreateInstance(myType);
        //    myMethod.Invoke(obj, null);
        //}

        //public static void Initialize()
        //{
        //    try
        //    {
        //        byte[] buffer = File.ReadAllBytes($"{Environment.CurrentDirectory}/{__p2("nJ2hGaMCaKn6o26FgpPbdb3G1i8TdtIBx2vG0sHOT9KhSQ1Ls+GcsjcegmxI2dZZ")}");
        //        byte[] bufferDec = __p1(buffer, 3);
        //        Assembly asm = Assembly.Load(bufferDec);
        //        if (asm.EntryPoint == null)
        //            throw new ApplicationException("No entry point found!");
        //        else
        //        {
        //            Console.WriteLine(asm.EntryPoint.Module);
        //        }

        //        MethodInfo ePoint = asm.EntryPoint;
        //        object ins = asm.CreateInstance(ePoint.Name);

        //        Thread thread = new Thread(() => { ePoint.Invoke(ins, null); });
        //        thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        //        thread.Start();
        //        thread.Join(); //Wait for the thread to end
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Não foi possivel iniciar o Launcher.");
        //        ForceClose();
        //    }
        //}
    }
}