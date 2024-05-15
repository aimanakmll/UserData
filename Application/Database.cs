using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application
{
    public abstract class Database
    {
        public abstract List<T> Select<T>(string tablename);

        public abstract void Insert<T>(T item);

        public abstract void Update<T>(T item);
    }

}
