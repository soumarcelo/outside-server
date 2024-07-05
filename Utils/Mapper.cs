using System.Reflection;

namespace OutsideServer.Utils;

/// <summary>
/// Deprecated
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
public class Mapper<TSource, TDestination> 
{
    public TDestination Map(TSource source)
    {
        TDestination destination = Activator.CreateInstance<TDestination>();
        foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties())
        {
            PropertyInfo? destinationProperty = 
                typeof(TDestination).GetProperty(sourceProperty.Name);

            destinationProperty?.SetValue(destination, sourceProperty.GetValue(source));
        }
        return destination;
    }

    public TDestination Map(TSource source, bool checkIfValueIsNull)
    {
        TDestination destination = Activator.CreateInstance<TDestination>();
        foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties())
        {
            PropertyInfo? destinationProperty =
                typeof(TDestination).GetProperty(sourceProperty.Name);
            if (destinationProperty is not null)
            {
                object? value = sourceProperty.GetValue(source);
                if (checkIfValueIsNull && value is null) continue;
                
                destinationProperty.SetValue(destination, value);
            }
        }
        return destination;
    }

    public TDestination Map(TSource source, TDestination destination, bool checkIfValueIsNull)
    {
        foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties())
        {
            PropertyInfo? destinationProperty =
                typeof(TDestination).GetProperty(sourceProperty.Name);
            if (destinationProperty is not null)
            {
                object? value = sourceProperty.GetValue(source);
                if (checkIfValueIsNull && value is null) continue;

                destinationProperty.SetValue(destination, value);
            }
        }
        return destination;
    }

    public List<TDestination> Map(List<TSource> sourceList)
        => sourceList.Select(Map).ToList();
}
