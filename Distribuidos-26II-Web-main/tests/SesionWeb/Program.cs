using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using capaDatos;
using appWEB_Distribuidos;
class Program {
 static int checks;
 static void Check(bool result) {if(!result) throw new Exception("Sesiones mezcladas"); Interlocked.Increment(ref checks);}
 static HttpContext Context(string id) {
  var c=new HttpContext(new HttpRequest("","http://localhost/",""),new HttpResponse(new StringWriter()));
  SessionStateUtility.AddHttpSessionStateToContext(c,new HttpSessionStateContainer(id,new SessionStateItemCollection(),new HttpStaticObjectsCollection(),20,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false)); return c;
 }
 static void Main() {
  SesionUsuario.ConfigurarAlmacen(new AlmacenSesionWeb());
  try {var id=SesionUsuario.IdUsuario;throw new Exception("Aceptó una operación sin sesión HTTP");} catch(InvalidOperationException) {checks++;}
  var store=new AlmacenSesionWeb(); var a=Context("A"); var b=Context("B");
  HttpContext.Current=a; store.Actual=new Usuario{IdUsuario=101,NombreRol="Lector"};
  HttpContext.Current=b; Check(SesionUsuario.IdUsuario==0); store.Actual=new Usuario{IdUsuario=202,NombreRol="Bibliotecario"};
  HttpContext.Current=a; Check(SesionUsuario.IdUsuario==101); Check(!SesionUsuario.EsPersonal); SesionUsuario.Cerrar();
  HttpContext.Current=b; Check(SesionUsuario.IdUsuario==202); Check(SesionUsuario.EsPersonal);
  using(var barrier=new Barrier(2)) {
   var t1=Task.Run(()=>{HttpContext.Current=a; store.Actual=new Usuario{IdUsuario=303};barrier.SignalAndWait();Check(SesionUsuario.IdUsuario==303);SesionUsuario.Cerrar();barrier.SignalAndWait();});
   var t2=Task.Run(()=>{HttpContext.Current=b;barrier.SignalAndWait();Check(SesionUsuario.IdUsuario==202);barrier.SignalAndWait();Check(SesionUsuario.IdUsuario==202);});
   Task.WaitAll(t1,t2);
  }
  Console.WriteLine("PASS: "+checks+" comprobaciones del adaptador web real.");
 }
}
