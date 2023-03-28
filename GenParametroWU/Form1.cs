using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data.DBBOFCT;

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
            List<Grupo> listGrupos = new List<Grupo>();
            listGrupos = Grupo.GetList(db);

            cbGrupos.DataSource = listGrupos;
            cbGrupos.DisplayMember = "Nombre";
            cbGrupos.ValueMember = "GrupoID";
        }


        #endregion

        #region Eventos

        #endregion

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            using (BOFCTEntities db = new BOFCTEntities())
            {
                List<ParametroWidget_Grupo> lista = new List<ParametroWidget_Grupo>();
                lista = ParametroWidget_Grupo.GetList(db, Convert.ToInt32(cbGrupos.SelectedValue.ToString()));

                //Hacer lista para el metodo GetUsersInGroup creado en Usuario
                List<Usuario> listaUsuarios = new List<Usuario>();
                listaUsuarios = Usuario.GetUsersInGroup(db, Convert.ToInt32(cbGrupos.SelectedValue.ToString()));



                //Buscar usuarios pertenecientes a Grupo
                //De esa consulta saldrá una lista de usuarios
                /*
                 * Ejemplo de insercion 
                 */
                Grupo g = db.Grupo.Create();
                g.Nombre = "Test Nombre 28/03/2023";
                g.Descripcion = "Test Descripcion";
                Grupo.Insert(db, g);

                db.SaveChanges();

                //Aquí pondremos otro foreach para recorrernos cada usuario
                //En cada iteración del foreach de usuarios, crearemos un registro en la tabla ParametroWidget_Grupo con GrupoID = null y el UsuarioID que corresponda

                foreach (ParametroWidget_Grupo item in lista)
                {
                    foreach(Usuario item2 in listaUsuarios)
                    {

                    }
                }
                

            }
        }
    }
}
