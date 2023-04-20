using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DBBOFCT
{
    public partial class Country
    {

        /// <summary>
        /// Obtenemos una lista de Items
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<Country> GetList(BOFCTEntities db)
        {
            return (from c in db.Country
                    orderby c.CountryID 
                    select c).ToList();
        }


        /// <summary>
        /// Inserción nuevo registro
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static void Insert(BOFCTEntities db, Country c)
        {
            db.Country.Add(c);
        }
    }
}
