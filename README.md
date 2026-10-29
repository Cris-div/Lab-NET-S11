# Laboratorio 11 Eventos e inscripciones

Aplicación ASP.NET Core MVC con EF Core Code First, SQL Server y consultas LINQ. El proyecto es independiente y no tiene conexión Git con el repositorio del profesor.

## Ejecutar

Requisitos: SDK .NET 10 y SQL Server. En este equipo se usa `.\SQLEXPRESS` con autenticación de Windows. La base `EventosDB` ya fue creada mediante las tres migraciones.

Desde PowerShell en esta carpeta:

```powershell
dotnet tool restore
cd Eventos.Web
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

Abrir http://localhost:5291. En VS Code se puede usar F5 con la configuración `Eventos.Web`; primero detener cualquier instancia que esté usando el puerto 5291.

La conexión se configura en `Eventos.Web/appsettings.json`, clave `ConnectionStrings:EventosContext`. En otra computadora solo debe ajustarse el servidor. No crear la base con SQL ni ejecutar scripts: `dotnet ef database update` aplica las migraciones incluidas. No usar `EnsureCreated`.

## Funciones

- Eventos: listado por fecha, filtro por categoría, detalle, creación, edición, modalidad, inscritos confirmados y cupos disponibles. Desactivar cambia `Activo` a `false` y conserva el historial.
- Asistentes: listado por nombres y registro con DNI de ocho dígitos, correo único y teléfono opcional de hasta nueve caracteres.
- Inscripciones: registro con listas activas, validación de fecha, duplicados y aforo; listado con relaciones y cancelación mediante cambio de estado.
- Formularios fuertemente tipados, protección antifalsificación, validación en servidor, Post/Redirect/Get y mensajes TempData.

## Estructura

```text
Eventos.Web/
  Models/           Cuatro entidades y restricciones con Data Annotations
  Data/             EventosContext y fábrica para dotnet ef
  Repositorios/     Consultas LINQ, listas, transacciones y guardado async
  Controllers/      Acciones MVC que reciben repositorios por constructor
  ViewModels/       Filas de eventos con ocupación y filtro
  Views/            Formularios y vistas de los tres módulos
  Migrations/       Tres migraciones reales y EventosContextModelSnapshot
```

La fábrica de contexto permite ejecutar las herramientas EF sin iniciar la carga de datos. Program.cs carga 3 categorías, 4 eventos y 5 asistentes solo si sus respectivas tablas están vacías. Los datos iniciales usan identidades ficticias y correos example.com. Se dejó una inscripción de demostración de Ana Torres al evento de desarrollo web.

## Migraciones

1. `20261029000314_InitialCreate`: crea las cuatro tablas, relaciones e índices únicos.
2. `20261029000350_AgregarModalidadEvento`: agrega Modalidad con valor inicial `Presencial`, aplicado sobre los cuatro eventos ya existentes.
3. `20261029000417_AgregarTelefonoAsistente`: agrega Telefono opcional con longitud máxima 9.

La herramienta `dotnet-ef` está fijada a 10.0.12 en el manifiesto local. La referencia explícita a System.Security.Cryptography.Pkcs 10.0.14 corrige advertencias de una dependencia transitiva; no cambia las migraciones EF.

## Verificación

Compilación sin errores ni advertencias. Las consultas de verificación confirmaron tres filas en `__EFMigrationsHistory`, coincidentes con los archivos.

El registro y la comprobación de cupos se ejecutan en una transacción serializable. La edición impide reducir el aforo por debajo de las inscripciones confirmadas. Las lecturas usan AsNoTracking; el seguimiento se reserva a las entidades que se actualizan.

## Observaciones y conclusiones

Code First permitió crear y evolucionar EventosDB desde los modelos C# sin escribir SQL de creación. Las tres migraciones registran cada cambio y permiten reproducir el esquema en otro equipo. El valor Presencial conservó la validez de los eventos existentes al agregar una propiedad obligatoria.

Los repositorios concentran el acceso a datos y mantienen a los controladores independientes de EventosContext. LINQ y los métodos async permiten consultar las relaciones, calcular la ocupación y guardar los cambios sin bloquear la ejecución durante la espera de la base de datos.

La baja lógica y la cancelación conservan el historial. Las validaciones evitan duplicados, inscripciones fuera de fecha y sobrepasar el aforo, mientras que el flujo Post/Redirect/Get evita repetir envíos al actualizar una página.

No se ha creado ni publicado un repositorio remoto para este laboratorio. Para entregar un enlace de GitHub se debe usar un repositorio propio.
