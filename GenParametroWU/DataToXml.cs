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
            string nombreTabla = "Usuario";

            //lista con los datos de usuarios
            List<Usuario> listaUsuarios = new List<Usuario>();
            listaUsuarios = Usuario.GetList(db);

            //creamos un nuevo documento XML
            XmlDocument xmlDoc = new XmlDocument();

            //ruta donde guardatemos el documento
            string path = @"c:\temp\dataTablaUsuario.xml";

            // Elemento raiz
            XmlElement root = xmlDoc.CreateElement("Table");
            root.SetAttribute("name", nombreTabla);
            xmlDoc.AppendChild(root);

            // miramos que tipo de objetos contiene lista suuarios
            Type objectType = listaUsuarios[0].GetType();

            // bucleamos entre las propiedades del objeto
            foreach (var prop in objectType.GetProperties())
            {
                XmlElement column = xmlDoc.CreateElement("Column");
                column.SetAttribute("name", prop.Name);//nombre para el atributo de cada column
                root.AppendChild(column);

                foreach (var obj in listaUsuarios)
                {
                    // Cogemos los valores dentro de cada column
                    object value = prop.GetValue(obj, null);

                    XmlElement valueElem = xmlDoc.CreateElement("Value");
                    if (value == null)
                    {
                        valueElem.SetAttribute("isNull", "true");
                    }
                    else
                    {
                        valueElem.InnerText = value.ToString();
                    }
                    column.AppendChild(valueElem);
                }
            }

            // Guardamos el xml en una ruta que queramos
            xmlDoc.Save(path);

            // Le hacemos saber al usuario donde se ha guardado el documento
            MessageBox.Show("Documento XML guardado en: " + Path.GetFullPath(path), "Process Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}