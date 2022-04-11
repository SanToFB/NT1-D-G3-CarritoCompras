# NT1-D-G3-CarritoCompras



Enunciado

•	Crear un nuevo proyecto en visual studio .
•	Adicionar todos los modelos dentro de la carpeta Models cada uno en un archivo separado.
•	Especificar todas las restricciones y validaciones solicitadas a cada una de las entidades. DataAnnotations .
•	Crear las relaciones entre las entidades
•	Crear una carpeta Data que dentro tendrá al menos la clase que representará el contexto de la base de datos DbContext.
•	Crear el DbContext utilizando base de datos en memoria (con fines de testing inicial). DbContext , Database In-Memory .
•	Agregar los DbSet para cada una de las entidades en el DbContext.
•	Crear el Scaffolding para permitir los CRUD de las entidades al menos solicitadas en el enunciado.
•	Aplicar las adecuaciones y validaciones necesarias en los controladores.
•	Realizar un sistema de login con al menos los roles equivalentes a <Usuario Cliente> y <Usuario Administrador> (o con permisos elevados).
•	Si el proyecto lo requiere, generar el proceso de registración.
•	Un administrador podrá realizar todas tareas que impliquen interacción del lado del negocio (ABM "Alta-Baja-Modificación" de las entidades del sistema y configuraciones en caso de ser necesarias).
•	El <Usuario Cliente> sólo podrá tomar acción en el sistema, en base al rol que tiene.
•	Realizar todos los ajustes necesarios en los modelos y/o funcionalidades.
•	Realizar los ajustes requeridos del lado de los permisos.
•	Todo lo referido a la presentación de la aplicación (cuestiones visuales).
  
  
Especificaciones:

Usuario
•	Los Clientes pueden auto registrarse.
•	La autoregistración desde el sitio, es exclusiva para los clientes. Por lo cual, se le asignará dicho rol.
•	Los empleados, deben ser agregados por otro empleado o administrador.
•	Al momento, del alta del empleado, se le definirá un username y password.
•	También se les asignará a estas cuentas el rol de Empleado.

Cliente
•	Un cliente puede navegar los productos y sus descripciones sin iniciar sesión, de forma anónima.
•	Para agregar productos en cantidad al carrito, debe iniciar sesión primero. (Estaría bueno que pueda sumar al carrito y ahí deja iniciar sesión, para luego finalizar agregando el producto al carrito).
•	El cliente, puede agregar diferentes productos en el carrito, y por cada producto modificar la cantidad que quiere. -- Esta acción, no implica validación en stock. -- El cliente, verá el subtotal, por cada producto/cantidad. -- También, verá el subtotal, del carrito.
•	El cliente, una vez que está satisfecho con su carrito, puede finalizar la compra y elegirá un lugar para retirar.
•	El cliente puede vaciar el carrito.
•	Puede actualizar datos de contacto, como el teléfono, dirección, Obra Social. Pero no puede modificar su DNI, Nombre, Apellido, etc.

Empleado
•	El empleado, puede listar las compras realizadas en el mes, en modo listado, ordenado de forma descendente por valor de compra.
•	Puede dar de alta otros empleados.
•	Puede crear productos, categorías, Sucursales, agregar productos al stock de cada sucursal.
•	Puede habilitar y/o deshabilitar productos.

Producto y Categoría
•	No pueden eliminarse del sistema.
•	Solo los productos pueden deshabilitarse.

Sucursal
•	Cada sucursal, tendrá su propio stock.
•	Y sus datos de locación y contacto.
•	Por el mercado tan volátil, las sucursales, pueden crearse y eliminarse en todo momento. -- Para poder eliminar una sucursal, la misma no tiene que tener productos en su stock.

StockItem
•	Pueden crearse, pero nunca pueden eliminarse desde el sistema. Son dependientes de la sucursal.
•	Puede modificarse la cantidad en todo momento que se dispone de dicho producto, en el stock.
•	Se eliminarán, junto con la sucursal, si esta fuese eliminada.

Carrito
•	El carrito se crea automáticamente con la creación de un cliente, en estado activo.
•	Solo puede haber un carrito activo por usuario en el sistema.
•	Un carrito que no está activo no puede modificarse en ningún aspecto.
•	No se puede eliminar carritos.
•	El carrito, se desactiva al momento de realizarse una compra de manera automatica.
•	Al vaciar el carrito, se eliminan todos los CarritoItems y datos que sean necesarios.
•	El subtotal, es un dato calculado.

CarritoItem
•	El valor unitario del carritoItem, debe actualizarse, al realizar cualquier modificación, según el precio que tenga vigente el producto.
•	El subtotal, debe ser una propiedad calculada, en base a la cantidad x el valor unitario.

Compra
•	Al generarse la compra, el carrito que tiene asociado pasa a estar en estado Inactivo.
•	Al finalizar la compra, se validará si hay disponibles en el stock de la locación que seleccionó el cliente. -- Si hay stock, disminuye el mismo, y crea la compra. -- Si no hay stock, verifica en otras locaciones, si hay stock. --- Si hay en alguna, propone las locaciones o indica que no hay en stock. --- Si seleccionó una nueva locación, finaliza la compra.
•	Al Finalizar la compra, se le muestra le da las gracias al cliente, se le da el Id de compra y los datos de la Sucursal que eligió.
•	No se pueden eliminar las compras.

Aplicación General
•	Información institucional.
•	Se deben mostrar los productos por categoría.
•	Los productos que están deshabilitados deben visualizarse como Pausados. Independientemente, de que haya o no en stock.
•	Los accesos a las funcionalidades y/o capacidades, debe estar basada en los roles que tenga cada individuo
