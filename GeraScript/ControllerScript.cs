using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace GeraScript
{
    public class ControllerScript
    {
        private readonly IniFile ini;
        public string NomeIni { get => ini.Read("Nome"); }
        public string TarefaIni { get => ini.Read("Tarefa"); }
        private string ConnectionString { get => ini.Read("ConnectionString"); }
        private readonly SqlConnection Conn;
        private int UltimoId;
        public ControllerScript()
        {
            ini = new IniFile();
            Conn = new SqlConnection(ConnectionString);
        }
        public void GerarScript(string nome, string tarefa)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(tarefa))
                throw new Exception("Nome e Tarefa são obrigatórios");


            ini.Write("Nome", nome);
            ini.Write("Tarefa", tarefa);

            ConectarBase();

            UltimoId = BuscaUltimoID();
            //MessageBox.Show($"Ultimo id encontrado = {UltimoId}");
            InsereRegistro();
            CriarArquivoScript();
        }
        private void ConectarBase()
        {
            try
            {
                Conn.Open();
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao conectar na base model \n\n" + erro.Message);
            }
        }

        private int BuscaUltimoID()
        {
            string queryString = "select max(Cnv_Codigo) + 1 from Conversao";

            try
            {
                var cmd = NewCommand(queryString);
                
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao buscar próximo ID \n\n" + erro.Message);
            }
        }

        private SqlCommand NewCommand(string query)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Conn;
            cmd.CommandTimeout = 15;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            return cmd;
        }

        private void InsereRegistro()
        {
            string insert = "INSERT INTO Conversao (Nr_Tarefa, Cnv_Descricao, Cnv_Dat_Cadastro) VALUES(@Nr_Tarefa, @Cnv_Descricao, GetDate())";

            try
            {
                var cmd = NewCommand(insert);
                cmd.Parameters.AddWithValue("@Nr_Tarefa", UltimoId);
                cmd.Parameters.AddWithValue("@Cnv_Descricao", $"{NomeIni}, {TarefaIni}");
                cmd.ExecuteNonQuery();
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao inserir dados na tabela Conversao \n\n" + erro.Message);
            }
        }
        private void CriarArquivoScript()
        {
            string[] lines = { "", "GO" };

            // Set a variable to the Documents path.
            string caminhoScripts = new Uri(ini.Read("CaminhoScripts")).LocalPath;
            caminhoScripts = Path.Combine(caminhoScripts, $"{UltimoId}.txt");
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(caminhoScripts))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
                outputFile.Close();
            }
            AbrirArquivo(caminhoScripts);
        }
        private static void AbrirArquivo(string caminhoScripts)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = caminhoScripts,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
    }
}
