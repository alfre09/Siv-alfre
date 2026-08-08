using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Siv.Desktop.Recursos;

public class EstadoVueloToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int estadoId)
        {
            return estadoId switch
            {
                1 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), // Verde - A Tiempo
                2 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")), // Amarillo - Retrasado
                3 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")), // Rojo - Cancelado
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))  // Gris - Desconocido
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BooleanToLeidaTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool leida)
        {
            return leida ? "Sí" : "No";
        }
        return "No";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
