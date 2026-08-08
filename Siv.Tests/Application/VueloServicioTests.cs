using Moq;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;
using Siv.Application.Servicios;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Domain.Repositorios;
using Xunit;

namespace Siv.Tests.Application;

public class VueloServicioTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IVueloRepositorio> _vueloRepoMock;
    private readonly Mock<IAerolineaRepositorio> _aerolineaRepoMock;
    private readonly Mock<IAeropuertoRepositorio> _aeropuertoRepoMock;
    private readonly Mock<IEstadoVueloRepositorio> _estadoVueloRepoMock;
    private readonly Mock<IAuditoriaServicio> _auditoriaMock;
    private readonly Mock<ICambioOperativoServicio> _cambioOperativoMock;
    private readonly VueloServicio _vueloServicio;

    public VueloServicioTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _vueloRepoMock = new Mock<IVueloRepositorio>();
        _aerolineaRepoMock = new Mock<IAerolineaRepositorio>();
        _aeropuertoRepoMock = new Mock<IAeropuertoRepositorio>();
        _estadoVueloRepoMock = new Mock<IEstadoVueloRepositorio>();
        _auditoriaMock = new Mock<IAuditoriaServicio>();
        _cambioOperativoMock = new Mock<ICambioOperativoServicio>();

        _unitOfWorkMock.Setup(u => u.Vuelos).Returns(_vueloRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Aerolineas).Returns(_aerolineaRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Aeropuertos).Returns(_aeropuertoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.EstadosVuelo).Returns(_estadoVueloRepoMock.Object);

        _vueloServicio = new VueloServicio(
            _unitOfWorkMock.Object,
            _auditoriaMock.Object,
            _cambioOperativoMock.Object,
            new Mock<Microsoft.Extensions.Logging.ILogger<VueloServicio>>().Object);
    }

    [Fact]
    public async Task CrearAsync_DebeCrearVueloYRegistrarAuditoria()
    {
        // Arrange
        var dto = new CrearVueloDto
        {
            NumeroVuelo = "AB123",
            AerolineaId = 1,
            AeropuertoOrigenId = 1,
            AeropuertoDestinoId = 2,
            HorarioProgramado = DateTime.Now.AddHours(2),
            Puerta = "A1"
        };
        
        var aerolineaInfo = typeof(Aerolinea).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault();
        var aerolinea = (Aerolinea)aerolineaInfo!.Invoke(Array.Empty<object>())!;
        
        var aeropuertoInfo = typeof(Aeropuerto).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault();
        var aeropuerto = (Aeropuerto)aeropuertoInfo!.Invoke(Array.Empty<object>())!;

        _aerolineaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(aerolinea);
        _aeropuertoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(aeropuerto);
        _aeropuertoRepoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(aeropuerto);
        
        var estadoVuelo = new EstadoVuelo(EstadoVuelo.Programado);
        var p = typeof(EstadoVuelo).GetProperty("Id");
        if(p != null) p.SetValue(estadoVuelo, 1);

        _estadoVueloRepoMock.Setup(r => r.ObtenerPorNombreAsync(EstadoVuelo.Programado)).ReturnsAsync(estadoVuelo);
        _vueloRepoMock.Setup(r => r.ExisteNumeroVueloAsync("AB123", null)).ReturnsAsync(false);
        _vueloRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Vuelo>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync()).ReturnsAsync(1);
        
        var vueloEsperado = new Vuelo("AB123", 1, 1, 2, DateTime.Now.AddHours(2), 1, "A1");
        _vueloRepoMock.Setup(r => r.ObtenerConDetalleAsync(It.IsAny<int>())).ReturnsAsync(vueloEsperado);

        // Act
        var result = await _vueloServicio.CrearAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AB123", result.NumeroVuelo);
        _vueloRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Vuelo>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(), Times.AtLeastOnce);
        _auditoriaMock.Verify(a => a.RegistrarAsync(
            "Crear", 
            "Vuelos", 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<int?>(), 
            It.IsAny<string?>(), 
            It.IsAny<string?>()), Times.Once);
    }
}
