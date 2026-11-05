# Backend compartido: implementación de compatibilidad

Fecha: 5 de noviembre de 2026.

## Estado

La propuesta inicial se implementó en las cuatro capas originales de escritorio: capaDatos, capaNegocio, capaDAO y capaConexion ahora tienen destino netstandard2.0. La interfaz Windows Forms conserva net10.0-windows y la web conserva .NET Framework 4.7.2.

La compilación y las pruebas sin SQL verifican el uso de las mismas DLL propias desde ambos entornos. Esto no equivale a una migración funcional completa ni a una validación de todas las operaciones contra la base de datos.

## Adaptaciones

- PasswordHasher conserva PBKDF2-HMAC-SHA256, 600000 iteraciones, sal de 16 bytes, salida de 32 bytes y el formato persistido original. Se emplea Microsoft.AspNetCore.Cryptography.KeyDerivation 2.3.13, compatible con netstandard2.0. No se rebajó el algoritmo a SHA-1.
- CriptografiaCompatible usa RandomNumberGenerator.Create para sales/tokens y una comparación de bytes sin terminación anticipada por contenido, con longitud comprobada previamente y sin optimización/inlining. Se verifican resultados funcionales; no se afirma una auditoría de tiempos ni de criptografía independiente.
- Se reemplazó IsDBNull por Convert.IsDBNull y se agregó System.Data.DataSetExtensions 4.5.0 para las operaciones DataTable. La validación de DNI exige ocho caracteres ASCII numéricos.
- SesionUsuario conserva su API para el negocio y delega el estado a IAlmacenSesionUsuario. El escritorio usa su almacén de proceso. La web configura una única vez el adaptador por HttpContext.Session en Application_Start. La identidad no se conserva en un usuario Shared dentro del host web.
- Conexion conserva únicamente configuración privada y una fábrica de conexiones independientes. Se retiraron las transacciones globales sin consumidores. Las transacciones locales existentes en DAO permanecen.
- Se extrajo la configuración de conexión del código original a un archivo privado local; también se admiten variables de entorno. No se ejecutó SQL.

## Consumo desde web

La entrega incluye las cuatro DLL propias y las dependencias Framework en Distribuidos2026-II-Web/Bibliotecas. El proyecto Web Forms referencia esos archivos locales y los copia a bin. La compilación normal ya no requiere el escritorio. tools/BackendWeb y scripts/Build-Backend.ps1 quedan como herramientas opcionales para regenerar y actualizar esas DLL cuando cambie el backend. Las antiguas capas vacías web están fuera de la solución activa.

BIBLIOTECA_BACKEND_ROOT solo se utiliza al ejecutar explícitamente la herramienta de actualización de DLL. La entrega web recompila de forma independiente, como se comprobó en una copia aislada.

## Evidencia

- Compilación de escritorio y web en Debug y Release.
- SHA-256 idéntico en las cuatro DLL propias distribuidas a ambas interfaces en Release.
- Precompilación ASP.NET satisfactoria.
- Pruebas de compatibilidad en tests/CompatibilidadBackend del escritorio: 24 comprobaciones net10.0 y 23 net472. Incluyen vectores PBKDF2 producidos por la API moderna, contraseña Unicode, verificación moderna de nuevos hashes, sal aleatoria, claves incorrectas, hashes corruptos, respuesta normalizada, carga de tipos y usuarios concurrentes.
- Pruebas del adaptador real en tests/SesionWeb: 9 comprobaciones con HttpContext/Session distintos y solicitudes simultáneas; cerrar una sesión no modifica la otra, y falta de sesión HTTP falla explícitamente.

## Límites y pendientes

System.Data.SqlClient 4.9.1 se conserva para no introducir otra migración del acceso SQL en esta etapa. Su obsolescencia produce advertencias BC40000 en compilaciones completas. Web.config incluye redirección a la versión de ensamblado 4.6.2.0 requerida por el consumidor Framework.

No se ejecutaron consultas, escrituras ni migraciones sobre Somee; faltan pruebas funcionales de autenticación, permisos, catálogo y circulación con datos controlados. Tampoco se realizaron pruebas de contraseñas tomadas de la base real; se verificó el formato/algoritmo con vectores independientes.

En verificaciones posteriores se logró iniciar IIS Express, revisar la página en navegador y comprobar el evento de navegación de Inicio. La maestra y el panel usan tablas HTML, controles ASP.NET y código asociado VB, siguiendo el ejemplo de la profesora. No se ha publicado la web. Posteriormente se implementaron los cinco mantenimientos y el acceso indicados en el README, reutilizando las DLL reales. Sus flujos se verificaron en una copia con backend en memoria; no se ejecutaron operaciones de escritura sobre Somee.

La decisión académica sobre aceptar .NET Standard 2.0 corresponde a la consigna del curso; esta implementación demuestra compatibilidad técnica sin atribuir aprobación al profesor.


