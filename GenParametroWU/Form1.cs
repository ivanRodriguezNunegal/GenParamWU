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

    }
}
