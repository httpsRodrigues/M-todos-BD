using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PIM
{
    public class ChamadoRepository
    {
        private readonly string _connectionString;

        public ChamadoRepository(DatabaseConfig databaseConfig)
        {
            _connectionString = databaseConfig.ConnectionString;
        }

        public List<Chamado> ObterChamadosPorFuncionario(int idFuncionario)
        {
            var chamados = new List<Chamado>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT id, nome, titulo, data_chamado, setor_chamado, 
                           tipo_problema, urgencia, descricao, status 
                           FROM chamados 
                           WHERE id_funcionario = @idFuncionario 
                           ORDER BY data_chamado DESC";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idFuncionario", idFuncionario);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chamados.Add(new Chamado
                            {
                                Id = reader.GetInt32(0),
                                Nome = reader.GetString(1),
                                Titulo = reader.GetString(2),
                                DataChamado = reader.GetDateTime(3),
                                Setor = reader.GetString(4),
                                TipoProblema = reader.GetString(5),
                                Urgencia = reader.GetString(6),
                                Descricao = reader.GetString(7),
                                Status = reader.GetString(8),
                                IdFuncionario = idFuncionario
                            });
                        }
                    }
                }
            }

            return chamados;
        }

        public bool ExcluirChamado(int idChamado, int idFuncionario)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string sql = @"DELETE FROM public.chamados 
                          WHERE id = @id AND id_funcionario = @idFuncionario";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idChamado);
                    cmd.Parameters.AddWithValue("@idFuncionario", idFuncionario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
