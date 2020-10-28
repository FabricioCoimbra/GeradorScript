using System;
using System.Windows;

namespace GeraScript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ControllerScript Controller;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Controller = new ControllerScript();
                TxtNome.Text = Controller.NomeIni;
                TxtTrf.Text = Controller.TarefaIni;
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro ao carregar as configurações \n\n" + erro.Message, "Erro");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Controller.GerarScript(TxtNome.Text, TxtTrf.Text);
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro ao tentar gerar o script \n\n" + erro.Message, "Erro");
            }
        }
    }
}
