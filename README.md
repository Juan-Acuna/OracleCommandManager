# OracleCommandManager
Una simple clase en C# que a traves de una conexión oracle permite ejecutar el CRUD completo.

## Modo de empleo
Se debe instanciar un objeto de la clase CommandManger para poder ejecutar los comandos.
Todos los metodos piden el tipo de objeto a ser manipulado en la base de datos.

### Insert
El metodo Insert solicita dos parametros: el objeto a ser insertado y un boolean especificando si el identificacor del objeto se genera automaticamente desde la base de datos.
#### Ejemplo:

```C#
public bool InsertarProducto(){
	Producto p = new Producto
	{       //Asumiremos que producto es de id automatica.
		Nombre = "Bebida",
  		Stock = 100,
  		Precio = 650
	};
  	OracleConnection con = new OracleConnection(strConexion); //Creamos la conexion.
  	CommandManager cmd = new CommandManager(con);             //Creamos la instancia de CommandManager.
  	return cmd.Insert<Producto>(p);                           //Insertamos el objeto en la base de datos.
}
```
	
#####
