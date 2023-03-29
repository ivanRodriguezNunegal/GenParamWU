using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DBBOFCT
{
    public partial class Grupo
    {

        /// <summary>
        /// Obtenemos una lista de Items
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Grupo> GetList(BOFCTEntities db)
        {
                return (from g in db.Grupo
                        orderby g.GrupoID
                        select g).ToList(); 
        }


            /// <summary>
            /// Inserción nuevo registro
            /// </summary>
            /// <param name="db"></param>
            /// <returns></returns>
            public static void Insert(BOFCTEntities db, Grupo g)
            {
                db.Grupo.Add(g);
            }
    }
}
