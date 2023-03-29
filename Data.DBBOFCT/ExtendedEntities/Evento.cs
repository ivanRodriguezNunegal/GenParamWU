using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DBBOFCT
{
    public partial class Evento
    {
        /// <summary>
        /// Inserción nuevo registro
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static void Insert(BOFCTEntities db, Evento ev)
        {
            db.Evento.Add(ev);
        }

    }

    
}
