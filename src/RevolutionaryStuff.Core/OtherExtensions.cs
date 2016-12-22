using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace RevolutionaryStuff.Core
{
    public static class OtherExtensions
    {
        public static bool IsWeekday(this DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return false;
                default:
                    return true;
            }
        }

        public static bool IsWeekend(this DateTime dt)
        {
            return !dt.IsWeekday();
        }

        public static int Age(this DateTime dt)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - dt.Year;
            age += now.DayOfYear < dt.DayOfYear ? -1 : 0;
            if (age < 0) age = 0;
            return age;
        }

        public static bool IsOdd(this Int32 i)
        {
            return ((i & 0x1) == 1);
        }
        public static bool IsEven(this Int32 i)
        {
            return ((i & 0x1) == 0);
        }

        public static Int16 NonZeroOr(this Int16 i, Int16 fallback)
        {
            if (i == 0) return fallback;
            return i;
        }

        public static Int32 NonZeroOr(this Int32 i, Int32 fallback)
        {
            if (i == 0) return fallback;
            return i;
        }

        public static Int64 NonZeroOr(this Int64 i, Int64 fallback)
        {
            if (i == 0) return fallback;
            return i;
        }

        public static Int16 PositiveOr(this Int16 i, Int16 fallback)
        {
            if (i < 1) return fallback;
            return i;
        }

        public static Int32 PositiveOr(this Int32 i, Int32 fallback)
        {
            if (i < 1) return fallback;
            return i;
        }

        public static Int64 PositiveOr(this Int64 i, Int64 fallback)
        {
            if (i < 1) return fallback;
            return i;
        }

        public static Int16 PositiveOr(this Int16? i, Int16 fallback)
        {
            if (i.GetValueOrDefault() < 1) return fallback;
            return i.Value;
        }

        public static Int32 PositiveOr(this Int32? i, Int32 fallback)
        {
            if (i.GetValueOrDefault() < 1) return fallback;
            return i.Value;
        }

        public static Int64 PositiveOr(this Int64? i, Int64 fallback)
        {
            if (i.GetValueOrDefault() < 1) return fallback;
            return i.Value;
        }

        public static void Substitute<TInt, TImp>(this IServiceCollection services, ServiceLifetime? newServiceLifetime = null, ServiceLifetime? existingServiceLifetime = null)
        {
            var serviceDescriptors = services.Where(s => typeof(TInt).IsA(s.ServiceType) && (existingServiceLifetime == null || s.Lifetime == existingServiceLifetime.Value)).ToList();
            Requires.Positive(serviceDescriptors.Count, nameof(serviceDescriptors));
            foreach (var oldServiceDescriptor in serviceDescriptors)
            {
                services.Remove(oldServiceDescriptor);
                var newServiceDescriptor = new ServiceDescriptor(oldServiceDescriptor.ServiceType, typeof(TImp), newServiceLifetime.GetValueOrDefault(oldServiceDescriptor.Lifetime));
                services.Add(newServiceDescriptor);
            }
        }

        public static void Substitute<TImp>(this IServiceCollection services, ServiceLifetime? newServiceLifetime = null, ServiceLifetime? existingServiceLifetime = null)
            => services.Substitute<TImp, TImp>(newServiceLifetime, existingServiceLifetime);
    }
}
