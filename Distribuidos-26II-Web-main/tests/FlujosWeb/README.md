# Pruebas HTTP aisladas

Estas pruebas usan únicamente una copia de la web en el puerto 51844 y un doble capaNegocio en memoria. La web de entrega conserva sus DLL reales. No se ejecuta SQL ni se modifica Somee.

1. Compilar la solución web real.
2. Ejecutar `tests/FlujosWeb/Preparar-Pruebas.ps1`. La herramienta crea una carpeta nueva y compila el doble de pruebas; requiere SDK .NET 10.
3. Iniciar IIS Express: `iisexpress.exe /path:RUTA_DEL_SITIO_DE_PRUEBAS /port:51844 /systray:false`.
4. Ejecutar `node tests/FlujosWeb/ejecutar-http.mjs` (Node 20 o posterior).

El guion comprueba 35 condiciones: login, altas, edición, bajas confirmadas, relaciones de libros, ejemplares por libro, cambio de estado, cierre de sesión, permisos de lector, recuperación, cambio de contraseña, ViewState entre sesiones y límite de intentos.

Solo en esta copia: admin@prueba.local y lector@prueba.local, clave Prueba2026!, respuesta Lima. Las pruebas cambian la clave ficticia. No son credenciales reales ni se usan en el proyecto de entrega.

El doble prueba la interacción de las páginas con los contratos del backend. No sustituye las pruebas de consultas SQL, concurrencia transaccional ni reglas de la base real. La conexión privada real se abrió y cerró para comprobar conectividad, sin ejecutar consultas.
