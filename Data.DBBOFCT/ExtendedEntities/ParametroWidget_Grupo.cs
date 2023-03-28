using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DBBOFCT;

namespace Data.DBBOFCT
{
    public partial class ParametroWidget_Grupo
    {
        public static List<ParametroWidget_Grupo> GetList(BOFCTEntities db, int grupoID)
        {
            List<ParametroWidget_Grupo> lista = new List<ParametroWidget_Grupo>();
            //LinQ
            lista = (from p in db.ParametroWidget_Grupo
                     where p.GrupoID == grupoID
                     select p).ToList();

            return lista;
        }
    }
}

