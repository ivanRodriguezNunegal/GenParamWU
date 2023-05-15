using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Data.DBBOFCT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Forms.ComboBox;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class DataToXml
{
    /// <summary>
    /// Tipo de objeto que le enviaremos a método de backup
    /// </summary>
    public class BackupStructure
    {
        public string NameTable { get; set; }
        public List<int> IDBackup { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="comboBox"></param>
    public void RellenarComboTablas(ComboBox comboBox)
    {
        // Create a connection to the local SQL Server database
        SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=BOFCT;Integrated Security=SSPI;");

        // Open the database connection
        connection.Open();

        // Get the list of table names from the database
        SqlCommand command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
        SqlDataReader reader = command.ExecuteReader();

        // Loop through the table names and add them to the combobox
        while (reader.Read())
        {
            string tableName = reader.GetString(0);
            comboBox.Items.Add(tableName);
        }

        // Close the database connection
        reader.Close();
        connection.Close();
    }

    /// <summary>
    /// Hacemos backup de varias tablas con los registros indicados en el parámetro
    /// </summary>
    /// <param name="comboBox"></param>
    public void Execute(BOFCTEntities db, List<BackupStructure> infoBackup)
    {
        XDocument xmlDoc = new XDocument();
        XElement tablesElement = new XElement("Tables");

        // Hacemos bucle dentro de las tablas que recibimos en infoBackup
        foreach (BackupStructure item in infoBackup)
        {
            string ids = string.Join(",", item.IDBackup);
            XDocument tableXmlDoc = new XDocument();

            // Recuperamos los datos de la sentencia SQL
            string query = "SELECT * FROM " + item.NameTable + " WHERE " + item.NameTable + "ID IN (" + ids + ")";
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = command;

                DataTable tbl = new DataTable();
                dataAdapter.Fill(tbl);

                XElement tableElement = new XElement("Table", new XAttribute("name", item.NameTable));

                //bucleamos para cad una de las filas
                foreach (DataRow row in tbl.Rows)
                {
                    XElement rowElement = new XElement("row");

                    // Iteramos para cada una de las columnas
                    foreach (DataColumn col in tbl.Columns)
                    {
                        string columnName = col.ColumnName;
                        object columnValue = row[col];

                        XElement columnElement = new XElement("Column", new XAttribute("name", columnName));
                        XAttribute valueAttribute = new XAttribute("isNull", (columnValue == null || columnValue == DBNull.Value) ? "true" : "false");
                        XElement valueElement = new XElement("Value", columnValue);
                        valueElement.Add(valueAttribute);
                        columnElement.Add(valueElement);

                        rowElement.Add(columnElement);
                    }

                    tableElement.Add(rowElement);
                }

                tablesElement.Add(tableElement);
            }
        }

        xmlDoc.Add(tablesElement);

        // Por ultimo vamos a guardar el documento generado en la ruta que deseemos 
        string nombreXML = "C:\\temp\\" + "prueba" + ".xml";
        xmlDoc.Save(nombreXML);

        MessageBox.Show("Documento XML guardado en: " + nombreXML);
    }

    //metodo para guardar los datos de una tabla en un documento XML
    public void Execute(ComboBox comboBox)
    {

        // Recuperamos el elemento seleccionado del comboBox
        string tablaSeleccionada = comboBox.SelectedItem.ToString();

        // Hacemos la coexion SQL y lanzamos la query para recuperar los datos 
        string connectionString = "Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123";
        string query = "SELECT * FROM " + tablaSeleccionada;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // Comando SQL, le pasamos los parametros
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            // Con "SqlDataReader" leemos los datos de la consulta realizada
            SqlDataReader reader = command.ExecuteReader();

            // Creamos documento XML y añadimos el elemento raiz con el nombre de la tabla
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Table");
            root.SetAttribute("name", tablaSeleccionada);
            xmlDoc.AppendChild(root);

            // Vamos a iterar en cada fila de los datos y generar una "row" para cada una en el XML
            while (reader.Read())
            {
                XmlElement row = xmlDoc.CreateElement("row");
                root.AppendChild(row);

                // 5. For each column in the row, create a new "Column" element with the name of the column and the value of the data
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    // Comprobamos los null
                    string columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();

                    XmlElement column = xmlDoc.CreateElement("Column");
                    column.SetAttribute("name", columnName);

                    // Hacemos que "Row" esté dentro de "Column"
                    row.AppendChild(column);

                    XmlElement value = xmlDoc.CreateElement("Value");
                    // En caso de que de null
                    if (columnValue == null)
                    {
                        value.SetAttribute("isNull", "true");
                    }
                    else
                    {
                        value.InnerText = columnValue;
                    }
                    column.AppendChild(value);
                }
            }

            // Por ultimo vamos a guardar el documento generado en la ruta que deseemos 
            string nombreXML = "C:\\temp\\" + tablaSeleccionada + ".xml";
            xmlDoc.Save(nombreXML);

            MessageBox.Show("Documento XML guardado en: " + nombreXML);
        }
    }


    public void RestoreFromXml(BOFCTEntities db)
    {
        // Creamos instancia
        OpenFileDialog openFileDialog = new OpenFileDialog();

        // Hcemos que solo nos filtre por archivos "XML"
        openFileDialog.Filter = "XML files (*.xml)|*.xml";

        // Ventana para que el usuario seleccione el archivo
        DialogResult result = openFileDialog.ShowDialog();

        // Si el usuario aha pulsado ok...
        if (result == DialogResult.OK)
        {
            // Cogemos la rta del archivo seleccionado
            string filePath = openFileDialog.FileName;

            // Carga el archivo XML
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            // Crea un "Dictionary" para almacenar las "List" por nombre de tabla
            Dictionary<string, List<Dictionary<string, string>>> tablas = new Dictionary<string, List<Dictionary<string, string>>>();

            // Vamos a hacer un bucle que recorrerá cada uno de nuestros nodos "Table"
            foreach (XmlNode tablaNode in doc.SelectNodes("//Table"))
            {
                string nombreTabla = tablaNode.Attributes["name"].Value;

                // Crea una nueva lista para la cada una de las tablas si no existe
                if (!tablas.ContainsKey(nombreTabla))
                {
                    tablas[nombreTabla] = new List<Dictionary<string, string>>();
                }

                // Recorre cada fila en la tabla
                foreach (XmlNode filaNode in tablaNode.SelectNodes("row"))
                {
                    Dictionary<string, string> fila = new Dictionary<string, string>();

                    // Recorre cada columna en la fila
                    foreach (XmlNode columnaNode in filaNode.SelectNodes("Column"))
                    {
                        string nombreColumna = columnaNode.Attributes["name"].Value;
                        string valorColumna = null;

                        // Obtiene el valor de la columna si no es nulo
                        XmlNode valorNode = columnaNode.SelectSingleNode("Value");
                        if (valorNode != null && valorNode.Attributes?["isNull"]?.Value != "true")
                        {
                            valorColumna = valorNode.InnerText;
                        }

                        // Agrega el valor de la columna al diccionario de la fila
                        fila[nombreColumna] = valorColumna;
                    }

                    // Agrega la fila a la lista de la tabla
                    tablas[nombreTabla].Add(fila);
                }
            }

            //sacamos la lista de grupousuarios
            List<GrupoUsuario> listaGu = new List<GrupoUsuario>();
            //listaGu = LA EXTRAES DEL DICCIONARIO
            if (tablas.ContainsKey("GrupoUsuario"))
            {
                // Obtener la lista de diccionarios correspondiente
                List<Dictionary<string, string>> listaDiccionarios = tablas["GrupoUsuario"];

                // Vamos a crear un bucle para que nos genere los diferentes objetos a partir de las listas de "Dictionary"
                foreach (Dictionary<string, string> diccionario in listaDiccionarios)
                {
                    GrupoUsuario gu = new GrupoUsuario();

                    // Asignar los valores del diccionario a las propiedades del objeto GrupoUsuario
                    gu.GrupoUsuarioID = int.Parse(diccionario["GrupoUsuarioID"]);
                    gu.UsuarioID = int.Parse(diccionario["UsuarioID"]);
                    gu.GrupoID = int.Parse(diccionario["GrupoID"]);

                    // Agregamos el objeto a la lista
                    listaGu.Add(gu);
                }

            }

            //listaUser = LA EXTRAES DEL DICCIONARIO
            List<Usuario> listaUser = new List<Usuario>();

            if (tablas.ContainsKey("Usuario"))
            {
                List<Dictionary<string, string>> listaDiccionarios = tablas["Usuario"];

                // Vamos a crear un bucle para que nos genere los diferentes objetos a partir de las listas de "Dictionary"
                foreach (Dictionary<string, string> diccionario in listaDiccionarios)
                {
                    Usuario usuario = new Usuario();

                    usuario.UsuarioID = int.Parse(diccionario["UsuarioID"]);
                    usuario.Login = diccionario["Login"];
                    usuario.Nombre = diccionario["Nombre"];
                    usuario.Descripcion = diccionario["Descripcion"];
                    usuario.Activo = bool.Parse(diccionario["Activo"]);


                    usuario.FechaCreacion = diccionario["FechaCreacion"] != null ?
                        DateTime.TryParse(diccionario["FechaCreacion"], out DateTime createdDate) ? createdDate : DateTime.MinValue : DateTime.MinValue;

                    usuario.FechaActualizacion = diccionario["FechaActualizacion"] != null ?
                        DateTime.TryParse(diccionario["FechaActualizacion"], out DateTime updatedDate) ? updatedDate : DateTime.MinValue : DateTime.MinValue;

                    usuario.IDEmpleado = diccionario["IDEmpleado"] != null ?
                        int.TryParse(diccionario["IDEmpleado"], out int employeeId) ? employeeId : 0 : 0;

                    usuario.GlobalEmployeeID = diccionario["GlobalEmployeeID"] != null ?
                        int.TryParse(diccionario["GlobalEmployeeID"], out int globalEmployeeId) ? globalEmployeeId : 0 : 0;


                    // Agregamos el objeto a la lista
                    listaUser.Add(usuario);
                }
            }

            //listaGrupo = LA EXTRAES DEL DICCIONARIO
            List<Grupo> listaGrupo = new List<Grupo>();

            if (tablas.ContainsKey("Grupo"))
            {
                List<Dictionary<string, string>> listaDiccionarios = tablas["Grupo"];

                // Vamos a crear un bucle para que nos genere los diferentes objetos a partir de las listas de "Dictionary"
                foreach (Dictionary<string, string> diccionario in listaDiccionarios)
                {
                    Grupo grupo = new Grupo();

                    grupo.GrupoID = int.Parse(diccionario["GrupoID"]);
                    grupo.Nombre = diccionario["Nombre"];
                    grupo.Descripcion = diccionario["Descripcion"];

                    // Agregamos el objeto a la lista
                    listaGrupo.Add(grupo);
                }
            }

            //recorremos la lista de grupousuarios
            foreach (GrupoUsuario grupoUsuario in listaGu)
            {
                //inicializamos el grupousuario
                GrupoUsuario newGu = db.GrupoUsuario.Create();

                //buscamos el UsuarioID del item que estamos recorriendo en la lista de Usuarios
                //si lo encontramos, damos de alta antes el Usuario para poder vincularlo
                //si no, continuamos con el siguiente elemento

                Usuario userToCreate = listaUser.Find(p => p.UsuarioID == grupoUsuario.UsuarioID);
                Usuario newUser = new Usuario();
                if (userToCreate != null)
                {
                    //lo creamos
                    newUser = db.Usuario.Create();
                    //igualamos todos los campos
                    newUser.UsuarioID = userToCreate.UsuarioID;
                    newUser.Login = userToCreate.Login;
                    newUser.Nombre = userToCreate.Nombre;
                    newUser.Descripcion = userToCreate.Descripcion;
                    newUser.Activo = userToCreate.Activo;
                    newUser.FechaCreacion = DateTime.UtcNow;
                    newUser.FechaActualizacion = DateTime.UtcNow;
                    newUser.IDEmpleado = userToCreate.IDEmpleado;
                    newUser.GlobalEmployeeID = userToCreate.GlobalEmployeeID;

                    db.Usuario.Add(newUser);
                    //Lo eliminamos, es la manera de "marcar" que ya está creado
                    listaUser.Remove(userToCreate);
                }
                else
                {
                    //nos vamos a la siguiente iteracion 
                    continue;
                }

                //AHORA HACEMOS LO MISMO CON GRUPO
                Grupo groupToCreate = listaGrupo.Find(g => g.GrupoID == grupoUsuario.GrupoID);
                Grupo newGrupo = new Grupo();
                if (groupToCreate != null)
                {
                    //lo creamos
                    newGrupo = db.Grupo.Create();
                    //igualamos todos los campos
                    newGrupo.GrupoID = groupToCreate.GrupoID;
                    newGrupo.Nombre = groupToCreate.Nombre;
                    newGrupo.Descripcion = groupToCreate.Descripcion;

                    db.Grupo.Add(newGrupo);
                    //Lo eliminamos, es la manera de "marcar" que ya está creado
                    listaGrupo.Remove(groupToCreate);
                }
                else
                {
                    //nos vamos a la siguiente iteracion 
                    continue;
                }

                //Y AHORA ESTABLECEMOS LA RELACION
                newGu.Usuario = newUser;
                //Y....
                newGu.Grupo = newGrupo;

                db.GrupoUsuario.Add(newGu);

            }

            //UNA VEZ HECHO ESTO, NOS RECORREMOS LA LISTA DE USUARIOS Y LA LISTA 
            //DE GRUPOS, POR SI NOS QUEDÓ ALGUNO QUE NO HEMOS DADO DE ALTA AL NO
            //ESTAR RELACIONADO CON NINGUN GRUPOUSUARIO
            foreach (var user in listaUser)
            {
                // Verificar si el usuario no existe en la base de datos
                if (!db.Usuario.Any(u => u.Nombre == user.Nombre))
                {
                    // Si no existe, crear un nuevo usuario en la base de datos
                    var newUser = new Usuario
                    {
                        UsuarioID = user.UsuarioID,
                        Login = user.Login,
                        Nombre = user.Nombre,
                        Descripcion = user.Descripcion,
                        Activo = user.Activo,
                        FechaCreacion = DateTime.UtcNow,
                        FechaActualizacion = DateTime.UtcNow,
                        IDEmpleado = user.IDEmpleado,
                        GlobalEmployeeID = user.GlobalEmployeeID,

                    };
                    db.Usuario.Add(newUser);
                }
            }

            foreach (var group in listaGrupo)
            {
                // Verificar si el grupo no existe en la base de datos
                if (!db.Grupo.Any(g => g.Nombre == group.Nombre))
                {
                    // Si no existe, crear un nuevo grupo en la base de datos
                    var newGroup = new Grupo
                    {
                        GrupoID = group.GrupoID,
                        Nombre = group.Nombre,
                        Descripcion = group.Descripcion,

                    };
                    db.Grupo.Add(newGroup);
                }
            }

            //Y POR ÚLTIMO, HACEMOS EL COMMIT, ES DECIR, PLASMAMOS LO QUE HEMOS HECHO
            //EN LA BASE DE DATOS


            // Try catch para interceptar los errores 
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationResult in ex.EntityValidationErrors)
                {
                    foreach (var error in validationResult.ValidationErrors)
                    {
                        Console.WriteLine("{0}: {1}", error.PropertyName, error.ErrorMessage);
                    }
                }
            }

            /*
                        // Comprobamos los datos por consola
                        foreach (string nombreTabla in tablas.Keys)
                        {
                            Console.WriteLine($"Tabla {nombreTabla}:");

                            // Recorre cada fila en la tabla y muestra sus datos por consola
                            foreach (Dictionary<string, string> fila in tablas[nombreTabla])
                            {
                                Console.WriteLine("Fila:");
                                foreach (string nombreColumna in fila.Keys)
                                {
                                    Console.WriteLine($"  {nombreColumna}: {fila[nombreColumna]}");
                                }
                            }
                        }
            */

        }
    }
    public void FillCountryComboBox(ComboBox comboBox)
    {
        // Conexión a la base de datos
        string connectionString = "Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // Comando SQL para obtener los nombres de los países
            string query = "SELECT Name FROM Country ORDER BY Name";
            SqlCommand command = new SqlCommand(query, connection);

            // Abrir la conexión y leer los datos
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            // Llenar el ComboBox con los nombres de los países
            comboBox.DataSource = null;
            while (reader.Read())
            {
                comboBox.Items.Add(reader["Name"].ToString());
            }

            // Cerrar la conexión y el lector
            reader.Close();
            connection.Close();
        }
    }

    public void LeerTXT(ComboBox comboBox)
    {
        // Mostrar el cuadro de diálogo de abrir archivo
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Archivos de texto|*.txt";
        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return; // El usuario canceló la operación
        }

        List<string[]> data = new List<string[]>();
        using (StreamReader reader = new StreamReader(openFileDialog.FileName))
        {
            // Hacemos la conexion con BD
            string connectionString = "Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Leemos el documento TXT y especificamos en que columnas se encuenta cada uno de los datos
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string campo1 = line.Substring(0, 8);
                string campo2 = line.Substring(16, 4);
                string campo3 = line.Substring(23, 20).Trim(); // eliminamos espacios sobrantes
                string campo4 = line.Substring(43, 55).Trim();
                string campo5 = line.Substring(98, 4);

                // Obtener el ID correspondiente al nombre seleccionado en el ComboBox
                string selectedName = comboBox.SelectedItem.ToString();
                string selectQuery = "SELECT CountryID FROM Country WHERE Name = @Name";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@Name", selectedName);
                int countryID = Convert.ToInt32(selectCommand.ExecuteScalar());
           
                string[] row = { campo1, campo2, campo3, campo4, campo5, countryID.ToString() };
                data.Add(row);
            }


            // INSERCION DE LOS DATOS EN LA BASE DE DATOS
            foreach (string[] row in data)
            {
                //Con este bloqe comprobamos que existe un registro con ese LocalEmployeeID y le asignamos el que tenia
                string selectQuery = "SELECT COUNT(*) FROM Employee WHERE LocalEmployeeID = @LocalEmployeeID";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@LocalEmployeeID", row[4]);
                int count = Convert.ToInt32(selectCommand.ExecuteScalar());

                // Checkeamos que ya exista en BD
                if (count > 0)
                {
                    // Si el registro ya existe en la BD actualizamos todos sus campos excepto su LocalEmployeeID para identificarlo
                    string updateQuery = "UPDATE Employee SET ProfileEmployeeID = @ProfileEmployeeID, StoreNumber = @StoreNumber, Name = @Name, Surname = @Surname, CountryID = @CountryID, Active = @Active, CreationDatetime = @CreationDatetime WHERE LocalEmployeeID = @LocalEmployeeID";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@ProfileEmployeeID", 1000);
                    updateCommand.Parameters.AddWithValue("@StoreNumber", row[1]);
                    updateCommand.Parameters.AddWithValue("@Name", row[2]);
                    updateCommand.Parameters.AddWithValue("@Surname", row[3]);
                    updateCommand.Parameters.AddWithValue("@CountryID", row[5]);
                    updateCommand.Parameters.AddWithValue("@Active", 0);
                    updateCommand.Parameters.AddWithValue("@CreationDatetime", DateTime.UtcNow);
                    updateCommand.Parameters.AddWithValue("@LocalEmployeeID", row[4]);
                    updateCommand.ExecuteNonQuery();
                }

                // Si no existe en BD
                else
                {
                    // Si no lo tenemos en la base de datos, lo insertamos
                    string insertQuery = "INSERT INTO Employee (ProfileEmployeeID, StoreNumber, Name, Surname, LocalEmployeeID, CountryID, Active, CreationDatetime) VALUES (@ProfileEmployeeID, @StoreNumber, @Name, @Surname, @LocalEmployeeID, @CountryID, @Active, @CreationDatetime)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@ProfileEmployeeID", 1000);
                    insertCommand.Parameters.AddWithValue("@StoreNumber", row[1]);
                    insertCommand.Parameters.AddWithValue("@Name", row[2]);
                    insertCommand.Parameters.AddWithValue("@Surname", row[3]);
                    insertCommand.Parameters.AddWithValue("@LocalEmployeeID", row[4]);
                    insertCommand.Parameters.AddWithValue("@CountryID", row[5]);
                    insertCommand.Parameters.AddWithValue("@Active", 0);
                    insertCommand.Parameters.AddWithValue("@CreationDatetime", DateTime.UtcNow);
                    insertCommand.ExecuteNonQuery();
                }
            }

            // Una vez hemos terminado de insertar y actualizar los datos correspondientes

            // Consulta para obtener todos los empleados que tengan el campo "Deleted" a 0
            string selectQuery2 = "SELECT * FROM Employee WHERE Deleted = @Deleted";
            SqlCommand selectCommand2 = new SqlCommand(selectQuery2, connection);
            selectCommand2.Parameters.AddWithValue("@Deleted", 0);
            SqlDataReader reader2 = selectCommand2.ExecuteReader();

            while (reader2.Read())
            {
                // Obtener el LocalEmployeeID del empleado actual
                string localEmployeeID = reader2["LocalEmployeeID"].ToString();

                // Comprobar si el LocalEmployeeID está en la lista de datos
                bool found = false;
                foreach (string[] row in data)
                {
                    if (row[4] == localEmployeeID)
                    {
                        found = true;
                        break;
                    }
                }

                // Si el LocalEmployeeID no está en la lista de datos, actualizar los campos Deleted y EndDate
                if (!found)
                {
                    string updateQuery2 = "UPDATE Employee SET Deleted = @Deleted, EndDate = @EndDate WHERE LocalEmployeeID = @LocalEmployeeID";
                    SqlCommand updateCommand2 = new SqlCommand(updateQuery2, connection);
                    updateCommand2.Parameters.AddWithValue("@Deleted", 1);
                    updateCommand2.Parameters.AddWithValue("@EndDate", DateTime.Now);
                    updateCommand2.Parameters.AddWithValue("@LocalEmployeeID", localEmployeeID);
                    updateCommand2.ExecuteNonQuery();
                }
            }

            // Cerrar el lector de datos
            reader.Close();

        }

    }


    //metodo para guardar los datos de una tabla en un documento JSON

    public void ExecuteJSON(ComboBox comboBox)
    {
        // Recuperamos el elemento seleccionado del comboBox
        string tablaSeleccionada = comboBox.SelectedItem.ToString();

        // Hacemos la conexión SQL y lanzamos la query para recuperar los datos
        string connectionString = "Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123";
        string query = "SELECT * FROM " + tablaSeleccionada;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // Comando SQL, le pasamos los parametros
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            // Con "SqlDataReader" leemos los datos de la consulta realizada
            SqlDataReader reader = command.ExecuteReader();

            // Creamos una lista de diccionarios para almacenar los datos de cada fila
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            // Para cada fila leemos las columnas y sus valores
            while (reader.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);

                    // Comprobamos los null para no añadirlos al documento
                    object columnValue = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);

                    // Si no es null, agregamos la linea con los datos
                    if (columnValue != DBNull.Value)
                    {
                        row.Add(columnName, columnValue);
                    }
                }

                rows.Add(row);
            }

            // Creamos un diccionario adicional con el nombre de la tabla como clave y la lista de diccionarios como valor
            Dictionary<string, List<Dictionary<string, object>>> data = new Dictionary<string, List<Dictionary<string, object>>>();
            data.Add(tablaSeleccionada, rows);

            // Configuramos las opciones de serialización JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
            };

            // Convertimos el diccionario adicional a un objeto JSON
            string json = JsonSerializer.Serialize(data, options);

            // Guardamos el objeto JSON en la ruta deseada
            string nombreJSON = "C:\\temp\\" + tablaSeleccionada + ".json";
            File.WriteAllText(nombreJSON, json);

            MessageBox.Show("Documento JSON guardado en: " + nombreJSON);
        }
    }

    // Metodo para guardar desde JSON
    public void RestoreFromJson(BOFCTEntities db)
    {
        // Conexion a BD
        string connectionString = "Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123";

        // Ventana de seleccion de archivo
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            // Leemos todos los datos del JSON seleccionado
            string jsonData = System.IO.File.ReadAllText(openFileDialog.FileName);
            JObject data = JObject.Parse(jsonData);

            // Insertamos cada una de las filas en la tabla "Usuario"
            foreach (JObject usuario in data["Usuario"])
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Usuario (Login, Nombre, Descripcion, Activo) " +
                                 "VALUES (@Login, @Nombre, @Descripcion, @Activo)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Login", (string)usuario["Login"]);
                        command.Parameters.AddWithValue("@Nombre", (string)usuario["Nombre"]);
                        command.Parameters.AddWithValue("@Descripcion", (string)usuario["Descripcion"]);
                        command.Parameters.AddWithValue("@Activo", (bool)(((int)usuario["Activo"]) != 0));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        // Hacemos saber al usuario que los datos han sido correctamente guardados
        MessageBox.Show("Datos a BD correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

}
