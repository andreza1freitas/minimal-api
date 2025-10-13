using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class VeiculoRequestTest
    {
        private static string? token;

        [ClassInitialize]
        public static async Task ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);

            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com",
                Senha = "123456"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDTO),
                Encoding.UTF8,
                "application/json"
            );

            var response = await Setup.client.PostAsync("/administradores/login", content);
            var result = await response.Content.ReadAsStringAsync();

            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            token = admLogado?.Token;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestarCriacaoVeiculo()
        {
            // Arrange
            var veiculoDTO = new VeiculoDTO
            {
                Nome = "Civic",
                Marca = "Honda",
                Ano = 2020
            };

            var content = new StringContent(
                JsonSerializer.Serialize(veiculoDTO),
                Encoding.UTF8,
                "application/json"
            );

            Setup.client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await Setup.client.PostAsync("/veiculos", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(veiculoCriado);
            Assert.AreEqual("Civic", veiculoCriado?.Nome);
            Assert.AreEqual("Honda", veiculoCriado?.Marca);
            Assert.AreEqual(2020, veiculoCriado?.Ano);

            Console.WriteLine($"✅ Veículo criado com ID: {veiculoCriado?.Id}");
        }

        [TestMethod]
        public async Task TestarListagemVeiculos()
        {
            Setup.client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await Setup.client.GetAsync("/veiculos");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(veiculos);
            Assert.IsTrue(veiculos!.Count > 0);

            Console.WriteLine($"📋 Total de veículos cadastrados: {veiculos.Count}");
        }
    }
}
