namespace DesafioFundamentos.Models
{
    public class Estacionamento
    {
        private decimal precoInicial = 0;
        private decimal precoPorHora = 0;
        private List<string> veiculos = new List<string>();

        public Estacionamento(decimal precoInicial, decimal precoPorHora)
        {
            this.precoInicial = precoInicial;
            this.precoPorHora = precoPorHora;
        }

        public void AdicionarVeiculo()
        {
            // Implementado
            Console.WriteLine("Digite a placa do veículo para estacionar:");

            string entradaPlaca = Console.ReadLine();

            // Validação de placa (extra): aceita formatos BR padrão AAA-0A00 ou ABC1234
            // Também normaliza para maiúsculas e remove hífen para persistência consistente
            if (!TentarNormalizarPlaca(entradaPlaca, out string placaNormalizada))
            {
                Console.WriteLine("Placa inválida. Utilize o formato ABC1234 ou ABC1D23.");
                return;
            }

            // Evitar duplicidade (extra): não permitir cadastro repetido (case-insensitive)
            if (veiculos.Any(v => v.ToUpper() == placaNormalizada.ToUpper()))
            {
                Console.WriteLine("Este veículo já está cadastrado no estacionamento.");
                return;
            }

            veiculos.Add(placaNormalizada);
            Console.WriteLine($"Veículo {placaNormalizada} cadastrado com sucesso.");
        }

        public void RemoverVeiculo()
        {
            // Verificar se há veículos para remover
            if (!veiculos.Any())
            {
                Console.WriteLine("Não há veículos estacionados para remover.");
                return;
            }

            // Mostrar lista de veículos disponíveis para facilitar a escolha
            Console.WriteLine("Veículos estacionados:");
            MostrarListaVeiculosNumerada();

            Console.WriteLine("\nDigite a placa do veículo para remover (ou parte dela para busca):");
            string entradaPlaca = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entradaPlaca))
            {
                Console.WriteLine("Entrada inválida.");
                return;
            }

            // Sistema de busca inteligente: autocomplete + filtro
            var veiculosEncontrados = BuscarVeiculosPorPlaca(entradaPlaca.Trim().ToUpper());

            if (!veiculosEncontrados.Any())
            {
                Console.WriteLine("Nenhum veículo encontrado com essa placa ou parte dela.");
                return;
            }

            string placaSelecionada;

            if (veiculosEncontrados.Count == 1)
            {
                // Se encontrou apenas um, usar diretamente
                placaSelecionada = veiculosEncontrados.First();
                Console.WriteLine($"Veículo encontrado: {placaSelecionada}");
            }
            else
            {
                // Se encontrou múltiplos, mostrar menu de seleção
                Console.WriteLine("\nMúltiplos veículos encontrados:");
                for (int i = 0; i < veiculosEncontrados.Count; i++)
                {
                    Console.WriteLine($"{i + 1} - {veiculosEncontrados[i]}");
                }

                Console.WriteLine("Digite o número do veículo desejado:");
                if (!int.TryParse(Console.ReadLine(), out int escolha) ||
                    escolha < 1 || escolha > veiculosEncontrados.Count)
                {
                    Console.WriteLine("Escolha inválida.");
                    return;
                }

                placaSelecionada = veiculosEncontrados[escolha - 1];
            }

            // Processar remoção do veículo selecionado
            ProcessarRemocaoVeiculo(placaSelecionada);
        }

        // Método auxiliar para mostrar lista numerada de veículos
        // Eu cadastrava diversos e esquecia qual era a placa correta e o menu limpava as placas
        // Depois de listar elas, então pensei que uma pequena lista junto com uma busca inteligente
        // Ajudaria nisso
        private void MostrarListaVeiculosNumerada()
        {
            if (!veiculos.Any())
            {
                Console.WriteLine("Nenhum veículo estacionado.");
                return;
            }

            for (int i = 0; i < veiculos.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {veiculos[i]}");
            }
        }

        // Sistema de busca inteligente com autocomplete
        private List<string> BuscarVeiculosPorPlaca(string entrada)
        {
            var resultados = new List<string>();
            
            // Busca exata (case-insensitive)
            var exato = veiculos.FirstOrDefault(v => v.ToUpper() == entrada);
            if (exato != null)
            {
                resultados.Add(exato);
                return resultados;
            }

            // Busca por prefixo (autocomplete)
            var porPrefixo = veiculos.Where(v => v.ToUpper().StartsWith(entrada)).ToList();
            if (porPrefixo.Any())
            {
                resultados.AddRange(porPrefixo);
            }

            // Busca por contém (fallback)
            var porContem = veiculos.Where(v => v.ToUpper().Contains(entrada)).ToList();
            foreach (var veiculo in porContem)
            {
                if (!resultados.Contains(veiculo))
                {
                    resultados.Add(veiculo);
                }
            }

            return resultados.Distinct().ToList();
        }

        // Método auxiliar para processar a remoção
        private void ProcessarRemocaoVeiculo(string placa)
        {
            Console.WriteLine($"\nRemovendo veículo: {placa}");
            Console.WriteLine("Digite a quantidade de horas que o veículo permaneceu estacionado:");

            if (!int.TryParse(Console.ReadLine(), out int horas) || horas < 0)
            {
                Console.WriteLine("Quantidade de horas inválida.");
                return;
            }

            decimal valorTotal = precoInicial + (precoPorHora * horas);

            // Remover o veículo da lista
            if (veiculos.Remove(placa))
            {
                Console.WriteLine($"O veículo {placa} foi removido e o preço total foi de: R$ {valorTotal:F2}");
            }
            else
            {
                Console.WriteLine("Erro ao remover o veículo.");
            }
        }

        public void ListarVeiculos()
        {
            // Verifica se há veículos no estacionamento
            if (veiculos.Any())
            {
                Console.WriteLine("Os veículos estacionados são:");
                MostrarListaVeiculosNumerada();
            }
            else
            {
                Console.WriteLine("Não há veículos estacionados.");
            }
        }

        // Valida e normaliza placas brasileiras (extra)
        // Retorna true se a placa for válida, e fornece a placa normalizada (ABC1234 ou ABC1D23, em maiúsculas)
        private bool TentarNormalizarPlaca(string entrada, out string placaNormalizada)
        {
            placaNormalizada = string.Empty;
            if (string.IsNullOrWhiteSpace(entrada)) return false;

            string raw = entrada.Trim().ToUpper();
            // Remover hífen para facilitar a validação e armazenar sem símbolos
            raw = raw.Replace("-", string.Empty);

            // Formato antigo: 3 letras + 4 dígitos (ABC1234)
            bool antigoValido = System.Text.RegularExpressions.Regex.IsMatch(raw, "^[A-Z]{3}[0-9]{4}$");
            // Mercosul: 3 letras + 1 dígito + 1 letra + 2 dígitos (ABC1D23)
            bool mercosulValido = System.Text.RegularExpressions.Regex.IsMatch(raw, "^[A-Z]{3}[0-9][A-Z][0-9]{2}$");

            if (!(antigoValido || mercosulValido)) return false;

            placaNormalizada = raw; // já está em maiúsculas e sem hífen
            return true;
        }
    }
}
