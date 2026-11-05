using System;
using System.Reflection;
class ConexionProbe {
 static int Main(string[] args) {
  try {
   var a=Assembly.LoadFrom(args[0]);
   var t=a.GetType("capaConexion.Conexion",true);
   var fabrica=t.GetMethod("ObtenerInstancia").Invoke(null,null);
   using(var conexion=(IDisposable)t.GetMethod("CrearConexionIndependiente").Invoke(fabrica,null)) {
    Console.WriteLine("CONEXION_BACKEND_OK");
   }
   return 0;
  } catch(Exception e) { while(e!=null) { Console.WriteLine(e.GetType().FullName); if(e is System.IO.FileNotFoundException) Console.WriteLine(((System.IO.FileNotFoundException)e).FileName); if(e is DllNotFoundException || e is PlatformNotSupportedException) Console.WriteLine(e.Message); e=e.InnerException; } return 1; }
 }
}

