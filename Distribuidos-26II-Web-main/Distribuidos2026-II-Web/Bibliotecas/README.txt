Estas bibliotecas son necesarias para la entrega, igual que capaDatos.dll y capaNegocio.dll en el ejemplo de la profesora.

DLL propias: capaDatos, capaNegocio, capaDAO, capaConexion, compiladas desde el backend original adaptado a .NET Standard 2.0, configuración Release.
Dependencias del consumidor Framework: Microsoft.AspNetCore.Cryptography.Internal 2.3.12, Microsoft.AspNetCore.Cryptography.KeyDerivation 2.3.13, System.Data.SqlClient 4.9.1 (versión de ensamblado 4.6.2.0).

No reemplazar por las DLL net10.0 antiguas. Para actualizar, usar scripts/Build-Backend.ps1 explícitamente; no se necesita ejecutarlo para compilar o presentar la web.
La carpeta LICENCIAS incluye los metadatos NuGet originales con la declaración o enlace de licencia de cada dependencia.

