# Agenda de turnos 📖

## Objetivos 📋
Desarrollar un sistema, que permita la administración general de un consultorio (de cara a los administradores): Prestaciones, Profesionales, Pacientes, etc., como así también, permitir a los pacientes, realizar reserva sobre turnos ofrecidos.
Utilizar Visual Studio 2022 community edition y crear una aplicación utilizando ASP.NET MVC Core (versión a definir por el docente, actualmente 8.0).

<hr />

## Enunciado 📢
La idea principal de este trabajo práctico, es que Uds. se comporten como un equipo de desarrollo.
Este documento, les acerca, un equivalente al resultado de una primera entrevista entre el cliente y alguien del equipo, el cual relevó e identificó la información aquí contenida. 
A partir de este momento, deberán comprender lo que se está requiriendo y construir dicha aplicación web.

Lo primero que deben hacer es comprender en detalle, que es lo que se espera y se busca como resultado del proyecto, para ello, deben recopilar todas las dudas que tengan entre Uds. y evacuarlas con su nexo (el docente) de cara al cliente. De esta manera, él nos ayudará a conseguir la información ya un poco más procesada. 
Es importante destacar, que este proceso no debe esperarse hacerlo 100% en clase; deben ir contemplandolas de manera independientemente, las unifican y hace una puesta comun dentro del equipo (ya sean de índole funcional o técnicas), en lugar de enviar consultas individuales, se sugiere y solicita que las envien de manera conjunta. 

Al inicio del proyecto, las consultas pueden ser enviadas por correo siguiente el siguiente formato:

Subject: [NT1-<CURSO LETRA>-GRP-<GRUPO NUMERO>] <Proyecto XXX> | Informativo o Consulta

Body: 

1.<xxxxxxxx>
2.< xxxxxxxx>

# Ejemplo
**Subject:** [NT1-A-GRP-5] Agenda de Turnos | Consulta

**Body:**

1.La relación del paciente con Turno es 1:1 o 1:N?
2.Está bien que encaremos la validación del turno activo, con una propiedad booleana en el Turno?

<hr />

Es sumamente importante que los correos siempre tengan:
1.Subject con la referencia, para agilizar cualquier interaccion entre el docente y el grupo
2. Siempre que envien una duda o consulta, pongan en copia a todos los participantes del equipo. 

Nota: A medida que avancemos en la materia, TODAS las dudas relacionadas al proyecto deberán ser canalizadas por medio de Github, y desde alli tendremos: seguimiento y las dudas con comentarios, accesibles por todo el equipo y el avance de las mismas. 

**Crear un Issue nuevo o agregar un comentario sobre un issue en cuestion**, si se requiere asistencia, evacuar una duda o lo que fuese, siempre arrobando al docente, ejemplo: @marianolongoort y agregando las etiquetas correspondientes.


### Proceso de ejecución en alto nivel ☑️
 - Crear un nuevo proyecto en [visual studio](https://visualstudio.microsoft.com/en/vs/) utilizando la template de MVC (Model-View-Controller).
 - Crear todos los modelos definidos y/o detectados por ustedes, dentro de la carpeta Models, cada uno en un archivo separado (Modelos anemicos, modelos sin responsabilidades).
 - En el proyecto trataremos de reducir al mínimo las herencias sobre los modelos anémicos.  Ej. la clase Persona, tendrá especializaciones como ser Empleado, Cliente, Alumno, Profesional, etc. según corresponda al proyecto.
 - Sobre dichos modelos, definir y aplica las restricciones necesarias y solicitadas para cada una de las entidades. [DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-8.0).
 - Agregar las propiedades navegacionales que pudisen faltar, para las relaciones entre las entidades (modelos) nueva que pudieramos generar o encontrar.
 - Agregar las propiedades relacionales, en el modelo donde se quiere alojar la relacion (entidad dependiente). La entidad fuerte solo tendrá una propiedad Navegacional, mientras que la entidad debíl tendrá la propiedad relacional.
 - Crear una carpeta Data en la raíz del proyecto, y crear dentro al menos una clase que representará el contexto de la base de datos (DbContext - los datos a almacenar) para nuestra aplicacion. 
 - Agregar los paquetes necesarios para Incorporar Entity Framework e Identitiy en nuestros proyectos.
 - Crear el DbContext utilizando en esta primera estapa con base de datos en memoria (con fines de testing inicial, introduccion y fine tunning de las relaciones entre modelos). [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext?view=efcore-8.0), [Database In-Memory](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=vs).
 - Agregar las propiedades del tipo DbSet para cada una de las entidades que queremos persistir en el DbContext. Estas propiedades, serán colecciones de tipos que deseamos trabajar en la base de datos. En nuestro caso, serán las Tablas en la base de datos.
 - Agregar Identity a nuestro proyecto, y al menos definir IdentityUser como clase base de Persona en nuestro poryecto. Esto nos facilitará la inclusion de funcionalidades como Iniciar y cerrar sesion, agregado de entidades de soporte para esto Usuario y Roles que nos serviran para aplicar un control de acceso basado en roles (RBAC) basico. 
 - Por medio de Scaffolding, crear en esta instancia todos los CRUD (Create-Read-Update-Delete)/ABM (Altas-Bajas-Modificaiciones) de las entidades a persistir. Luego verificaremos cuales mantenemos, cuales removemos, y cuales adecuaremos para darle forma a nuestra WebApp.
 - Antes de continuar es importante realizar algun tipo de pre-carga de la base de datos. No solo es requisito del proyecto, sino que les ahorrara mucho tiempo en las pruebas y adecuaciones de los ABM.
 - Testear en detalle los ABM generados, y detectar todas las modificaciones requeridas para nuestros ABM e interfaces de usuario faltantes para resolver funcionalidades requeridas. (siempre tener presente el checklist de evaluacion final, que les dara el rumbo para esto).
 - Cambiar el dabatabase service provider de Database In Memory a SQL. Para aquellos casos que algunos alumnos utilicen MAC, tendran dos opciones para avanzar (adecuar el proyecto, para utilizar SQLLite o usar un docker con SQL Server instalado alli).
 - Aplicar las adecuaciones y validaciones necesarias en los controladores.  
 - Si el proyecto lo requiere, generar el proceso de auto-registración. Es importante aclarar que este proceso debe ser adecuado según las necesidades de cada proyecto, sus entidades y requerimientos al momento de auto-registrar; no seguir explicitamente un registro tan simple como email y password. 
 - A estas alturas, ya se han topado con varios inconvenientes en los procesos de adecuacion de las vistas y por consiguiente es una buena idea que generen ViewModels para desbloquear esas problematicas que nos estan trayendo los Modelos anemicos utilizados hasta el momento.
 - En el caso de ser requerido en el enunciado, un administrador podrá realizar todas tareas que impliquen interacción del lado del negocio (ABM "Alta-Baja-Modificación" de las entidades del sistema y configuraciones en caso de ser necesarias).
 - El <Usuario Cliente o equivalente> sólo podrá tomar acción en el sistema, en base al rol que que se le ha asignado al momento de auto-registrarse o creado por otro medio o entidad.
 - Realizar todos los ajustes necesarios en los modelos y/o código desde la perspectiva de funcionalidad.
 - Realizar los ajustes requeridos desde la perspectiva de permisos y validaciones.
 - Realizar los ajustes y mejoras en referencia a la presentación de la aplicaión (cuestiones visuales).
 
 Nota: Para la pre-carga de datos, las cuentas creadas por este proceso, deben cumplir las siguientes reglas de manera EXCLUYENTE:
 1. La contraseña por defecto para todas las cuentas pre-cargadas será: Password1!
 2. El UserName y el Email deben seguir la siguiente regla:  <classname>+<rolname si corresponde diferenciar>+<indice>@ort.edu.ar Ej.: cliente1@ort.edu.ar, empleado1@ort.edu.ar, empleadorrhh1@ort.edu.ar

<hr />

## Entidades 📄
- Persona
- Paciente
- Profesional
- Matricula
- Direccion
- Telefono
- Cobertura
- Turno
- Prestacion
- Formulario

## `⚠️Importante: Todas las entidades deben tener su identificador único. Id⚠️`

`
Las propiedades descriptas a continuación, son las mínimas que deben tener las entidades. Uds. pueden proponer agregar las que consideren necesarias. Siempre validar primero con el docente.
De la misma manera Uds. deben definir los tipos de datos asociados a cada una de ellas, como así también las restricciones.
`

**Persona**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- NombreCompleto (Propiedad calculada usando Nombre y Apellido)
- DNI
- Telefonos
- Direccion
```

**Profesional**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- NombreCompleto
- DNI
- Telefonos
- Direccion
- Matricula (datos y tipo)
- Prestacion (servicio o procedimiento específico brindado a un paciente)
- HoraInicio (Horario de atención)
- HoraFin (Horario de atención)
- Turnos (Asignados - Que debe atender)
```

**Paciente**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- NombreCompleto
- DNI
- Telefonos
- Direccion
- Cobertura
- Turnos (Asignados - Para ser atendido)
- Formularios
```

**Direccion**
```
- Calle
- Altura
- Piso
- Departamento
- Persona
```

**Telefono**
```
- CodigoPais
- Prefijo
- Numero
- TipoTelefono (enum)
- Persona
```

**Cobertura**
```
- NumeroCredencial
- Paciente
- Prestadora (enum)
```

**Matricula**
```
- Numero
- Provincia (enum)
- Tipo (enum)
- Profesional
```

**Turno**
```
- Fecha
- Confirmado
- Activo
- FechaAlta
- Paciente
- Profesional
- DescripcionCancelacion
```

**Prestación**
```
- Nombre
- Descripcion
- Duracion (lapso tiempo)
- Precio
- Profesionales
```

**Formulario**
```
- Email
- Fecha
- Nombre
- Apellido
- DNI
- Leido
- Titulo
- Mensaje
- Cobertura 
- Prestadora (enum)
- Usuario
```

**NOTA:** aquí un link para refrescar el uso de los [Data annotations](https://www.c-sharpcorner.com/UploadFile/af66b7/data-annotations-for-mvc/).

<hr />

## Características y Funcionalidades ⌨️
`⚠️Todas las entidades, deben tener implementado su correspondiente ABM, a menos que sea implícito el no tener que soportar alguna de estas acciones.⚠️`
 
 **NOTA:** En el EP3, se deberán presentar las ABM de todoas las entidades, independientemente de que luego sean modificadas o eliminadas. El fin academico de esto, es que tomen contacto con formas de manejar los datos con los usuarios desde una interfaz gráfica de usuario y les sea más facil en el siguiente entregable comprender que deben modificar o adecuar.

## Generalidades 🏠
- La aplicación deberá incluir un logo en el layout
- Deberá contar con información institucional (inventada) relacionada al proyecto.
- Se deben mostrar las Prestaciones ofrecidas, con una descripción de esta, costos, duración típica de la prestación, etc. 
    - Por cada prestación, se debe poder visualizar los profesionales que la brindan, con su Nombre completo y el email. En orden alfabético por Apellido y luego Nombre.
- Los pacientes pueden auto registrarse.
- La auto-registración desde el sitio, es exclusiva para los pacientes. Por lo cual, se le asignará dicho rol.
- Los profesionales, deben ser agregados por un Administrador(Persona).
	- Al momento, del alta del profesional, se le definirá un username y la password será definida por el sistema.
    - También se les asignará a estas cuentas el rol de Profesional.

**Paciente**
- Puede auto registrarse definiendo también su password, a diferencia si es creado por un administrador que le asignará una automáticamente por el sistema.
    - Adicionalmente a datos personales y de contacto, debe cargar una dirección, un teléfono de contacto y su cobertura.    
- Puede solicitar un turno Online en cualquier momento, desde un botón en la parte superior.
    - El proceso será en modo Wizard
        - Selecciona la prestación
        - Selecciona un profesional que brinde dicha prestación (selecciona de una lista)
        - Seleccionará un turno disponible dentro de la oferta.
            - La oferta estará restringida desde el momento de la consulta hasta 7 días posteriores.
            La oferta debe ser:
                - En base a la disponibilidad del profesional, solo días hábiles de lunes a viernes. 
                - Calculará la cantidad de turnos disponibles en base a la duración de la prestación.
                - Ignorar los turnos ya tomados para este profesional, dentro del horario de atención.
            - No debe incluir desde luego turnos tomados.            
            - El paciente, solo puede tener un turno activo. Por lo cual, si tiene un turno activo, no podrá solicitar otro y desde el primer paso del wizard, se le informará que ya tiene un turno activo que bloqueará el avance.
- Mis Turnos
    - El paciente, podrá en todo momento, ver sus turnos, y por consiguiente si tiene o no un turno solicitado activo.
    - Listado de turnos 
        - Fecha, Hora, Prestación, Profesional, Estado.
        - Ordenados por fecha decreciente.
        - Verá el estado, si está o no Activo
        - Solo si está activo, podrá cancelarlo, solo si es hasta 24hs. antes desde ese mismo listado.
            - En caso de no poder cancelarlo por tiempo, debe recibir un mensaje del motivo.
- Un Paciente puede llenar un formulario de contacto, para dejar una consulta.
    - El formulario, puede ser enviado de forma anónima o por un paciente que inició sesión.
        - Si es enviado de forma anónima, se le pedirá que complete todos los datos de contacto.
        - Si el paciente está logueado, se cargarán automáticamente los datos de contacto de este (no modificables para el paciente), solo cargará un Motivo (Titulo) y una descripción (Mensaje)        
- Puede actualizar sus datos de contacto, como teléfonos, su dirección, y datos de su cobertura, pero no puede modificar su DNI, Nombre, Apellido, etc.
- No puede editar datos de otros, ni acceder a ninguna funcionalidad (por URL) que no le corresponda.


**Profesional**
- Puede listar los turnos que tiene asignado el día de hoy para atender.
- Puede listar los turnos que tiene asignado en el futuro, para atender.
    - Puede filtrar por una fecha especifica.
- Puede confirmar/cancelar sus turnos, uno por uno. 
- Puede confirmar/cancelar sus turnos, todos los de un día especifico. 
- Puede confirmar/cancelar sus turnos, todos los de la proxima semana. 

- Puede ver un balance de los turnos que ya realizó en el mes calendario pasado. 
    - Visualizará en este el valor que deberá percibir a fin de mes. Este estará calculado en cantidad de turnos confirmados, en el lapso de tiempo plasmado x el valor hora de la prestación en el momento de la consulta.
Nota: No hay concepto de turnos atendidos. Si el turno quedó confirmado, se considera atendido, se presentara o no el paciente. 

- No pueden confirmar/cancelar turnos de otros profesionales.
- No pueden modificar datos de pacientes.
- No pueden modificar prestaciones, ni otras configuraciones del sistema.
- Pueden listar y leer los formularios de contacto que llegan al sistema, en una lista creciente por fecha.
    - Al leerlos, se marcarán como leidos.
    - Pueden volver a ponerlos como no leidos, con una acción rapida (botón) desde el listado.
    - Pueden filtrar, leidos, no leidos, todos.
    
**Administrador**
Administrador o Administrativo, No es un Administrador de sistema; son las personas que trabajan en administración. 

- Es del tipo Persona, no hay entidad definida para este, es más bien un rol asignado.
- Pueden ejecutar todas las tareas posibles del sistema.
- Puede crear Administradores y Profesionales, con sus respectivos roles. 
- Puede confirmar los turnos de cualquier profesional.
- Puede cancelar un turno en cualquier momento, agregando una descripción del motivo.
- Puede cargar las prestaciones que brinda el centro.
- Dar de alta a profesionales, con su horario de atención, y todos los datos requeridos.

**Turno**
- El turno al crearse, debe estar en estado sin confirmar, y algún administrador o el propio profesional debe confirmar el mismo.
- No se pueden superponer dos turnos del mismo profesional.

Nota: La finalidad del turno confirmado, es que los profesionales de esta clinica tienen varios trabajos y compromisos, por lo cual, la dinámica es que semana a semana, los profesionales, pueden ir manejando sus agendas o pedirle a un administrador que le active los turnos de la agenda de la semana según su disponibilidad.
    En caso de que un profesional no pueda atender un turno, debe ser cancelado y este no implica notificación de ningún tipo al paciente o replanificación.


**Prestacion**
- La duración debe estar expresada en un lapso de tiempo
- No debe ser más largo que 3 horas.

**Aplicación General**
- Nadie, puede eliminar los pacientes del sistema. Esto debe estar restringido.
- Los accesos a las funcionalidades y/o capacidades, debe estar basada en los roles que tenga cada individuo.
- No se pueden repetir/duplicar la combinación de Prestacion.Nombre, Cobertura.NumroCredencial y Matricula.Numero.

`
Nota: El centro tiene consultorios ilimitados y cada consultorio está preparado para soportar cualquier prestación, por lo cual esto no implica restricciones.
`
