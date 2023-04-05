using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

public partial class DataToXml
{
    public void Execute()
    {
        using (BOFCTEntities db = new BOFCTEntities())
        {
            // Especificamos nombre de la tabla y ruta para guardar el archivo
            string nombreTabla = "Usuario";
            string path = @"C:\temp\MyData.xml";

            //lista con los datos de usuarios
            List<Usuario> listaUsuarios = new List<Usuario>();
            listaUsuarios = Usuario.GetList(db);


            //creamos documanto XML
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootElement = xmlDocument.CreateElement("Table");
            rootElement.SetAttribute("name", nombreTabla);

            //recorremos los objetos en los datos de la lista de usuarios
            foreach (Usuario obj in listaUsuarios)
            {
                //creamos cada "Row"
                XmlElement rowElement = xmlDocument.CreateElement("Row");

                // Recorremos las propiedades de cada usuario
                foreach (var prop in typeof(Usuario).GetProperties())
                {
                    // Creamos "Column" por cada propiedad
                    XmlElement colElement = xmlDocument.CreateElement("Column");
                    colElement.SetAttribute("name", prop.Name);

                    // Asignamos el valor dentro de la etiqueta "Value"
                    XmlElement valElement = xmlDocument.CreateElement("Value");
                    var value = prop.GetValue(obj);

                    
                    if (value != null)
                    {
                        valElement.InnerText = value.ToString();
                    }
                    // Si elvalor es null lo indicamos creando el atributo correspondiente
                    else
                    {
                        valElement.SetAttribute("isNull", "true");
                    }

                    //"Value" irá dentro de "Column"
                    colElement.AppendChild(valElement);

                    // "Column" irá dentro de "Row"
                    rowElement.AppendChild(colElement);
                }
                // "Row" irá dentro del elemento raiz "Table"
                rootElement.AppendChild(rowElement);
            }

            // Añadimos el elemento raiz
            xmlDocument.AppendChild(rootElement);

            //guardamos el documento
            xmlDocument.Save(path);

            // Mostrar un mensaje al usuario que indique la ubicación del archivo donde se ha guardado el documento XML
            MessageBox.Show("El documento XML ha sido creado y guardado en: " + path);

        }
    }
}