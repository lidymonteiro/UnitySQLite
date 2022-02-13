using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Persistence.DAO.Specification
{
	public interface ISQLiteConnectionProvider
	{
		SqliteConnection Connection { get; }
	}
}