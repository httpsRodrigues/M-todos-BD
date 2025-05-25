using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using static PIM.DatabaseConfig;

namespace PIM
{
    public class FuncionarioRepository
    {
        private readonly string _connectionString;

        public FuncionarioRepository(DatabaseConfig databaseConfig)
        {
            _connectionString = databaseConfig.ConnectionString;
        }

        public (bool Sucesso, int IdFuncionario) ValidarCredenciais(string email, string senha)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT id FROM funcionarios WHERE email_corporativo = @email AND senha = @senha";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@senha", senha);

                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int idFuncionario = Convert.ToInt32(result);
                        return (true, idFuncionario);
                    }

                    return (false, 0);
                }
            }
        }

        public (string Nome, string Perfil) ObterDadosUsuario(int idFuncionario)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT nome, perfil FROM funcionarios WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idFuncionario);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (reader["nome"].ToString(), reader["perfil"].ToString());
                        }
                    }
                }
            }
            return (null, null);
        }
    }
}
