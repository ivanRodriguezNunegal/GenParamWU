using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Data.DBBOFCT;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Forms.ComboBox;


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
    public void RellenarCombo(ComboBox comboBox)
    {


        // Create a connection to the local SQL Server database
        SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123");

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
    public void Execute(List<BackupStructure> infoBackup)
    {    //procesamos

        string ids = "";
        foreach (BackupStructure item in infoBackup)
        {
            foreach (int idActual in item.IDBackup)
            {
                ids += idActual.ToString() + ",";
            }
            ids = ids.TrimEnd(',');
        }
        XDocument xmlDoc = new XDocument();

        // Iteramos y vamos ejecutando la query 
        foreach (var item in infoBackup.Select(bs => bs.NameTable))
        {
            // Recuperamos los datos de la sentencia SQL
            string query2 = "SELECT * FROM " + item + " WHERE " + item + "ID IN (" + ids + ")";
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=BOFCT;User ID=sa;Password=Aulanosa123"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query2, connection);
                SqlDataReader reader = command.ExecuteReader();

                // Miramos cada fila y vamos añadiendo los datos al XML
                while (reader.Read())
                {
                    var table = infoBackup.FirstOrDefault(t => t.NameTable == item);
                    if (table == null)
                    {
                        continue;
                    }

                    XElement rowElement = new XElement("row");

                    // Añadimos a row, cada uno de los datos que queremos guardar en el documento
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        if (!table.IDBackup.Contains(i))
                        {
                            continue;
                        }

                        XElement columnElement = new XElement("Column", new XAttribute("name", columnName));
                        //si es null añadimos el atributto isnull
                        XAttribute valueAttribute = new XAttribute("isNull", reader.IsDBNull(i) ? "true" : "false");
                        XElement valueElement = new XElement("Value", reader.GetValue(i));
                        valueElement.Add(valueAttribute);
                        columnElement.Add(valueElement);
                        rowElement.Add(columnElement);
                    }

                    XElement tableElement = new XElement("Table", new XAttribute("name", item));
                    tableElement.Add(rowElement);
                    xmlDoc.Add(tableElement);
                }

                reader.Close();
            }
        }

        // Agregamos un elemento raíz "Tables"
        XElement tablesElement = new XElement("Tables");
        tablesElement.Add(xmlDoc.Root);
        xmlDoc.Root.ReplaceWith(tablesElement);

        // Por ultimo vamos a guardar el documento generado en la ruta que deseemos 
        string nombreXML = "C:\\temp\\" + "prueba" + ".xml";
        xmlDoc.Save(nombreXML);

        MessageBox.Show("Documento XML guardado en: " + nombreXML);
    }



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
}
