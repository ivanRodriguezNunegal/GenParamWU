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
using Data.DBBOFCT;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GenParametroWU
{
    public partial class Form1 : Form
    {

        #region Metodos

        public Form1()
        {
            InitializeComponent();
            using (BOFCTEntities db = new BOFCTEntities())
            {
                LoadControls(db);
            }
        }

        private void LoadControls(BOFCTEntities db)
        {
            // Combo Tablas
            List<Grupo> listGrupos = new List<Grupo>();
            listGrupos = Grupo.GetList(db);

            cbGrupos.DataSource = listGrupos;
            cbGrupos.DisplayMember = "Nombre";
            cbGrupos.ValueMember = "GrupoID";


            //rellenamos el combo de las tablas 
            //creamos una instancia de la clase DataToXml
            DataToXml dataToXml = new DataToXml();
            dataToXml.RellenarComboTablas(comboTablas);



            //Combo Country

            List<Country> countriesList = new List<Country>();
            countriesList = Country.GetList(db);

            cbCountries.DataSource = countriesList;
            cbCountries.DisplayMember = "Name";
            cbCountries.ValueMember = "CountryID";


            //rellenamos el combo de paises
           
           dataToXml.FillCountryComboBox(cbCountries);

        }


        #endregion

        #region Eventos

        #endregion


        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            //Buscar usuarios pertenecientes a Grupo
            //De esa consulta saldrá una lista de usuarios

            using (BOFCTEntities db = new BOFCTEntities())
            {
                try
                {

                    List<ParametroWidget_Grupo> lista = new List<ParametroWidget_Grupo>();
                    List<ParametroWidget_Grupo> registrosRedundantes = new List<ParametroWidget_Grupo>();
                    lista = ParametroWidget_Grupo.GetList(db, Convert.ToInt32(cbGrupos.SelectedValue.ToString()));

                    //Hacer lista para el metodo GetUsersInGroup creado en Usuario
                    List<Usuario> listaUsuarios = new List<Usuario>();
                    listaUsuarios = Usuario.GetUsersInGroup(db, Convert.ToInt32(cbGrupos.SelectedValue.ToString()));

                    pgbrEjecucion.Maximum = lista.Count;

                    /*
                    //para que se guarden los cambios realizados en la BD
                    db.SaveChanges();

                    */


                    foreach (ParametroWidget_Grupo item in lista)
                    {
                        foreach (Usuario item2 in listaUsuarios)
                        {
                            //En cada iteración del foreach de usuarios,
                            //crearemos un registro en la tabla ParametroWidget_Grupo con GrupoID = null
                            //y el UsuarioID que corresponda
                            ParametroWidget_Grupo pwg = db.ParametroWidget_Grupo.Create();
                            pwg.ParametroWidgetID = item.ParametroWidgetID;
                            pwg.GrupoID = null;
                            pwg.Valor = item.Valor;
                            pwg.UsuarioID = item2.UsuarioID;



                            //comprobacion de datos redundantes
                            bool duplicado = ParametroWidget_Grupo.CheckRedundant(db, pwg);

                            if (!duplicado)
                            {
                                ParametroWidget_Grupo.Insert(db, pwg);

                                //insertamos un evento de info para registrar en BD
                                Evento evReg = db.Evento.Create();
                                evReg.TipoEventoID = 4;
                                evReg.Asunto = "Inserción ParametroWidget_Grupo";
                                evReg.Mensaje = item2.UsuarioID.ToString();
                                evReg.FechaCreacion = DateTime.Now;
                                Evento.Insert(db, evReg);
                            }
                            else
                            {
                                registrosRedundantes.Add(pwg);
                            }
                        }

                        foreach (ParametroWidget_Grupo redundante in registrosRedundantes)
                        {
                            ParametroWidget_Grupo.Delete(db, redundante);

                            // Insertar evento de info para registrar en BD la eliminación del registro redundante
                            Evento evEliminacion = db.Evento.Create();
                            evEliminacion.TipoEventoID = 5;
                            evEliminacion.Asunto = "Eliminación ParametroWidget_Grupo";
                            evEliminacion.Mensaje = redundante.UsuarioID.ToString();
                            evEliminacion.FechaCreacion = DateTime.Now;
                            Evento.Insert(db, evEliminacion);

                            pgbrEjecucion.Value++;
                        }
            
                    }
                    //mostramos mensaje de finalizado y reset de la progressbar
                    MessageBox.Show("¡Proceso finalizado exitosamente!", "¡Terminado!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pgbrEjecucion.Value = 0;

                    //guardamos cambios en BD
                    db.SaveChanges();

                }
                catch (Exception ex)
                {

                    Evento ev = db.Evento.Create();
                    ev.TipoEventoID = 1;
                    ev.Asunto = "Error";
                    ev.Mensaje = ex.Message;
                    ev.FechaCreacion = DateTime.Now;
                    Evento.Insert(db, ev);
                    db.SaveChanges();

                }
            }
        }

        private void tablaXml_Click(object sender, EventArgs e)
        {

            //Creamos instancia
            /*
            DataToXml dataToXml = new DataToXml();

            List<DataToXml.BackupStructure> listTables = new List<DataToXml.BackupStructure>();

            DataToXml.BackupStructure tabla1 = new DataToXml.BackupStructure();
            tabla1.NameTable = "Grupo";
            tabla1.IDBackup = new List<int> { 4, 7, 15 };
            listTables.Add(tabla1);

            DataToXml.BackupStructure tabla2 = new DataToXml.BackupStructure();
            tabla2.NameTable = "GrupoUsuario";
            tabla2.IDBackup = new List<int> { 10, 12, 14 };
            listTables.Add(tabla2);

            DataToXml.BackupStructure tabla3 = new DataToXml.BackupStructure();
            tabla3.NameTable = "Usuario";
            tabla3.IDBackup = new List<int> { 15, 18, 53 };
            listTables.Add(tabla3);

            dataToXml.Execute(listTables);
            */

            
            DataToXml dataToXml = new DataToXml();
            dataToXml.ExecuteJSON(comboTablas);
             
        }

        private void btnFromXML_Click(object sender, EventArgs e)
        {
            using (BOFCTEntities db = new BOFCTEntities())
            {
                DataToXml dataToXml = new DataToXml();
                dataToXml.RestoreFromJson(db);
            }
        }

        private void btnPaises_Click(object sender, EventArgs e)
        {
            DataToXml dataToXml = new DataToXml();
            dataToXml.LeerTXT(cbCountries);
        }
    }
}
