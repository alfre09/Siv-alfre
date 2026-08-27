using Microsoft.Extensions.Logging;
using Moq;
using Siv.Application.Interfaces;
using Siv.Application.Servicios;
using Siv.Domain.Repositorios;

namespace Siv.Tests.Application;

public class NotificacionServicioTests
{
    [Fact]
    public async Task GenerarNotificaciones_DebeAuditarEntregaExitosaYFallida()
    {
        var unidadDeTrabajo = new Mock<IUnitOfWork>();
        var seguimientos = new Mock<ISeguimientoRepositorio>();
        var notificaciones = new Mock<INotificacionRepositorio>();
        var auditoria = new Mock<IAuditoriaServicio>();
        var notificador = new Mock<INotificadorTiempoReal>();
        var usuarios = new Mock<IUsuarioServicio>();
        var correo = new Mock<IEmailServicio>();

        unidadDeTrabajo.Setup(u => u.Seguimientos).Returns(seguimientos.Object);
        unidadDeTrabajo.Setup(u => u.Notificaciones).Returns(notificaciones.Object);
        unidadDeTrabajo.Setup(u => u.GuardarCambiosAsync()).ReturnsAsync(1);
        seguimientos
            .Setup(s => s.ObtenerUsuariosInteresadosAsync(7))
            .ReturnsAsync(new List<string> { "cliente1", "cliente2" });
        notificaciones
            .Setup(n => n.AgregarRangoAsync(It.IsAny<IEnumerable<Siv.Domain.Entidades.Notificacion>>()))
            .Returns(Task.CompletedTask);
        notificador
            .Setup(n => n.EnviarNotificacionAsync("cliente1", It.IsAny<string>()))
            .ReturnsAsync(true);
        notificador
            .Setup(n => n.EnviarNotificacionAsync("cliente2", It.IsAny<string>()))
            .ReturnsAsync(false);
        usuarios
            .Setup(u => u.ObtenerCorreoAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        var servicio = new NotificacionServicio(
            unidadDeTrabajo.Object,
            auditoria.Object,
            notificador.Object,
            usuarios.Object,
            correo.Object,
            new Mock<ILogger<NotificacionServicio>>().Object);

        await servicio.GenerarNotificacionesPorCambioAsync(7, 11, "Cambio de puerta");

        auditoria.Verify(a => a.RegistrarAsync(
            "Enviar",
            "Notificaciones",
            It.Is<string>(d => d.Contains("cliente1") && d.Contains("Enviado")),
            "Sistema",
            It.IsAny<int?>(),
            "Pendiente",
            "Enviado"), Times.Once);
        auditoria.Verify(a => a.RegistrarAsync(
            "ErrorEnvio",
            "Notificaciones",
            It.Is<string>(d => d.Contains("cliente2") && d.Contains("Fallido")),
            "Sistema",
            It.IsAny<int?>(),
            "Pendiente",
            "Fallido"), Times.Once);
    }
}
