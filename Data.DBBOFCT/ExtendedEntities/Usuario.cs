using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DBBOFCT
{
    public partial class Usuario
    { 
        public static List<Usuario> GetUsersInGroup(BOFCTEntities db, int idGrupo)
        {
            List<Usuario> lista = new List<Usuario>();
            //LinQ
            lista = (from g in db.Grupo
                     join gu in db.GrupoUsuario on g.GrupoID equals gu.GrupoID
                     join u in db.Usuario on gu.UsuarioID equals u.UsuarioID
                     where g.GrupoID == idGrupo
                     select u).ToList();

            return lista;
            
    }
        
    }
}
