
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using capaDatos;
namespace capaNegocio {
 // Doble de pruebas HTTP. Nunca abre conexiones ni usa SQL.
 static class Datos {
  internal static List<Autor> Autores=new List<Autor>{new Autor{IdAutor=1,NombreAutor="Autor de prueba"}};
  internal static DataTable Categorias=CrearCategorias();
  internal static List<Libro> Libros=new List<Libro>{new Libro{IdLibro=1,TituloLibro="Libro de prueba",EditorialLibro="Editorial",AnoPublicacion=2026,PublicoLibro="General",Autor="Autor de prueba",Categoria="Prueba",EstadoLibro=true}};
  internal static List<Penalizacion> Penalizaciones=new List<Penalizacion>{new Penalizacion{IdPenalizacion=1,TipoPenalizacion="Prueba",MontoPenalizacion=2,EstadoPenalizacion=true}};
  internal static Dictionary<int,DataTable> Ejemplares=new Dictionary<int,DataTable>();
  internal static DataTable CrearCategorias(){var t=new DataTable();t.Columns.Add("id_categoria",typeof(int));t.Columns.Add("nombre_categoria");t.Columns.Add("estado",typeof(bool));t.Rows.Add(1,"Prueba",true);return t;}
  internal static DataTable Ej(int libro){if(!Ejemplares.ContainsKey(libro)){var t=new DataTable();t.Columns.Add("id_ejemplar",typeof(int));t.Columns.Add("activo",typeof(bool));t.Columns.Add("estado");Ejemplares[libro]=t;}return Ejemplares[libro];}
 }
 public class seguridadNegocio {
  public static int ExigirPersonal(){if(SesionUsuario.IdUsuario<=0||!SesionUsuario.EsPersonal)throw new UnauthorizedAccessException("Sin permiso.");return SesionUsuario.IdUsuario;}
 }
 public class usuarioNegocio {
  static string clave="Prueba2026!";
  static Dictionary<string,string> tokens=new Dictionary<string,string>();
  public Usuario IniciarSesion(string email,string password){SesionUsuario.Cerrar();int id=email=="admin@prueba.local"?1:email=="lector@prueba.local"?2:0;if(id==0||password!=clave)return null;var u=new Usuario{IdUsuario=id,NombreUsuario=id==1?"Administrador de prueba":"Lector de prueba",NombreRol=id==1?"Administrador":"Lector",EmailUsuario=email};SesionUsuario.Iniciar(u);return u;}
  public string ObtenerPregunta(string email){if(email!="admin@prueba.local"&&email!="lector@prueba.local")throw new InvalidOperationException("Cuenta de prueba no encontrada.");return "¿Ciudad de prueba?";}
  public string ValidarRespuesta(string email,string respuesta){if(respuesta!="Lima")throw new InvalidOperationException("Respuesta incorrecta.");var t=Guid.NewGuid().ToString();tokens[t]=email;return t;}
  public void CambiarContrasena(string email,string nueva,string confirmar,string token){if(!tokens.ContainsKey(token)||tokens[token]!=email)throw new UnauthorizedAccessException("Verificación requerida.");if(nueva!=confirmar||nueva.Length<8||!nueva.Any(char.IsUpper)||!nueva.Any(char.IsDigit))throw new ArgumentException("La clave no cumple los requisitos.");clave=nueva;tokens.Remove(token);}
  public void CancelarRecuperacion(string t){tokens.Remove(t);}
 }
 public class autorNegocio {
  public List<Autor> listarTodos(){return Datos.Autores.ToList();}
  public Autor buscarPorId(int id){return Datos.Autores.FirstOrDefault(a=>a.IdAutor==id);}
  public bool agregar(string n,string pais,DateTime? nac,DateTime? fin,string bio){seguridadNegocio.ExigirPersonal();if(string.IsNullOrWhiteSpace(n))throw new ArgumentException("Nombre requerido.");Datos.Autores.Add(new Autor{IdAutor=Datos.Autores.Max(a=>a.IdAutor)+1,NombreAutor=n,NacionalidadAutor=pais,FechaNacimientoAutor=nac,FechaFallecimientoAutor=fin,BiografiaAutor=bio});return true;}
  public bool actualizar(int id,string n,string pais,DateTime? nac,DateTime? fin,string bio){var a=buscarPorId(id);if(a==null)return false;a.NombreAutor=n;a.NacionalidadAutor=pais;a.FechaNacimientoAutor=nac;a.FechaFallecimientoAutor=fin;a.BiografiaAutor=bio;return true;}
  public bool eliminar(int id){return Datos.Autores.RemoveAll(a=>a.IdAutor==id)>0;}
 }
 public class categoriaNegocio {
  public DataTable listarCategoria(string estado="Todos"){var t=Datos.Categorias.Clone();t.Columns.Remove("estado");t.Columns.Add("estado",typeof(string));foreach(DataRow r in Datos.Categorias.Rows)t.Rows.Add(r[0],r[1],(bool)r[2]?"Activo":"Inactivo");return t;}
  public DataTable buscarCategoria(string n,string estado="Todos"){var t=listarCategoria();return t;}
  public DataTable obtenerCategoria(int id){var t=Datos.Categorias.Clone();foreach(DataRow r in Datos.Categorias.Rows)if((int)r[0]==id)t.ImportRow(r);return t;}
  public bool agregarCategoria(string n,int estado){if(string.IsNullOrWhiteSpace(n))throw new ArgumentException("Nombre requerido.");Datos.Categorias.Rows.Add(Datos.Categorias.Rows.Count+1,n,estado==1);return true;}
  public bool actualizarCategoria(int id,string n,int estado){foreach(DataRow r in Datos.Categorias.Rows)if((int)r[0]==id){r[1]=n;r[2]=estado==1;return true;}return false;}
  public bool desactivarCategoria(int id){foreach(DataRow r in Datos.Categorias.Rows)if((int)r[0]==id){r[2]=false;return true;}return false;}
 }
 public class libroNegocio {
  public List<Libro> listarTodos(){return Datos.Libros.ToList();}
  public bool agregarLibroCompleto(string t,string e,short anio,string p,bool activo,IList<int> autores,IList<int> categorias,int cantidad){if(autores.Count==0||categorias.Count==0)throw new ArgumentException("Selecciona autores y categorías.");var id=Datos.Libros.Max(l=>l.IdLibro)+1;Datos.Libros.Add(new Libro{IdLibro=id,TituloLibro=t,EditorialLibro=e,AnoPublicacion=anio,PublicoLibro=p,EstadoLibro=activo,Autor=string.Join(", ",autores.Select(i=>Datos.Autores.First(a=>a.IdAutor==i).NombreAutor)),Categoria=string.Join(", ",categorias.Select(i=>Datos.Categorias.Rows.Cast<DataRow>().First(r=>(int)r[0]==i)[1].ToString())),StockTotal=cantidad});for(int i=0;i<cantidad;i++)new ejemplarNegocio().agregar(id);return true;}
  public bool actualizarLibroCompleto(int id,string t,string e,short anio,string p,bool activo,IList<int> autores,IList<int> categorias){var l=Datos.Libros.First(x=>x.IdLibro==id);l.TituloLibro=t;l.EditorialLibro=e;l.AnoPublicacion=anio;l.PublicoLibro=p;l.EstadoLibro=activo;return true;}
  public bool eliminarLibro(int id){Datos.Libros.First(l=>l.IdLibro==id).EstadoLibro=false;return true;}
 }
 public class ejemplarNegocio {
  public DataTable listarPorLibro(int libro){return Datos.Ej(libro);}
  public bool agregar(int libro){var t=Datos.Ej(libro);t.Rows.Add(t.Rows.Count+1,true,"Disponible");return true;}
  public bool cambiarEstado(int libro,int id,bool activo){foreach(DataRow r in Datos.Ej(libro).Rows)if((int)r[0]==id){r[1]=activo;r[2]=activo?"Disponible":"Inactivo";return true;}return false;}
 }
 public class penalizacionNegocio {
  public List<Penalizacion> buscar(string texto,bool? estado){return Datos.Penalizaciones.ToList();}
  public Penalizacion buscarPorId(int id){return Datos.Penalizaciones.FirstOrDefault(p=>p.IdPenalizacion==id);}
  public bool agregarPenalizacion(string t,string d,decimal m,bool estado,int? dias=null){Datos.Penalizaciones.Add(new Penalizacion{IdPenalizacion=Datos.Penalizaciones.Max(p=>p.IdPenalizacion)+1,TipoPenalizacion=t,DescripcionPenalizacion=d,MontoPenalizacion=m,EstadoPenalizacion=estado,DiasDesde=dias});return true;}
  public bool actualizarPenalizacion(int id,string t,string d,decimal m,bool estado,int? dias=null){var p=buscarPorId(id);p.TipoPenalizacion=t;p.DescripcionPenalizacion=d;p.MontoPenalizacion=m;p.EstadoPenalizacion=estado;p.DiasDesde=dias;return true;}
  public bool cambiarEstado(int id,bool estado){buscarPorId(id).EstadoPenalizacion=estado;return true;}
 }
}
