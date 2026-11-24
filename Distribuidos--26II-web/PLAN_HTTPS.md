# Migración de la biblioteca de HTTP a HTTPS

Hay dos conexiones que probar por separado. El navegador abre el sitio Web Forms y el servidor Web Forms llama al ASMX de Somee mediante Libreria. El certificado debe ser válido para el nombre de cada sitio que atienda por HTTPS. El cambio de transporte conserva los métodos SOAP, los eventos ASPX y la base de datos.

## 1. Preparar el cambio

Anotar las dos URL públicas definitivas. Guardar una copia de los Web.config publicados del cliente y del ASMX. Conservar una publicación Release conocida y registrar el resultado de HelloWorld por HTTP como referencia. Hacer las pruebas de escritura con cuentas y datos de ensayo.

## 2. Habilitar primero HTTPS en el ASMX

Configurar el certificado y el enlace HTTPS en el alojamiento de webservicesbiblioteca.somee.com. Abrir por HTTPS la ruta /swBiblioteca.asmx?WSDL, verificar la cadena de confianza, la vigencia y el nombre del dominio. Comprobar que la dirección SOAP anunciada usa HTTPS. Enviar también un POST SOAP HelloWorld directamente a la URL HTTPS del ASMX y esperar «Hola a todos».

## 3. Conectar el cliente al servicio seguro

En el Web.config del proyecto Web Forms, cambiar la dirección del endpoint swBibliotecaSoap a https://webservicesbiblioteca.somee.com/swBiblioteca.asmx. Dentro del binding basicHttpBinding llamado swBibliotecaSoap, agregar el siguiente elemento después de readerQuotas.

```xml
<security mode="Transport" />
```

Conservar allowCookies, los límites de tamaño y el contrato Libreria.swBibliotecaSoap. Generar una nueva publicación Release y revisar el Web.config resultante. Probar desde el cliente publicado para verificar que el servidor web también confía en el certificado.

## 4. Habilitar HTTPS en el sitio Web Forms

Configurar HTTPS para el dominio del cliente. Revisar recursos, enlaces y la URL pública del QR de demostración si se usa. Una vez verificado el acceso HTTPS, configurar las cookies de autenticación y sesión del navegador como seguras y comprobar su atributo Secure. Probar en una sesión nueva después del cambio.

## 5. Comprobar el recorrido completo

- Abrir el cliente por HTTPS y verificar que el navegador carga las páginas y recursos sin advertencias de certificado ni contenido mixto.
- Entrar con un administrador y un lector en dos navegadores independientes. Abrir una página protegida, hacer varios postbacks y salir de una cuenta. La otra debe conservar su sesión y su rol.
- Desde Autores, crear, buscar, editar y eliminar un registro de prueba. Verificar el resultado en la grilla y el usuario en bitácora.
- Con datos de ensayo, reservar dos títulos, entregarlos juntos, registrar la devolución y abrir el comprobante. Verificar los reportes Yape con pagos ficticios.
- Confirmar la URL final del navegador y el endpoint efectivo del Web.config publicado. El candado del navegador comprueba únicamente el primer tramo.

## 6. Si se necesitan ambos protocolos

Durante la transición, el alojamiento puede aceptar HTTP y HTTPS si el plan lo permite. El cliente Web Forms utilizará un solo endpoint SOAP activo por ambiente. Mantener HTTPS para sus llamadas y evitar cualquier reintento automático por HTTP. Cuando ambos tramos estén verificados, redirigir la navegación HTTP del sitio web a HTTPS. Actualizar cada consumidor SOAP a la URL HTTPS antes de retirar HTTP del ASMX: una redirección de un POST puede cambiar el método o fallar.

## Si falla una prueba

Si falla HelloWorld por HTTPS, revisar primero el dominio, el certificado y el enlace del ASMX. Si funciona directamente pero falla desde Web Forms, revisar el binding Transport, la dirección publicada y la confianza del servidor cliente. Si el login funciona pero la sesión se pierde al cambiar de página, comprobar allowCookies y las cookies de sesión en ambos tramos. Mantener la presentación HTTP con datos ficticios mientras se resuelve el problema. Pasar a cuentas reales cuando las comprobaciones de ambos tramos estén completas.
