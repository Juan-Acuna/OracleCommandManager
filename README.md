# OracleCommandManager
Una simple clase en C# que a traves de una conexión oracle permite ejecutar el CRUD completo.

## Modo de empleo
Se debe instanciar un objeto de la clase **CommandManger** para poder ejecutar los comandos.
Todos los metodos piden el tipo de objeto a ser manipulado en la base de datos.

### Insert
El metodo **Insert** solicita dos parametros: el objeto a ser insertado y un boolean especificando si el identificacor del objeto se genera automaticamente desde la base de datos. Por defecto este ultimo es verdadero.
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
  	return cmd.Insert(p);                                     //Insertamos el objeto en la base de datos.
	//Tambien se puede usar de la siguiente manera:
	return cmd.Insert<Producto>(p);
}
```

### Get
El metodo **Get** solicita un parametro "id" de tipo _dynamic_(puede ser int o String) y devuelve un objeto del tipo especificado segun la id proporcionada.
#### Ejemplo:

```C#
public Producto ObtenerProducto(){
	int id = 1;
  	OracleConnection con = new OracleConnection(strConexion); //Creamos la conexion.
  	CommandManager cmd = new CommandManager(con);             //Creamos la instancia de CommandManager.
  	return cmd.Get<Producto>(id);                             //Buscamos el objeto en la base de datos.
}
```

### GetAll
El metodo **GetAll** no solicita parametros y devuelve una lista de objetos del tipo especificado(List&lt;T&gt;).
#### Ejemplo:

```C#
public List<Producto> ObtenerProductos(){
  	OracleConnection con = new OracleConnection(strConexion); //Creamos la conexion.
  	CommandManager cmd = new CommandManager(con);             //Creamos la instancia de CommandManager.
  	return cmd.GetAll<Producto>();                            //Obtenemos los objetos de la base de datos y los pasamos como lista.
}
```

### Update
El metodo **Update** solicita un parametro: un objeto del tipo especificado con los valores a actualizar y el identificador. Devuelve un bool indicando si fue posible realizar la acción.
#### Ejemplo:

```C#
public bool ActualizarProducto(){
	Producto p = new Producto
	{       Id = 2,
  		Stock = 100
	};
  	OracleConnection con = new OracleConnection(strConexion); //Creamos la conexion.
  	CommandManager cmd = new CommandManager(con);             //Creamos la instancia de CommandManager.
  	return cmd.Update(p);                                     //Actualizamos el objeto en la base de datos.
	//Tambien se puede usar de la siguiente manera:
	return cmd.Update<Producto>(p);
}
```

### Delete
El metodo **Delete** solicita un parametro: un objeto del tipo especificado con el identificador. Devuelve un bool indicando si fue posible realizar la acción.
#### Ejemplo:

```C#
public bool EliminarProducto(){
	Producto p = new Producto();
	p.Id = 2;
  	OracleConnection con = new OracleConnection(strConexion); //Creamos la conexion.
  	CommandManager cmd = new CommandManager(con);             //Creamos la instancia de CommandManager.
  	return cmd.Delete(p);                                     //Eliminamos el objeto de la base de datos.
	//Tambien se puede usar de la siguiente manera:
	return cmd.Delete<Producto>(p);
}
```

