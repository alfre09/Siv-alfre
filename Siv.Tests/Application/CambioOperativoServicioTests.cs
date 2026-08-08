using Moq;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;
using Siv.Application.Servicios;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Domain.Repositorios;
using Xunit;

namespace Siv.Tests.Application;

public class CambioOperativoServicioTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICambioOperativoRepositorio> _cambioOperativoRepoMock;
    private readonly Mock<IVueloRepositorio> _vueloRepoMock;
    private readonly Mock<IEstadoVueloRepositorio> _estadoVueloRepoMock;
    private readonly Mock<INotificacionServicio> _notificacionServicioMock;
    private readonly Mock<IAuditoriaServicio> _auditoriaMock;
    private readonly CambioOperativoServicio _cambioOperativoServicio;

    public CambioOperativoServicioTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cambioOperativoRepoMock = new Mock<ICambioOperativoRepositorio>();
        _vueloRepoMock = new Mock<IVueloRepositorio>();
        _estadoVueloRepoMock = new Mock<IEstadoVueloRepositorio>();
        _notificacionServicioMock = new Mock<INotificacionServicio>();
        _auditoriaMock = new Mock<IAuditoriaServicio>();

        _unitOfWorkMock.Setup(u => u.CambiosOperativos).Returns(_cambioOperativoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Vuelos).Returns(_vueloRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.EstadosVuelo).Returns(_estadoVueloRepoMock.Object);

        // This is necessary so the transaction executes our delegate
        _unitOfWorkMock
            .Setup(u => u.EjecutarEnTransaccionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(async action => await action());

        _cambioOperativoServicio = new CambioOperativoServicio(
            _unitOfWorkMock.Object,
            _notificacionServicioMock.Object,
            _auditoriaMock.Object,
            new Mock<Microsoft.Extensions.Logging.ILogger<CambioOperativoServicio>>().Object);
    }

    [Fact]
    public async Task RegistrarRetrasoAsync_DebeRegistrarCambioNotificarYAuditar()
    {
        // Arrange
        var dto = new RegistrarRetrasoOAdelantoDto
        {
            VueloId = 1,
            EsAdelanto = false,
            NuevoHorario = DateTime.Now.AddHours(3),
            Causa = "Mal clima"
        };

        var vueloReal = new Vuelo("AB123", 1, 1, 2, DateTime.Now.AddHours(1), 1, "A1");
        var p = typeof(Vuelo).GetProperty("Id"); 
        if (p != null) p.SetValue(vueloReal, 1);
        
        var estadoProgramado = new EstadoVuelo(EstadoVuelo.Programado);
        var pEstado = typeof(EstadoVuelo).GetProperty("Id");
        if(pEstado != null) pEstado.SetValue(estadoProgramado, 1);
        
        var metodo = typeof(Vuelo).GetMethod("SetEstadoActual", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                     typeof(Vuelo).GetMethod("ActualizarEstado", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                     typeof(Vuelo).GetMethod("CambiarEstado", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                     typeof(Vuelo).GetMethod("CambiarEstado", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        if (metodo != null)
        {
            metodo.Invoke(vueloReal, new object[] { estadoProgramado });
        }
        else
        {
            var propEstado = typeof(Vuelo).GetProperty("EstadoActual");
            if (propEstado != null)
            {
                propEstado.SetValue(vueloReal, estadoProgramado);
            }
        }

        _vueloRepoMock.Setup(r => r.ObtenerConDetalleAsync(1)).ReturnsAsync(vueloReal);
        _cambioOperativoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<CambioOperativo>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.GuardarCambiosAsync()).ReturnsAsync(1);
        _notificacionServicioMock.Setup(n => n.GenerarNotificacionesPorCambioAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _cambioOperativoServicio.RegistrarRetrasoOAdelantoAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TipoCambioOperativo.Retraso.ToString(), result.TipoCambio);
        Assert.Equal("Mal clima", result.Causa);
        
        _cambioOperativoRepoMock.Verify(r => r.AgregarAsync(It.IsAny<CambioOperativo>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.GuardarCambiosAsync(), Times.AtLeastOnce);
        _notificacionServicioMock.Verify(n => n.GenerarNotificacionesPorCambioAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        _auditoriaMock.Verify(a => a.RegistrarAsync(
            "Registrar", 
            "CambiosOperativos", 
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorVueloAsync_DebeDevolverElHistorialDeUnVueloPublico()
    {
        var vuelo = new Vuelo(
            "AB123",
            1,
            1,
            2,
            DateTime.UtcNow.AddHours(2),
            1,
            "A1",
            NivelVisibilidad.Publico);
        typeof(Vuelo).BaseType!.GetProperty("Id")!.SetValue(vuelo, 1);

        var cambio = new CambioOperativo(
            1,
            TipoCambioOperativo.CambioPuerta,
            "Mantenimiento",
            "A1",
            "B2");
        typeof(CambioOperativo).BaseType!.GetProperty("Id")!.SetValue(cambio, 10);

        _vueloRepoMock.Setup(r => r.ObtenerConDetalleAsync(1)).ReturnsAsync(vuelo);
        _cambioOperativoRepoMock
            .Setup(r => r.ObtenerPorVueloAsync(1))
            .ReturnsAsync(new List<CambioOperativo> { cambio });

        var resultado = await _cambioOperativoServicio.ObtenerPorVueloAsync(1, "Admin");

        var registro = Assert.Single(resultado);
        Assert.Equal(10, registro.CambioOperativoId);
        Assert.Equal("Cambio de puerta", registro.TipoCambio);
        Assert.Equal("Mantenimiento", registro.Causa);
    }
}
