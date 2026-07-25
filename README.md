# Prueba Técnica - API de Inventario

API REST desarrollada con **ASP.NET Core 8**, **Entity Framework Core**, **SQL Server**, **MediatR**, **Clean Architecture** y autenticación mediante **JWT**.

El sistema permite administrar categorías y productos, registrar movimientos de inventario, consultar el kardex, obtener reportes de stock y proteger los endpoints mediante autenticación.

---

## Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- MediatR
- JWT Bearer Authentication
- Swagger / OpenAPI
- Clean Architecture

---

## Estructura de la solución

```text
PruebaTecnica/
├── PruebaTecnica.Api/
├── PruebaTecnica.Application/
├── PruebaTecnica.Domain/
├── PruebaTecnica.Infrastructure/
├── PruebaTecnica.slnx
├── README.md
└── [script de base de datos].sql
```

### Responsabilidad de cada proyecto

- **PruebaTecnica.Api**: controladores, configuración de Swagger, autenticación y punto de entrada.
- **PruebaTecnica.Application**: casos de uso, DTOs, comandos, consultas, handlers e interfaces.
- **PruebaTecnica.Domain**: entidades y reglas principales del dominio.
- **PruebaTecnica.Infrastructure**: Entity Framework Core, SQL Server, JWT, hash de contraseñas e inyección de dependencias.

---

# Requisitos previos

En el dispositivo donde se ejecutará el proyecto deben estar instalados:

1. **.NET 8 SDK**
2. **SQL Server**
3. **SQL Server Management Studio**, Azure Data Studio u otra herramienta compatible
4. **Git**
5. Visual Studio 2022, Visual Studio Code o cualquier editor compatible con .NET

Puedes comprobar la instalación de .NET con:

```powershell
dotnet --list-sdks
```

Debe aparecer una versión `8.0.x`.

También puedes ejecutar:

```powershell
dotnet --version
```

---

# Instalación del proyecto

## 1. Clonar el repositorio

```powershell
git clone https://github.com/PixelGenetics/PruebaTecnica.git
cd PruebaTecnica
```

Repositorio oficial:

```text
https://github.com/PixelGenetics/PruebaTecnica
```

---

## 2. Restaurar las dependencias

Desde la carpeta raíz del proyecto:

```powershell
dotnet restore
```

Después compila la solución:

```powershell
dotnet build
```

La compilación debe finalizar sin errores.

---

# Configuración de SQL Server

## 1. Iniciar SQL Server

Confirma que el servicio de SQL Server esté iniciado.

En Windows puedes abrir:

```text
Servicios > SQL Server
```

El nombre puede variar, por ejemplo:

```text
SQL Server (MSSQLSERVER)
SQL Server (SQLEXPRESS)
```

---

## 2. Ejecutar el script de base de datos

El repositorio incluye un archivo `.sql` con la creación de:

- Base de datos
- Tablas
- Restricciones
- Relaciones
- Datos iniciales

Localiza el script SQL dentro del repositorio.

Desde PowerShell puedes encontrarlo con:

```powershell
Get-ChildItem -Recurse -Filter *.sql
```

Luego:

1. Abre SQL Server Management Studio.
2. Conéctate a tu instancia.
3. Abre el archivo `.sql`.
4. Ejecuta el script completo.
5. Confirma que la base de datos y sus tablas fueron creadas correctamente.

No es necesario ejecutar el script línea por línea.

---

## 3. Verificar la base de datos

Puedes comprobar las tablas con:

```sql
USE InventarioDB;
GO

SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
```

La solución utiliza tablas para:

- Categorías
- Productos
- Movimientos de inventario
- Usuarios

El nombre exacto de la base de datos debe coincidir con el indicado en la cadena de conexión.

---

# Configuración de la cadena de conexión

Abre:

```text
PruebaTecnica.Api/appsettings.json
```

Configura la conexión de acuerdo con tu instalación de SQL Server.

## Ejemplo con SQL Server local y autenticación de Windows

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Ejemplo con SQL Server Express

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Ejemplo con usuario y contraseña de SQL Server

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InventarioDB;User Id=USUARIO_SQL;Password=CONTRASENA_SQL;TrustServerCertificate=True;"
  }
}
```

Reemplaza:

```text
USUARIO_SQL
CONTRASENA_SQL
```

por las credenciales correspondientes.

> No subas contraseñas reales de SQL Server al repositorio.

Si el proyecto utiliza un nombre diferente a `DefaultConnection`, conserva el nombre definido en `Infrastructure/DependencyInjection.cs`.

---

# Configuración de JWT

La API utiliza JWT para autenticar las solicitudes.

La configuración general puede mantenerse en:

```text
PruebaTecnica.Api/appsettings.json
```

Ejemplo:

```json
{
  "Jwt": {
    "Issuer": "PruebaTecnica.Api",
    "Audience": "PruebaTecnica.Client",
    "ExpirationMinutes": 60
  }
}
```

La clave secreta no debería almacenarse en un repositorio público.

## Configurar la clave con User Secrets

Desde la raíz del proyecto:

```powershell
dotnet user-secrets init --project PruebaTecnica.Api
```

Luego registra la clave:

```powershell
dotnet user-secrets set "Jwt:Key" "UNA-CLAVE-LARGA-Y-SEGURA-DE-AL-MENOS-32-CARACTERES" --project PruebaTecnica.Api
```

Para comprobar los secretos configurados:

```powershell
dotnet user-secrets list --project PruebaTecnica.Api
```

En un entorno de producción, utiliza variables de entorno o un administrador de secretos.

---

# Ejecutar la aplicación

Desde la raíz de la solución:

```powershell
dotnet run --project PruebaTecnica.Api
```

La consola mostrará una dirección similar a:

```text
https://localhost:7150
http://localhost:5150
```

Abre Swagger usando la URL HTTPS indicada por la aplicación:

```text
https://localhost:PUERTO/swagger
```

El puerto puede cambiar dependiendo del dispositivo.

También puedes ejecutar el proyecto desde Visual Studio:

1. Abre `PruebaTecnica.slnx`.
2. Selecciona `PruebaTecnica.Api` como proyecto de inicio.
3. Presiona `F5` o el botón de ejecución HTTPS.

---

# Certificado HTTPS local

En un dispositivo nuevo, .NET puede mostrar una advertencia relacionada con el certificado HTTPS.

Ejecuta:

```powershell
dotnet dev-certs https --trust
```

Después reinicia el proyecto.

---

# Crear el primer usuario

La contraseña debe registrarse mediante la API para que se almacene como hash.

No insertes una contraseña en texto plano directamente desde SQL Server.

En Swagger ejecuta:

```http
POST /api/auth/register
```

Ejemplo:

```json
{
  "nombreUsuario": "admin",
  "contrasenia": "Admin123!",
  "nombre": "Administrador",
  "correo": "admin@pruebatecnica.com"
}
```

La respuesta esperada es:

```http
201 Created
```

La columna `contrasenia` de la tabla `[User]` mostrará una cadena extensa porque contiene el hash seguro de la contraseña.

---

# Iniciar sesión

Ejecuta:

```http
POST /api/auth/login
```

Ejemplo:

```json
{
  "nombreUsuario": "admin",
  "contrasenia": "Admin123!"
}
```

La API devolverá información del usuario y un token JWT.

Ejemplo simplificado:

```json
{
  "message": "Inicio de sesión exitoso.",
  "usuario": {
    "usuarioId": 1,
    "nombreUsuario": "admin",
    "nombre": "Administrador",
    "correo": "admin@pruebatecnica.com",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "expiracion": "2026-01-01T12:00:00Z"
  }
}
```

---

# Autorizar solicitudes en Swagger

1. Ejecuta el endpoint de login.
2. Copia únicamente el valor de `token`.
3. Pulsa el botón **Authorize** en Swagger.
4. Pega solamente el token.
5. Confirma la autorización.
6. Ejecuta los endpoints protegidos.

No escribas la contraseña en el botón **Authorize**.

La contraseña se utiliza en el login. El JWT se utiliza para las solicitudes posteriores.

Sin un token válido, los endpoints protegidos deben responder:

```http
401 Unauthorized
```

---

# Endpoints principales

## Autenticación

```http
POST /api/auth/register
POST /api/auth/login
```

## Categorías

```http
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

## Productos

```http
GET    /api/products
GET    /api/products/{id}
POST   /api/products
PATCH  /api/products/{id}
PATCH  /api/products/{id}/estado
DELETE /api/products/{id}
```

El listado admite búsqueda, filtros, paginación y ordenamiento.

## Inventario

```http
POST /api/inventory/movements
GET  /api/inventory/products/{productId}
GET  /api/inventory/products/{productId}/kardex
GET  /api/inventory/stock
```

El sistema evita registrar salidas que dejen el stock en negativo.

---

# Flujo recomendado de prueba

1. Ejecutar el script SQL.
2. Configurar la cadena de conexión.
3. Configurar `Jwt:Key`.
4. Compilar la solución.
5. Ejecutar la API.
6. Registrar un usuario.
7. Iniciar sesión.
8. Autorizar Swagger con el token.
9. Crear una categoría.
10. Crear un producto.
11. Registrar una entrada de inventario.
12. Registrar una salida.
13. Consultar kardex y reporte de stock.

---

# Comandos útiles

## Restaurar dependencias

```powershell
dotnet restore
```

## Compilar

```powershell
dotnet build
```

## Ejecutar

```powershell
dotnet run --project PruebaTecnica.Api
```

## Limpiar archivos generados

```powershell
dotnet clean
```

## Volver a restaurar y compilar

```powershell
dotnet clean
dotnet restore
dotnet build
```

---

# Solución de problemas

## Error: no se puede conectar a SQL Server

Revisa:

- Que SQL Server esté iniciado.
- Que el nombre de la instancia sea correcto.
- Que la base de datos exista.
- Que la cadena de conexión sea válida.
- Que `TrustServerCertificate=True` esté presente para el entorno local.
- Que el usuario SQL tenga permisos sobre la base de datos.

Ejemplos comunes de servidor:

```text
localhost
.
.\SQLEXPRESS
localhost\SQLEXPRESS
NOMBRE-PC\SQLEXPRESS
```

---

## Error: la base de datos no existe

Ejecuta nuevamente el script `.sql` incluido en el repositorio y confirma que el nombre creado coincida con la cadena de conexión.

---

## Error: `401 Unauthorized`

Comprueba que:

- El usuario esté activo.
- El login se haya realizado correctamente.
- El token no haya expirado.
- El token completo se haya agregado en **Authorize**.
- `Jwt:Issuer`, `Jwt:Audience` y `Jwt:Key` sean iguales durante la generación y validación del token.

---

## Error de contraseña en Base64

Esto ocurre cuando la base de datos contiene una contraseña en texto plano o un hash incompatible.

Elimina el usuario de prueba y vuelve a crearlo mediante:

```http
POST /api/auth/register
```

No insertes contraseñas directamente con SQL como:

```sql
UPDATE [User]
SET contrasenia = 'Admin123!';
```

---

## El puerto de Swagger es diferente

El puerto se asigna en la configuración local del proyecto. Utiliza la URL mostrada en la consola al ejecutar:

```powershell
dotnet run --project PruebaTecnica.Api
```

---

## La solución no compila

Ejecuta:

```powershell
dotnet clean
dotnet restore
dotnet build
```

Confirma también que esté instalado el SDK de .NET 8.

---

# Consideraciones de seguridad

- No guardar contraseñas en texto plano.
- No publicar la clave JWT real.
- No subir credenciales de SQL Server.
- Utilizar HTTPS.
- Usar variables de entorno o secretos en producción.
- Cambiar las credenciales de prueba antes de desplegar.
- Configurar una clave JWT extensa y aleatoria.
- Limitar el tiempo de expiración de los tokens.

---

# Estado actual

La solución incluye:

- CRUD de categorías.
- CRUD de productos.
- Búsqueda, filtros, paginación y ordenamiento.
- Activación y desactivación de productos.
- Entradas y salidas de inventario.
- Validación para evitar stock negativo.
- Kardex con saldo acumulado.
- Resumen de entradas, salidas y stock final.
- Reporte de stock con filtros.
- Registro de usuarios.
- Hash seguro de contraseñas.
- Inicio de sesión.
- Autenticación JWT.
- Endpoints protegidos.
- Documentación interactiva con Swagger.
