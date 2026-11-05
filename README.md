# API REST de gimnasio

Proyecto académico ASP.NET Core Web API sobre gestión de socios y membresías. Un socio puede tener varias membresías (relación 1:N). Se usa SQL Server y Entity Framework Core; los controladores usan consultas LINQ directamente sobre el DbContext.

## Ejecutar desde VS Code

Abre la carpeta `Lab12DWA` y, en la terminal integrada, ejecuta:

```powershell
cd Gimnasio.Api
dotnet restore
$env:ConnectionStrings__GimnasioDb = "Server=localhost\SQLEXPRESS;Database=GimnasioDB;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet run
```

Luego abre `/swagger` en la URL indicada por `dotnet run`.

Si cambias una versión de paquete, ejecuta `dotnet restore` y luego `dotnet run`. No copies DLLs antiguas a la carpeta de publicación; elimina `bin` y `obj` si persisten errores de carga después de restaurar.

## Crear la base

En SSMS crea una base vacía llamada `GimnasioDB` y ejecuta [`sql/gimnasio.sql`](sql/gimnasio.sql). El script crea las tablas, restricciones y datos de ejemplo. **El script elimina las tablas existentes antes de crearlas**, así que úsalo solo en una base vacía.

## Endpoints

| Método | Ruta | Uso |
|---|---|---|
| GET | `/api/socios?nombre=Ana&correo=example` | Lista y filtros |
| GET | `/api/socios/{id}` | Socio por id |
| GET | `/api/socios/{id}/membresias` | Ruta anidada |
| POST | `/api/socios` | Crear socio |
| PUT | `/api/socios/{id}` | Actualizar socio |
| DELETE | `/api/socios/{id}` | Eliminar socio (409 si tiene membresías) |
| GET | `/api/membresias?tipoPlan=Mensual&socioId=1` | Lista y filtros |
| GET | `/api/membresias/{id}` | Membresía por id |
| POST | `/api/membresias` | Crear membresía |
| PUT | `/api/membresias/{id}` | Actualizar membresía |
| DELETE | `/api/membresias/{id}` | Eliminar membresía |

El correo del socio y el código de membresía son únicos (409 si se repiten). La validación automática de Data Annotations produce 400. Swagger está habilitado en todos los entornos. El origen CORS se configura en `Cors:Origin`; reemplázalo por el dominio exacto de tu frontend si publicas uno.

## Scaffold (base de datos primero)

El modelo incluido corresponde al esquema de `sql/gimnasio.sql`. Para ejecutar el Scaffold solicitado por el curso, instala la herramienta y genera las entidades desde tu base:

```powershell
dotnet tool install --global dotnet-ef
$env:ConnectionStrings__GimnasioDb = "Server=localhost\SQLEXPRESS;Database=GimnasioDB;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet ef dbcontext scaffold "$env:ConnectionStrings__GimnasioDb" Microsoft.EntityFrameworkCore.SqlServer --context GimnasioContext --context-dir Data --output-dir Models --use-database-names --force
```

Scaffold regenera contexto y modelos. Revisa/reaplica las validaciones y configuraciones personalizadas después de volverlo a ejecutar.

## Despliegue en MonsterASP.NET

1. En el panel crea una base MSSQL y copia sus datos de conexión para tu uso privado. No los publiques en el repositorio ni los envíes por el chat.
2. Ejecuta `sql/gimnasio.sql` en esa base desde la herramienta SQL del panel o SSMS conectado al servidor remoto.
3. Desde `Gimnasio.Api`, ejecuta `dotnet publish -c Release -o publish`. En MonsterASP.NET, puedes subir el contenido de esa carpeta a `wwwroot` por FTP/SFTP, o usar Git deploy/Web Deploy si lo tienes habilitado. No subas `appsettings.Development.json` ni archivos que contengan secretos.
4. En las variables de entorno del sitio configura `ConnectionStrings__GimnasioDb` con la cadena de conexión **Local** que proporciona MonsterASP.NET para una aplicación alojada allí. Si el proveedor lo requiere, usa la opción TLS incluida en su cadena.
5. Configura `Cors__Origin` con el origen exacto del frontend, si tienes uno.
6. Abre `https://<dominio-asignado>/swagger` y realiza allí las pruebas del curso.

La captura muestra el panel, pero no indica la versión de .NET disponible para tu servicio ni los datos de conexión de SQL. Verifica en MonsterASP.NET que el plan permite .NET 10 y copia la cadena desde los detalles de la base. Ninguna credencial debe guardarse en archivos versionados.

## Pruebas a documentar

En la URL publicada, guarda capturas de cada endpoint y de las respuestas 200, 201, 204, 400, 404 y 409. Para 409 prueba correo duplicado, código duplicado y eliminar un socio que ya tiene membresías.
