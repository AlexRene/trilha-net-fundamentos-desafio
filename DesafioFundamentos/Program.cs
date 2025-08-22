using DesafioFundamentos.Models;

// Coloca o encoding para UTF8 para exibir acentuação
Console.OutputEncoding = System.Text.Encoding.UTF8;

decimal precoInicial = 0;
decimal precoPorHora = 0;

Console.WriteLine("Seja bem vindo ao sistema de estacionamento!\n" +
                  "Digite o preço inicial:");
precoInicial = LerDecimal("preço inicial");

Console.WriteLine("Agora digite o preço por hora:");
precoPorHora = LerDecimal("preço por hora");

// Instancia a classe Estacionamento, já com os valores obtidos anteriormente
Estacionamento es = new Estacionamento(precoInicial, precoPorHora);

string opcao = string.Empty;
bool exibirMenu = true;

// Realiza o loop do menu
while (exibirMenu)
{
    Console.Clear();
    Console.WriteLine("Digite a sua opção:");
    Console.WriteLine("1 - Cadastrar veículo");
    Console.WriteLine("2 - Remover veículo");
    Console.WriteLine("3 - Listar veículos");
    Console.WriteLine("4 - Encerrar");

    switch (Console.ReadLine())
    {
        case "1":
            es.AdicionarVeiculo();
            break;

        case "2":
            es.RemoverVeiculo();
            break;

        case "3":
            es.ListarVeiculos();
            break;

        case "4":
            exibirMenu = false;
            break;

        default:
            Console.WriteLine("Opção inválida");
            break;
    }

    Console.WriteLine("Pressione Enter para continuar");
    Console.ReadLine();
}

Console.WriteLine("O programa se encerrou");

// Método auxiliar para parsing robusto de números decimais
// Aceita tanto vírgula quanto ponto como separador decimal
static decimal LerDecimal(string nomeCampo)
{
    while (true)
    {
        string entrada = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(entrada))
        {
            Console.WriteLine($"Por favor, digite um valor válido para {nomeCampo}:");
            continue;
        }

        // Normalizar entrada: substituir vírgula por ponto para parsing consistente
        string entradaNormalizada = entrada.Replace(',', '.');
        
        // Tentar parse usando cultura invariante (sempre usa ponto como separador)
        if (decimal.TryParse(entradaNormalizada, 
            System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, 
            out decimal resultado))
        {
            if (resultado < 0)
            {
                Console.WriteLine($"O {nomeCampo} não pode ser negativo. Digite novamente:");
                continue;
            }
            return resultado;
        }
        
        Console.WriteLine($"Valor inválido para {nomeCampo}. Digite um número válido (ex: 5,50 ou 5.50):");
    }
}
