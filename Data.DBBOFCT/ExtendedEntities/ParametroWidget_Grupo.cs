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

        /// <summary>
        /// Inserción nuevo registro
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static void Insert(BOFCTEntities db, ParametroWidget_Grupo pwg)
        {
            db.ParametroWidget_Grupo.Add(pwg);
        }

        public static void CheckRedundant(BOFCTEntities db, ParametroWidget_Grupo pwg)
        {
          
            List<ParametroWidget_Grupo> lista = new List<ParametroWidget_Grupo>();
            lista = (from p in db.ParametroWidget_Grupo
                     where p.UsuarioID == pwg.UsuarioID &&
                     p.Valor == pwg.Valor &&
                     p.ParametroWidgetID == pwg.ParametroWidgetID
                     select p).ToList();

            if (lista.Count == 0)
            {
                ParametroWidget_Grupo.Insert(db, pwg);

            }    
           
        }
    }
}

